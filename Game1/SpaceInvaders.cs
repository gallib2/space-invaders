using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Game1
{
    public class SpaceInvaders : Game
    {
        public delegate void DelegateActionToCommit(Enemy enemy);
        public delegate bool DelegateCheckToCommit(Enemy enemy);

        public bool IsChangeEnemyDirection { get; set; }
        public bool IsChangeEnemiesIntervalBetweenJumps { get; set; }
        public bool IsCanEnemyMatrixMoveRegular { get; set; }

        public bool IsGameOver { get; set; }
        Random m_RandomTime = new Random();

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<Entity> m_gameComponents;

        List<List<Entity>> m_EnemiesList = new List<List<Entity>>();
        public float GapToWall { get; set; }
        int m_EnemyNumOfRows = 5;
        int m_EnemyNumOfColumns = 9;

        Spaceship m_Spaceship;
        bool m_IsShooting = false;

        MotherShip m_MotherShip;
        public bool MotherShipNeedToPass { get; set; }
        double m_TimeToPass = 0;
        int m_PrevTimeMotherShipPass;
        private int m_TimeMotherShipPass;
        private const int k_MinTimeMotherShipToPass = 10;
        private const int k_MaxTimeMotherShipToPass = 15;

        Texture2D m_TextureBackground;
        Vector2 m_PositionBackground;
        Color m_TintBackground = Color.White;


        public SpaceInvaders()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            m_gameComponents = new List<Entity>();

            for (int i = 0; i < m_EnemyNumOfRows; i++)
            {
                m_EnemiesList.Add(new List<Entity>());
                for (int j = 0; j < m_EnemyNumOfColumns; j++)
                {
                    m_EnemiesList[i].Add(new Enemy(this));
                }
            }

            Enemy.IsEnemyMoveRight = true;
            Enemy.speedMovement = 0.25f;

            m_Spaceship = new Spaceship(this);
            m_Spaceship.Direction = 1f;

            m_gameComponents.Add(m_Spaceship);

            m_MotherShip = new MotherShip(this);
            MotherShipNeedToPass = false;
            MotherShip.speedMovement = 120f;
            m_TimeMotherShipPass = m_RandomTime.Next(1, k_MinTimeMotherShipToPass);

            m_gameComponents.Add(m_MotherShip);

            this.IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            m_TextureBackground = Content.Load<Texture2D>(ImagePathProvider.BackgroundPathImage);

            //m_BulletSpaceShip.Texture = Content.Load<Texture2D>(ImagePathProvider.BulletPathImage);
            //m_BulletSpaceShip.Color = Color.Red;

            for (int i = 0; i < m_gameComponents.Count; i++)
            {
                m_gameComponents[i].LoadContent(Content);
            }

            //foreach (Entity gameComponent in m_gameComponents)
            //{
            //    gameComponent.LoadContent(Content);
            //}

            loadEnemyContent();

            InitPositions();
        }

        private void InitPositions()
        {
            // 1. init the ship position
            // Get the bottom and center:
            float x = (float)GraphicsDevice.Viewport.Width / 2;
            float y = (float)GraphicsDevice.Viewport.Height;

            // Offset:
            x -= m_Spaceship.Texture.Width / 2;
            y -= m_Spaceship.Texture.Height / 2;

            // Put it a little bit higher:
            y -= 30;

            m_Spaceship.Position = new Vector2(x, y);

            // TODO : const row and col...
            int j = 0;
            for (int i = 0; i < 5; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    // TODO : fix the spaces between the enemies
                    x = m_EnemiesList[i][j].Texture.Width / 2 + (j * m_EnemiesList[i][j].Texture.Width * 2);
                    y = m_EnemiesList[i][j].Texture.Height * 3 + (i * m_EnemiesList[i][j].Texture.Height);

                    m_EnemiesList[i][j].Position = new Vector2(x, y);
                }
            }

            m_MotherShip.Position = InitMotherShipPosition();

            // 3. Init the bg position:
            m_PositionBackground = Vector2.Zero;

            //create an alpah channel for background:
            Vector4 bgTint = Vector4.One;
            bgTint.W = 0.4f; // set the alpha component to 0.2
            m_TintBackground = new Color(bgTint);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ImagePathProvider.InitializeImagesPath();
            //ImagePathProvider.InitializeImagesPathStarWars();
            IsCanEnemyMatrixMoveRegular = true;
            base.Initialize();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // get the current input devices state:
            int timeMotherShipToPass = gameTime.TotalGameTime.Seconds - m_PrevTimeMotherShipPass;

            // Allows the game to exit by GameButton 'back' button or Esc:
            if (this.IsGameOver || InputStateProvider.CurrKeyboardState.IsKeyDown(Keys.Escape))
            {
                handleGameOver();
            }

            isMotherShipNeedToPass(gameTime, timeMotherShipToPass);

            // update game components
            //foreach (Entity gameComponent in m_gameComponents)
            //{
            //    gameComponent.Update(gameTime);
            //}

            for (int i = 0; i < m_gameComponents.Count; i++)
            {
                m_gameComponents[i].Update(gameTime);
            }

            if (isEnemyNextMoveIsFloor())
            {
                this.IsGameOver = true;
            }

            checkIfEnemyNeedMoveGapAndUpdate();

            enemyUpdate(gameTime);

            base.Update(gameTime);

            if (IsChangeEnemiesIntervalBetweenJumps)
            {
                Enemy.speedMovement = Enemy.speedMovement * 0.9f;
                IsChangeEnemiesIntervalBetweenJumps = false;
            }

            if (IsChangeEnemyDirection)
            {
                Enemy.IsEnemyMoveRight = !Enemy.IsEnemyMoveRight;
                IsChangeEnemyDirection = false;
            }
        }

        public void shootStatus(Entity entity)
        {
            //bool isPossibleToShoot = true; // TODO!!!!

            if (InputStateProvider.PrevMouseState != null)
            {
                if (InputStateProvider.CurrentMouseState.LeftButton == ButtonState.Pressed && InputStateProvider.PrevMouseState.Value.LeftButton == ButtonState.Released)
                {
                    if (isPossibleToShoot())
                    {
                        Bullet m_BulletSpaceShip = new Bullet(Bullet.eBulletType.SpaceShip, this); // TODO
                        m_BulletSpaceShip.Texture = Content.Load<Texture2D>(ImagePathProvider.BulletPathImage);
                        m_BulletSpaceShip.Color = Color.Red;
                        m_BulletSpaceShip.Position = new Vector2(entity.Position.X + entity.Texture.Width/2 - 1 , entity.Position.Y);
                        m_gameComponents.Add(m_BulletSpaceShip);
                        m_IsShooting = true;
                    }
                }
            }

        }

        private bool isPossibleToShoot()
        {
            return Bullet.s_NumberOfSpaceShipBullets < 4;
        }

        private void handleGameOver()
        {
            string message = "Game Is Over...";
            string caption = "Game Over";
            System.Windows.Forms.MessageBoxButtons buttons = System.Windows.Forms.MessageBoxButtons.OK;
            System.Windows.Forms.DialogResult result;

            // Displays the MessageBox.
            result = System.Windows.Forms.MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.Exit();
            }
        }

        #region MotherShip

        public Vector2 InitMotherShipPosition()
        {
            return new Vector2((-m_MotherShip.Texture.Width), m_MotherShip.Texture.Height);
        }

        public bool MotherShipPositionOutOfBoundry()
        {
            bool isOutOfBoundry = false;

            if (m_MotherShip.Position.X > this.GraphicsDevice.Viewport.Width)
            {
                isOutOfBoundry = !isOutOfBoundry;
            }

            return isOutOfBoundry;
        }

        private void isMotherShipNeedToPass(GameTime gameTime, int timeMotherShipToPass)
        {
            bool isNeedToPass = false;

            if (!MotherShipNeedToPass)
            {
                m_TimeToPass += gameTime.ElapsedGameTime.TotalSeconds;
                if (timeMotherShipToPass > 0 && (timeMotherShipToPass % m_TimeMotherShipPass) == 0)
                {
                    m_PrevTimeMotherShipPass += m_TimeMotherShipPass;
                    m_TimeMotherShipPass = m_RandomTime.Next(k_MinTimeMotherShipToPass, k_MaxTimeMotherShipToPass);

                    isNeedToPass = !isNeedToPass;
                }

                MotherShipNeedToPass = isNeedToPass;
            }
        }

        #endregion


        #region Enemy

        private void loadEnemyContent()
        {
            int i = 0;

            foreach (var enemiesRow in m_EnemiesList)
            {
                foreach (var enemy in enemiesRow)
                {
                    KeyValuePair<string, Color> imageAndColorToLoad = getEnemyImageAndColor(i);
                    enemy.Texture = Content.Load<Texture2D>(imageAndColorToLoad.Key);
                    enemy.Color = imageAndColorToLoad.Value;
                }
                i++;
            }
        }

        private KeyValuePair<string, Color> getEnemyImageAndColor(int i)
        {
            KeyValuePair<string, Color> imageAndColorToLoad;

            if (i == 0)
            {
                imageAndColorToLoad = new KeyValuePair<string, Color>(ImagePathProvider.EnemyiesPathImageDictionary[ImagePathProvider.eEnemyTypes.Enemy1], Color.LightPink);
            }
            else if (i == 1 || i == 2)
            {
                imageAndColorToLoad = new KeyValuePair<string, Color>(ImagePathProvider.EnemyiesPathImageDictionary[ImagePathProvider.eEnemyTypes.Enemy2], Color.LightBlue);
            }
            else
            {
                imageAndColorToLoad = new KeyValuePair<string, Color>(ImagePathProvider.EnemyiesPathImageDictionary[ImagePathProvider.eEnemyTypes.Enemy3], Color.White);
            }

            return imageAndColorToLoad;
        }

        private void enemyUpdate(GameTime gameTime)
        {
            foreach (var enemiesRow in m_EnemiesList)
            {
                foreach (var enemy in enemiesRow)
                {
                    enemy.Update(gameTime);
                }
            }
        }

        private void checkIfEnemyNeedMoveGapAndUpdate()
        {
            if (IsCanEnemyMatrixMoveRegular)
            {
                isEnemyNextMoveIsWallAndUpdateGap();
                if (!IsCanEnemyMatrixMoveRegular)
                {
                    foreach (var enemiesRow in m_EnemiesList)
                    {
                        foreach (var enemy in enemiesRow)
                        {
                            (enemy as Enemy).EnemyMovementStatus = Enemy.eEnemyMovementOptions.MoveGap;
                        }
                    }
                }
            }
        }

        private bool isEnemyNextMoveIsFloor()
        {
            bool isNextMoveIsFloor = false;
            float nextEnemyPosition;

            foreach (var enemiesRow in m_EnemiesList)
            {
                foreach (var enemy in enemiesRow)
                {
                    nextEnemyPosition = enemy.Position.Y; // + (enemy.TextureEnemy.Height / 2);
                    if (nextEnemyPosition >= this.GraphicsDevice.Viewport.Height - enemy.Texture.Height)
                    {
                        isNextMoveIsFloor = true;
                    }
                }
            }

            return isNextMoveIsFloor;
        }

        private bool isEnemyNextMoveIsWallAndUpdateGap()
        {
            bool isNextMoveIsWall = false;
            float nextEnemyPosition;

            if (Enemy.IsEnemyMoveRight)
            {
                // isNextMoveIsWall = checkConditionOnEveryEnemy(isEnemyNextRightMoveIsWallAndUpdateGap);
                foreach (var enemiesRow in m_EnemiesList)
                {
                    foreach (var enemy in enemiesRow)
                    {
                        nextEnemyPosition = enemy.Position.X + (enemy.Texture.Width / 2);
                        if (nextEnemyPosition > this.GraphicsDevice.Viewport.Width - enemy.Texture.Width)
                        {
                            isNextMoveIsWall = true;
                            (enemy as Enemy).EnemyMovementStatus = Enemy.eEnemyMovementOptions.MoveGap;
                            this.GapToWall = this.GraphicsDevice.Viewport.Width - enemy.Position.X - enemy.Texture.Width;
                            IsCanEnemyMatrixMoveRegular = false;
                        }
                    }
                }
            }
            else
            {
                //isNextMoveIsWall = checkConditionOnEveryEnemy(isEnemyNextLeftMoveIsWallAndUpdateGap);
                foreach (var enemiesRow in m_EnemiesList)
                {
                    foreach (var enemy in enemiesRow)
                    {
                        nextEnemyPosition = enemy.Position.X - (enemy.Texture.Width / 2);
                        if (nextEnemyPosition < 0)
                        {
                            isNextMoveIsWall = true;
                            this.GapToWall = -enemy.Position.X;
                            IsCanEnemyMatrixMoveRegular = false;
                        }
                    }
                }
            }

            return isNextMoveIsWall;
        }

        #endregion

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(m_TextureBackground, m_PositionBackground, m_TintBackground); // tinting with alpha channel
                                                                                           //spriteBatch.Draw(m_Enemy.TextureEnemy, m_Enemy.Position, Color.LightPink); // purple ship
                                                                                           ///spriteBatch.Draw(m_TextureShip, m_PositionShip, Color.White); //no tinting

            //foreach (Entity gameComponent in m_gameComponents)
            //{
            //    gameComponent.Draw(gameTime, spriteBatch, gameComponent);
            //}

            for (int i = 0; i < m_gameComponents.Count; i++)
            {
                m_gameComponents[i].Draw(gameTime, spriteBatch, m_gameComponents[i]);
            }

            foreach (var enemiesRow in m_EnemiesList)
            {
                foreach (var enemy in enemiesRow)
                {
                    enemy.Draw(gameTime, spriteBatch, enemy);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
