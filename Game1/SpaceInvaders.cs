using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Game1
{
    public class SpaceInvaders : Game
    {
        private EnemyCollection enemiesCollection;

        public bool IsChangeEnemyDirection { get; set; }
        public bool IsChangeEnemiesIntervalBetweenJumps { get; set; }
        public bool IsCanEnemyMatrixMoveRegular { get; set; }

        private bool m_IsXEnemiesDead;


        public bool IsGameOver { get; set; }
        Random m_RandomTime = new Random();

        GraphicsDeviceManager graphics;
        public SpriteBatch SpriteBatch { get; set; }

        private const float spaceBetweenSpaceship = 0.6f;
        private const int initEnemiesHieghtFactor = 3;
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

        private int m_Score;
        private int m_NumberOfSouls = 3;


        public SpaceInvaders()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Components.Add(new Background(this));
            
            enemiesCollection = new EnemyCollection(this);
            for (int i = 0; i < m_EnemyNumOfRows * m_EnemyNumOfColumns; i++)
            {
                enemiesCollection.Add(new Enemy(this));
            }

            Components.Add(enemiesCollection);

            Enemy.IsEnemyMoveRight = true;
            Enemy.speedMovement = 0.5f;
            m_TimeEnemyToShoot = m_RandomTime.Next(1, k_MaxTimeForEnemyToShoot);

            m_Spaceship = new Spaceship(this);
            m_Spaceship.Direction = 1f;

            Components.Add(m_Spaceship);

            m_MotherShip = new MotherShip(this);
            MotherShipNeedToPass = false;
            MotherShip.speedMovement = 120f;
            m_TimeMotherShipPass = m_RandomTime.Next(1, k_MinTimeMotherShipToPass);

            Components.Add(m_MotherShip);

            this.IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            base.LoadContent();

            InitPositions();
        }

        private void InitPositions()
        {
            initSpaceShipPosition();

            initEnemiesPosition();

            m_MotherShip.Position = InitMotherShipPosition();

        }

        private void initEnemiesPosition()
        {
            float x, y;
            
            int i = 0;
            x = Enemy.Width * spaceBetweenSpaceship;
            y = initEnemiesHieghtFactor * Enemy.Height;
            foreach (var enemy in enemiesCollection)
            {
                enemy.Position = new Vector2(x, y);

                x += enemy.Texture.Width + enemy.Texture.Width * spaceBetweenSpaceship;

                i++;
                if (i % m_EnemyNumOfColumns == 0)
                {
                    y += enemy.Texture.Height + enemy.Texture.Height * spaceBetweenSpaceship;
                    x = enemy.Texture.Width * spaceBetweenSpaceship;
                }
            }
        }

        private void initSpaceShipPosition()
        {
            // 1. init the ship position
            // Get the bottom and center:
            float x = 0;//(float)GraphicsDevice.Viewport.Width;
            float y = (float)GraphicsDevice.Viewport.Height;

            // Offset:
            x -= m_Spaceship.Texture.Width / 2;
            y -= m_Spaceship.Texture.Height / 2;

            // Put it a little bit higher:
            y -= 30;

            m_Spaceship.Position = new Vector2(x, y);
        }

        protected override void Initialize()
        {
            ImagePathProvider.InitializeImagesPath();
            //ImagePathProvider.InitializeImagesPathStarWars();
            IsCanEnemyMatrixMoveRegular = true;
            base.Initialize();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // get the current input devices state:
            int timeMotherShipToPass = gameTime.TotalGameTime.Seconds - m_PrevTimeMotherShipPass;
            int timeEnemyToShoot = gameTime.TotalGameTime.Seconds - m_PrevTimeEnemyShot;

            checkScore();
            checkIfAllEnemiesDead();
            checkIfEnemiesIntersectsSpaceship();

            // Allows the game to exit by GameButton 'back' button or Esc:
            if (this.IsGameOver || InputStateProvider.CurrKeyboardState.IsKeyDown(Keys.Escape) || isEnemyNextMoveIsFloor())
            {
                handleGameOver();
                return;
            }

            isMotherShipNeedToPass(gameTime, timeMotherShipToPass);
            checkIfEnemyTimeToShoot(gameTime, timeEnemyToShoot);

            checkIfComponentsHitByBullet();

            checkIfEnemyNeedMoveGapAndUpdate();

            base.Update(gameTime);

            if (IsChangeEnemiesIntervalBetweenJumps)
            {
                Enemy.speedMovement = Enemy.speedMovement * 0.91f;
                IsChangeEnemiesIntervalBetweenJumps = false;
            }

            if (m_IsXEnemiesDead)
            {
                Enemy.speedMovement = Enemy.speedMovement * 0.94f;
                m_IsXEnemiesDead = false;
            }

            if (IsChangeEnemyDirection)
            {
                Enemy.IsEnemyMoveRight = !Enemy.IsEnemyMoveRight;
                IsChangeEnemyDirection = false;
            }
        }

        private void checkIfComponentsHitByBullet()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i] is Ivulnerable &&  (!(Components[i] is EnemyCollection)))
                {
                    //if (Components[i] is Bullet)
                    //{
                    //    checkIfBulletHitBullet(Components[i] as Sprite);
                    //}
                    checkIfBulletHitSprite(Components[i] as Sprite);
                }
            }
        }

        private void checkScore()
        {
            if (m_Score < 0)
            {
                m_Score = 0;
                // m_NumberOfSouls--;
            }
            if (m_NumberOfSouls < 0)
            {
                IsGameOver = true;
            }
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

        private Rectangle getRectangleFromSprite(Sprite sprite)
        {
            return new Rectangle((int)sprite.Position.X, (int)sprite.Position.Y, sprite.Texture.Width, sprite.Texture.Height);
        }

        public void checkIfBulletHitSprite(Sprite sprite)
        {
            //Bullet.eBulletType spriteBulletType = (Bullet.eBulletType)Enum.Parse(typeof(Bullet.eBulletType), sprite.ToString());
            Bullet.eBulletType spriteBulletType;
            bool isParse = Enum.TryParse(sprite.ToString(), out spriteBulletType);

            if (sprite.Visible)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    if (Components[i] is Bullet && isParse && (Components[i] as Bullet).BullletType != spriteBulletType)
                    {
                        Bullet currentBullet = (Components[i] as Bullet);
                        Rectangle bulletRect = getRectangleFromSprite(currentBullet);
                        Rectangle spriteRect = getRectangleFromSprite(sprite);

                        if (bulletRect.Intersects(spriteRect))
                        {
                            (sprite as Ivulnerable).IsHitted = true;
                            doWhenHitBulletHitSprite(spriteBulletType, sprite);
                            currentBullet.Dispose();
                            Components.Remove(currentBullet);
                        }
                    }
                }
            }
        }

        public void checkIfBulletHitBullet(Sprite sprite)
        {
            Bullet.eBulletType spriteBulletType;
            bool isParse = Enum.TryParse(sprite.ToString(), out spriteBulletType);

            if (sprite.Visible)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    if (Components[i] is Bullet && sprite is Bullet && !Components[i].Equals(sprite))
                    {
                        Bullet currentBullet = (Components[i] as Bullet);
                        Rectangle bulletRect = getRectangleFromSprite(currentBullet);
                        Rectangle spriteRect = getRectangleFromSprite(sprite);

                        if (bulletRect.Intersects(spriteRect))
                        {
                            (sprite as Ivulnerable).IsHitted = true;
                            //doWhenHitBulletHitSprite(spriteBulletType, sprite);
                            currentBullet.Dispose();
                            Components.Remove(currentBullet);
                        }
                    }
                }
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
                    m_MotherShip.Visible = true;
                }

                MotherShipNeedToPass = isNeedToPass;
            }
        }

        #endregion


        #region Enemy

        private void checkIfAllEnemiesDead()
        {
            if (m_NumberOfHittedEnemies == (m_EnemyNumOfRows * m_EnemyNumOfColumns))
            {
                IsGameOver = true;
            }
        }

        public KeyValuePair<string, Color> getEnemyImageAndColorAndSetType(int i, Enemy enemy)
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

        private void checkIfEnemyTimeToShoot(GameTime gameTime, int timeEnemyToShoot)
        {
            KeyValuePair<int, int> randomRowAndCol = getRandomRowAndCol();

            Sprite enemy = enemiesCollection[randomRowAndCol.Key * randomRowAndCol.Value];

            if (enemy.Visible)
            {
                m_TimeToShoot += gameTime.ElapsedGameTime.TotalSeconds;
                if (timeEnemyToShoot > 0 && (timeEnemyToShoot % m_TimeEnemyToShoot) == 0)
                {
                    m_PrevTimeEnemyShot += m_TimeEnemyToShoot;
                    m_TimeEnemyToShoot = m_RandomTime.Next(k_MinTimeForEnemyToShoot, k_MaxTimeForEnemyToShoot);

                    (enemy as Enemy).Shoot();
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

            } while (!enemiesCollection[randomRowAndCol.Key * randomRowAndCol.Value].Visible);

            return randomRowAndCol;
        }

        private void doWhenHitBulletHitSprite(Bullet.eBulletType sprtieBulletType, Sprite sprite)
        {
            switch (sprtieBulletType)
            {
                case Bullet.eBulletType.Spaceship:
                    doWhenhitSpaceship();
                    break;
                case Bullet.eBulletType.Enemy:
                    doWhenHitEnemy(sprite as EnemyBase);
                    break;
                default:
                    break;
            }
        }

        private void doWhenHitEnemy(EnemyBase enemy)
        {
            enemy.Visible = false;

            m_Score += (int)enemy.Type;
            if (enemy is Enemy)
            {
                m_NumberOfHittedEnemies++;
                if(m_NumberOfHittedEnemies % 3 == 0)
                {
                    m_IsXEnemiesDead = true;
                }

            }
        }

        private void doWhenhitSpaceship()
        {
            initSpaceShipPosition();
            m_NumberOfSouls--;
            m_Score -= 1900;
        }

        private void checkIfEnemyNeedMoveGapAndUpdate()
        {
            if (IsCanEnemyMatrixMoveRegular)
            {
                isEnemyNextMoveIsWallAndUpdateGap();
                if (!IsCanEnemyMatrixMoveRegular)
                {
                    foreach (var enemy in enemiesCollection)
                    {
                        enemy.EnemyMovementStatus = Enemy.eEnemyMovementOptions.MoveGap;
                    }
                }
            }
        }

        private bool isEnemyNextMoveIsFloor()
        {
            bool isNextMoveIsFloor = false;
            float nextEnemyYPosition;

            foreach (var enemy in enemiesCollection)
            {
                nextEnemyYPosition = enemy.Position.Y; // + (enemy.TextureEnemy.Height / 2);
                if ((nextEnemyYPosition >= this.GraphicsDevice.Viewport.Height - enemy.Texture.Height))
                {
                    isNextMoveIsFloor = true;
                }
            }

            return isNextMoveIsFloor;
        }

        private void checkIfEnemiesIntersectsSpaceship()
        {
            foreach (EnemyComponent enemy in enemiesCollection)
            {
                if (enemy.Visible)
                {
                    if (IsEnemyNextMoveIsSpaceShip((enemy as Enemy)))
                    {
                        IsGameOver = true;
                    }
                }
            }
        }

        private bool IsEnemyNextMoveIsSpaceShip(Enemy enemy)
        {
            Rectangle enemyRect = getRectangleFromSprite(enemy);
            Rectangle spaceshipRect = getRectangleFromSprite(m_Spaceship);

            return enemyRect.Intersects(spaceshipRect);
        }

        private bool isEnemyNextMoveIsWallAndUpdateGap()
        {
            bool isNextMoveIsWall = false;
            float nextEnemyPosition;

            if (Enemy.IsEnemyMoveRight)
            {
                // isNextMoveIsWall = checkConditionOnEveryEnemy(isEnemyNextRightMoveIsWallAndUpdateGap);
                foreach (EnemyComponent enemy in enemiesCollection)
                {
                    if (enemy.Visible)
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
                foreach (var enemy in enemiesCollection)
                {
                    if (enemy.Visible)
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

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin();

            base.Draw(gameTime);

            SpriteBatch.End();
        }
    }
}
