using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public delegate void DelegateActionToCommit(Enemy enemy);
        public delegate bool DelegateCheckToCommit(Enemy enemy);

        public bool IsGameOver { get; set; }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Enemy m_Enemy = new Enemy();
        List<List<Enemy>> m_EnemiesList = new List<List<Enemy>>();
        public float GapToWall { get; set; }


        Spaceship m_Spaceship = new Spaceship();
        bool m_IsShooting = false;
        Bullet m_BulletSpaceShip = new Bullet();

        Texture2D m_TextureBackground;
        Vector2 m_PositionBackground;
        Color m_TintBackground = Color.White;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Enemy.EnemyMovementStatus = Enemy.eEnemyMovementOptions.MoveRegular;
            Enemy.IsEnemyMoveRight = true;
            Enemy.speedMovement = 0.25f;

            m_Spaceship.Direction = 1f;

            this.IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            m_TextureBackground = Content.Load<Texture2D>(@"Sprites\BG_Space01_1024x768");
            m_Enemy.Texture = Content.Load<Texture2D>(ImagePathProvider.EnemyiesPathImageDictionary[ImagePathProvider.eEnemyTypes.Enemy1]);
            /// m_TextureShip = Content.Load<Texture2D>(@"Sprites\Ship01_32x32");

            m_BulletSpaceShip.Texture = Content.Load<Texture2D>(ImagePathProvider.BulletPathImage);
            m_BulletSpaceShip.Color = Color.Red;
            m_Spaceship.Texture = Content.Load<Texture2D>(ImagePathProvider.SpaceShipPathImage);
            loadEnemyContent(5, 9);


            InitPositions();
        }

        private void loadEnemyContent(int numOfRows, int numOfColms)
        {
            for (int i = 0; i < numOfRows; i++)
            {
                m_EnemiesList.Add(new List<Enemy>());
                for (int j = 0; j < numOfColms; j++)
                {
                    m_EnemiesList[i].Add(new Enemy());
                    KeyValuePair<string, Color> imageAndColorToLoad = getEnemyImageAndColor(i);
                    //string imageToLoad = getImageToLoad(i);
                    m_EnemiesList[i][j].Texture = Content.Load<Texture2D>(imageAndColorToLoad.Key);
                    m_EnemiesList[i][j].Color = imageAndColorToLoad.Value;
                }
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

            m_Enemy.InitPosition();

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

            // 3. Init the bg position:
            m_PositionBackground = Vector2.Zero;

            //create an alpah channel for background:
            Vector4 bgTint = Vector4.One;
            bgTint.W = 0.4f; // set the alpha component to 0.2
            m_TintBackground = new Color(bgTint);
        }

        MouseState? m_PrevMouseState;

        private Vector2 GetMousePositionDelta()
        {
            Vector2 retVal = Vector2.Zero;

            MouseState currState = Mouse.GetState();

            if (m_PrevMouseState != null)
            {
                retVal.X = (currState.X - m_PrevMouseState.Value.X);
                retVal.Y = (currState.Y - m_PrevMouseState.Value.Y);
            }

            m_PrevMouseState = currState;

            return retVal;
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
            GamePadState currGamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState currKeyboardState = Keyboard.GetState();

            // Allows the game to exit by GameButton 'back' button or Esc:
            if (this.IsGameOver || currKeyboardState.IsKeyDown(Keys.Escape))
            {
                handleGameOver();
            }

            // move the ship using the GamePad left thumb stick and set viberation according to movement:
            ///m_PositionShip.X += currGamePadState.ThumbSticks.Left.X * 120 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            ///GamePad.SetVibration(PlayerIndex.One, 0, Math.Abs(currGamePadState.ThumbSticks.Left.X));

            shipUpdate(gameTime);

            if (m_IsShooting)
            {
                // if()  get to ciel OR hit enemy => m_IsShooting = false
                m_BulletSpaceShip.Position = new Vector2(m_BulletSpaceShip.Position.X, m_BulletSpaceShip.Position.Y - (150 * (float)gameTime.ElapsedGameTime.TotalSeconds));
            }

            if (isEnemyNextMoveIsWallAndUpdateGap())
            {
                Enemy.EnemyMovementStatus = Enemy.eEnemyMovementOptions.MoveGap;
            }

            enemyUpdate(gameTime);

            base.Update(gameTime);
        }

        private void shipUpdate(GameTime gameTime)
        {
            shootStatus();

            // move the ship using the mouse:
            m_Spaceship.Position = new Vector2((m_Spaceship.Position.X + GetMousePositionDelta().X), m_Spaceship.Position.Y);

            // clam the position between screen boundries:
            m_Spaceship.Position = new Vector2(MathHelper.Clamp(m_Spaceship.Position.X, 0, this.GraphicsDevice.Viewport.Width - m_Spaceship.Texture.Width), m_Spaceship.Position.Y);

            // if we hit the wall, lets change direction:
            if (m_Spaceship.Position.X == 0 || m_Spaceship.Position.X == this.GraphicsDevice.Viewport.Width - m_Spaceship.Texture.Width)
            {
                m_Spaceship.Direction *= -1f;
            }
        }

        private void shootStatus()
        {
            bool isPossibleToShoot = true; // TODO!!!!
            MouseState currMouseState = Mouse.GetState();

            if(m_PrevMouseState != null)
            {
                if (currMouseState.LeftButton == ButtonState.Pressed && m_PrevMouseState.Value.LeftButton == ButtonState.Released)
                {
                    if (isPossibleToShoot)
                    {
                        m_BulletSpaceShip.Position = new Vector2(m_Spaceship.Position.X, m_Spaceship.Position.Y);
                        m_IsShooting = true;
                    }
                }
            }


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

        #region Enemy stuff

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

        private bool isEnemyNextRightMoveIsWallAndUpdateGap(Enemy enemy)
        {
            float nextEnemyPosition = enemy.Position.X + (enemy.Position.X / 2);
            bool isNextMoveIsWall = false;
            if (nextEnemyPosition > this.GraphicsDevice.Viewport.Width - enemy.Texture.Width)
            {
                isNextMoveIsWall = true;
                this.GapToWall = this.GraphicsDevice.Viewport.Width - enemy.Position.X - enemy.Texture.Width;
            }

            return isNextMoveIsWall;
        }

        private bool isEnemyNextLeftMoveIsWallAndUpdateGap(Enemy enemy)
        {
            float nextEnemyPosition = enemy.Position.X - (enemy.Texture.Width / 2);
            bool isNextMoveIsWall = false;
            if (nextEnemyPosition < 0)
            {
                isNextMoveIsWall = true;
                this.GapToWall = -enemy.Position.X;
            }

            return isNextMoveIsWall;
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
                            this.GapToWall = this.GraphicsDevice.Viewport.Width - enemy.Position.X - enemy.Texture.Width;
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
                        }
                    }
                }
            }

            return isNextMoveIsWall;
        }

        double m_TimeToNextBlink = 0;

        private void enemyUpdate(GameTime gameTime)
        {
            m_TimeToNextBlink += gameTime.ElapsedGameTime.TotalSeconds;

            if (m_TimeToNextBlink >= Enemy.speedMovement)
            {
                // m_IsMsgVisible = !m_IsMsgVisible;
                m_TimeToNextBlink -= Enemy.speedMovement;

                switch (Enemy.EnemyMovementStatus)
                {
                    case Enemy.eEnemyMovementOptions.MoveDown:
                        Enemy.Direction *= -1f;
                        Enemy.EnemyMovementStatus = Enemy.eEnemyMovementOptions.MoveRegular;
                        Enemy.speedMovement = Enemy.speedMovement * Enemy.sr_TimePercentBetweenJumps;
                        actionOnEveryEnemy(enemyMoveDown);
                        if (isEnemyNextMoveIsFloor())
                        {
                            this.IsGameOver = true;
                        }
                        break;
                    case Enemy.eEnemyMovementOptions.MoveGap:
                        actionOnEveryEnemy(enemyMoveGap);
                        Enemy.EnemyMovementStatus = Enemy.eEnemyMovementOptions.MoveDown;
                        Enemy.IsEnemyMoveRight = !Enemy.IsEnemyMoveRight;
                        break;
                    case Enemy.eEnemyMovementOptions.MoveRegular:
                        actionOnEveryEnemy(enemyMoveRegular);
                        break;
                    default:
                        break;
                }
            }
        }

        public void enemyMoveGap(Enemy enemy)
        {
            enemy.Position = new Vector2(enemy.Position.X + this.GapToWall, enemy.Position.Y);
        }

        private void enemyMoveDown(Enemy enemy)
        {
            enemy.Position = new Vector2(enemy.Position.X, enemy.Position.Y + enemy.Texture.Height / 2);
        }

        private void enemyMoveRegular(Enemy enemy)
        {
            enemy.Position = new Vector2(enemy.Position.X + (Enemy.Direction) * (enemy.Texture.Width / 2), enemy.Position.Y);
        }

        private void actionOnEveryEnemy(DelegateActionToCommit actionsToCommit)
        {
            foreach (var enemiesRow in m_EnemiesList)
            {
                foreach (var enemy in enemiesRow)
                {
                    actionsToCommit(enemy);
                }
            }
        }

        private bool checkConditionOnEveryEnemy(DelegateCheckToCommit checkToCommit)
        {
            bool answerOfCheckToCommit = false;

            foreach (var enemiesRow in m_EnemiesList)
            {
                foreach (var enemy in enemiesRow)
                {
                    answerOfCheckToCommit = checkToCommit(enemy);
                    if (answerOfCheckToCommit) // if one of them return true then stop to ask
                    {
                        break;
                    }
                }
            }

            return answerOfCheckToCommit;
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

            spriteBatch.Draw(m_Spaceship.Texture, m_Spaceship.Position, Color.White); //no tinting

            spriteBatch.Draw(m_BulletSpaceShip.Texture, m_BulletSpaceShip.Position, m_BulletSpaceShip.Color); //no tinting

            int j = 0;
            for (int i = 0; i < 5; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    spriteBatch.Draw(m_EnemiesList[i][j].Texture, m_EnemiesList[i][j].Position, m_EnemiesList[i][j].Color);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
