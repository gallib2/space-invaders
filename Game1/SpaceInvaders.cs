﻿using Microsoft.Xna.Framework;
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

        public List<Entity> GameComponents { get; set; }

        List<List<Entity>> m_EnemiesList = new List<List<Entity>>();
        public float GapToWall { get; set; }
        int m_EnemyNumOfRows = 5;
        int m_EnemyNumOfColumns = 9;
        private int m_NumberOfHittedEnemies;
        public bool EnemyNeedToShoot { get; set; }
        double m_TimeToShoot = 0;
        int m_PrevTimeEnemyShot;
        private int m_TimeEnemyToShoot;
        private const int k_MinTimeForEnemyToShoot = 10;
        private const int k_MaxTimeForEnemyToShoot = 12;

        Spaceship m_Spaceship;

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

        private int m_Score;
        private int m_NumberOfSouls = 3;


        public SpaceInvaders()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            GameComponents = new List<Entity>();

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
            m_TimeEnemyToShoot = m_RandomTime.Next(1, k_MaxTimeForEnemyToShoot);

            m_Spaceship = new Spaceship(this);
            m_Spaceship.Direction = 1f;

            GameComponents.Add(m_Spaceship);

            m_MotherShip = new MotherShip(this);
            MotherShipNeedToPass = false;
            MotherShip.speedMovement = 120f;
            m_TimeMotherShipPass = m_RandomTime.Next(1, k_MinTimeMotherShipToPass);

            GameComponents.Add(m_MotherShip);

            this.IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            m_TextureBackground = Content.Load<Texture2D>(ImagePathProvider.BackgroundPathImage);

            for (int i = 0; i < GameComponents.Count; i++)
            {
                GameComponents[i].LoadContent(Content);
            }

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
            int timeEnemyToShoot = gameTime.TotalGameTime.Seconds - m_PrevTimeEnemyShot;

            checkScore();
            checkIfAllEnemiesDead();

            // Allows the game to exit by GameButton 'back' button or Esc:
            if (this.IsGameOver || InputStateProvider.CurrKeyboardState.IsKeyDown(Keys.Escape) || isEnemyNextMoveIsFloor())
            {
                handleGameOver();
            }

            isMotherShipNeedToPass(gameTime, timeMotherShipToPass);
            checkIfEnemyTimeToShoot(gameTime, timeEnemyToShoot);

            for (int i = 0; i < GameComponents.Count; i++)
            {
                if (GameComponents[i] is Ivulnerable && GameComponents[i] is EnemyBase)
                {
                    checkIfBulletHitEnemy(GameComponents[i] as EnemyBase);
                }

                GameComponents[i].Update(gameTime);
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

        private void checkIfAllEnemiesDead()
        {
            if(m_NumberOfHittedEnemies == m_EnemiesList.Count)
            {
                IsGameOver = true;
            }
        }

        private void checkScore()
        {
            if (m_Score < 0)
            {
                // m_NumberOfSouls--;
            }
            if (m_NumberOfSouls < 0)
            {
                IsGameOver = true;
            }
        }

        private Bullet.eBulletType getBulletType(Entity entity)
        {
            Bullet.eBulletType bulletType;

            if (entity is Enemy)
            {
                bulletType = Bullet.eBulletType.Enemy;
            }
            else
            {
                bulletType = Bullet.eBulletType.SpaceShip;
            }

            return bulletType;

        }

        private bool isPossibleToShoot(Bullet.eBulletType bulletType)
        {
            bool possibleToShoot = true;

            if (bulletType == Bullet.eBulletType.SpaceShip)
            {
                possibleToShoot = Bullet.s_NumberOfSpaceShipBullets < 4;
            }

            return possibleToShoot;
        }

        private void handleGameOver()
        {
            string message = $"Game Is Over..Score: {m_Score} ";
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
                    m_MotherShip.IsVisible = true;
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
                    KeyValuePair<string, Color> imageAndColorToLoad = getEnemyImageAndColorAndSetType(i, (enemy as Enemy));
                    enemy.Texture = Content.Load<Texture2D>(imageAndColorToLoad.Key);
                    enemy.Color = imageAndColorToLoad.Value;
                }
                i++;
            }
        }

        private KeyValuePair<string, Color> getEnemyImageAndColorAndSetType(int i, Enemy enemy)
        {
            KeyValuePair<string, Color> imageAndColorToLoad;

            if (i == 0)
            {
                enemy.Type = EnemyBase.eEnemyTypes.Enemy1;
                imageAndColorToLoad = new KeyValuePair<string, Color>(ImagePathProvider.EnemyiesPathImageDictionary[enemy.Type], Color.LightPink);
            }
            else if (i == 1 || i == 2)
            {
                enemy.Type = EnemyBase.eEnemyTypes.Enemy2;
                imageAndColorToLoad = new KeyValuePair<string, Color>(ImagePathProvider.EnemyiesPathImageDictionary[enemy.Type], Color.LightBlue);
            }
            else
            {
                enemy.Type = EnemyBase.eEnemyTypes.Enemy3;
                imageAndColorToLoad = new KeyValuePair<string, Color>(ImagePathProvider.EnemyiesPathImageDictionary[enemy.Type], Color.White);
            }

            return imageAndColorToLoad;
        }

        private void enemyUpdate(GameTime gameTime)
        {
            foreach (var enemiesRow in m_EnemiesList)
            {
                foreach (var enemy in enemiesRow)
                {
                    checkIfBulletHitEnemy(enemy as Enemy);
                    //if (m_NumberOfHittedEnemies % 3 == 0)
                    //{
                    //    Enemy.speedMovement = Enemy.speedMovement * 0.6f;
                    //}
                    //checkIfEnemyTimeToShoot(enemy as Enemy, gameTime);
                    enemy.Update(gameTime);
                }
            }
        }

        private void checkIfEnemyTimeToShoot(GameTime gameTime, int timeEnemyToShoot)
        {
            //int randomRow = m_RandomTime.Next(0, 5);
            //int randomCol = m_RandomTime.Next(0, 8);

            KeyValuePair<int, int> randomRowAndCol = getRandomRowAndCol();

            Entity enemy = m_EnemiesList[randomRowAndCol.Key][randomRowAndCol.Value];
            
            if (enemy.IsVisible)
            {
                //bool isNeedToShoot = false;
               // int timeEnemyToShoot = gameTime.TotalGameTime.Seconds - m_PrevTimeEnemyShot;

                //if (!EnemyNeedToShoot)
               // {
                    m_TimeToShoot += gameTime.ElapsedGameTime.TotalSeconds;
                    if (timeEnemyToShoot > 0 && (timeEnemyToShoot % m_TimeEnemyToShoot) == 0)
                    {
                        m_PrevTimeEnemyShot += m_TimeEnemyToShoot;
                        m_TimeEnemyToShoot = m_RandomTime.Next(k_MinTimeForEnemyToShoot, k_MaxTimeForEnemyToShoot);

                        //isNeedToShoot = !isNeedToShoot;
                        (enemy as Enemy).Shoot();
                   // }

                   // EnemyNeedToShoot = isNeedToShoot;
                }
            }
        }

        private KeyValuePair<int, int> getRandomRowAndCol()
        {
            KeyValuePair<int, int> randomRowAndCol;

            do
            {
                int row = m_RandomTime.Next(0, m_EnemyNumOfRows);
                int col = m_RandomTime.Next(0, m_EnemyNumOfColumns);
                randomRowAndCol = new KeyValuePair<int, int>(row, col);

            } while (!m_EnemiesList[randomRowAndCol.Key][randomRowAndCol.Value].IsVisible);

            return randomRowAndCol;
        }

        private void checkIfBulletHitEnemy(EnemyBase enemy)
        {
            if (enemy.IsVisible)
            {
                for (int i = 0; i < GameComponents.Count; i++)
                {
                    if (GameComponents[i] is Bullet && (GameComponents[i] as Bullet).BullletType == Bullet.eBulletType.SpaceShip)
                    {
                        Bullet currentBullet = (GameComponents[i] as Bullet);
                        if ((currentBullet.Position.X > enemy.Position.X && currentBullet.Position.X < enemy.Position.X + enemy.Texture.Width) &&
                            (currentBullet.Position.Y > enemy.Position.Y && currentBullet.Position.Y < enemy.Position.Y + enemy.Texture.Height))
                        {
                            (enemy as EnemyBase).IsHitted = true;
                            (enemy as EnemyBase).IsVisible = false;

                            m_Score += (int)enemy.Type;
                            m_NumberOfHittedEnemies++;
                            currentBullet.Dispose();
                            GameComponents.Remove(currentBullet);
                        }
                    }
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

            for (int i = 0; i < GameComponents.Count; i++)
            {
                if (GameComponents[i].IsVisible)
                {
                    GameComponents[i].Draw(gameTime, spriteBatch, GameComponents[i]);
                }
            }

            foreach (var enemiesRow in m_EnemiesList)
            {
                foreach (var enemy in enemiesRow)
                {
                    if ((enemy as Enemy).IsVisible)
                    {
                        enemy.Draw(gameTime, spriteBatch, enemy);
                    }
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
