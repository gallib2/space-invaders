using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Infrastructure;

namespace SpaceInvaders
{
    public class SpaceInvaders : Base2DGame
    {
        private const float v_SpaceBetweenSpaceship = 0.6f;
        private const int v_InitEnemiesHieghtFactor = 3;
        private EnemyCollection m_EnemiesCollection;
        private Spaceship m_Spaceship;
        private MotherShip m_MotherShip;
        private int m_NumberOfHitEnemies;
        private int m_Score;
        private int m_NumberOfSouls = 3;

        public static Random RandomNumber { get; set; }

        public bool IsGameOver { get; set; }

        public SpaceInvaders()
        {
            Content.RootDirectory = "Content";
            RandomNumber = new Random();

            Components.Add(new Background(this));

            m_EnemiesCollection = new EnemyCollection(this);
            for (int i = 0; i < EnemyCollection.NumOfRows * EnemyCollection.NumOfColumns; i++)
            {
                m_EnemiesCollection.Add(new Enemy(this));
            }

            Components.Add(m_EnemiesCollection);

            m_Spaceship = new Spaceship(this);
            Components.Add(m_Spaceship);

            m_MotherShip = new MotherShip(this);
            Components.Add(m_MotherShip);

            this.IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            initPositions();
        }

        private void initPositions()
        {
            initSpaceShipPosition();

            initEnemiesPosition();

            initMotherShipPosition();
        }

        private void initEnemiesPosition()
        {
            float x, y;

            int i = 0;
            x = Enemy.Width * v_SpaceBetweenSpaceship;
            y = v_InitEnemiesHieghtFactor * Enemy.Height;
            foreach (var enemy in m_EnemiesCollection)
            {
                enemy.Position = new Vector2(x, y);

                x += enemy.Texture.Width + (enemy.Texture.Width * v_SpaceBetweenSpaceship);

                i++;
                if (i % EnemyCollection.NumOfColumns == 0)
                {
                    y += enemy.Texture.Height + (enemy.Texture.Height * v_SpaceBetweenSpaceship);
                    x = enemy.Texture.Width * v_SpaceBetweenSpaceship;
                }
            }
        }

        private void initSpaceShipPosition()
        {
            // 1. init the ship position
            // Get the bottom and center:
            float x = 0;
            float y = (float)GraphicsDevice.Viewport.Height;

            // Offset:
            x -= m_Spaceship.Texture.Width / 2;
            y -= m_Spaceship.Texture.Height / 2;

            // Put it a little bit higher:
            y -= 30;

            m_Spaceship.Position = new Vector2(x, y);
        }

        private void initMotherShipPosition()
        {
            m_MotherShip.Position = m_MotherShip.InitPosition;
        }

        protected override void Initialize()
        {
            ImagePathProvider.InitializeImagesPath();

            // this is just for more fun:) 

             ////ImagePathProvider.InitializeImagesPathStarWars(); 

            base.Initialize();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime i_GameTime)
        {
            checkScore();
            checkIfAllEnemiesDead();
            checkIfEnemiesIntersectsSpaceship();

            if (this.IsGameOver || InputStateProvider.CurrKeyboardState.IsKeyDown(Keys.Escape) || isEnemyNextMoveIsFloor())
            {
                handleGameOver();
                return;
            }

            checkIfComponentsHitByBullet();

            base.Update(i_GameTime);
        }

        private void checkIfComponentsHitByBullet()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i] is Ivulnerable && (!(Components[i] is EnemyCollection)))
                {
                    CheckIfBulletHitSprite(Components[i] as Sprite);
                }
            }

            checkIfBulletHitByBullet();
        }

        private void checkIfBulletHitByBullet()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i] is Bullet && (!(Components[i] is EnemyCollection)))
                {
                    CheckIfBulletHitBullet(Components[i] as Sprite);
                }
            }
        }

        private void checkScore()
        {
            if (m_Score < 0)
            {
                m_Score = 0;
            }

            if (m_NumberOfSouls <= 0)
            {
                IsGameOver = true;
            }
        }

        private void handleGameOver()
        {
            string message = $"Game Over! Score: {m_Score} ";
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

        private Rectangle getRectangleFromSprite(Sprite o_Sprite)
        {
            return new Rectangle((int)o_Sprite.Position.X, (int)o_Sprite.Position.Y, o_Sprite.Texture.Width, o_Sprite.Texture.Height);
        }

        public void CheckIfBulletHitSprite(Sprite io_Sprite)
        {
            eBulletType spriteBulletType;
            bool isParse = Enum.TryParse(io_Sprite.ToString(), out spriteBulletType);

            if (io_Sprite.Visible)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    if (Components[i] is Bullet && isParse && (Components[i] as Bullet).BullletType != spriteBulletType)
                    {
                        checkIfBulletIntersectsWithSpriteAndHandle(Components[i] as Bullet, io_Sprite, spriteBulletType);
                    }
                }
            }
        }

        public void CheckIfBulletHitBullet(Sprite sprite)
        {
            eBulletType spriteBulletType;
            bool isParse = Enum.TryParse(sprite.ToString(), out spriteBulletType);

            if (sprite.Visible)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    if (Components[i] is Bullet && sprite is Bullet && !Components[i].Equals(sprite))
                    {
                        checkIfBulletIntersectsWithSpriteAndHandle(Components[i] as Bullet, sprite, spriteBulletType);
                    }
                }
            }
        }

        private void checkIfBulletIntersectsWithSpriteAndHandle(Bullet i_CurrentBullet, Sprite io_Sprite, eBulletType o_SpriteBulletType)
        {
            Rectangle bulletRect = getRectangleFromSprite(i_CurrentBullet);
            Rectangle spriteRect = getRectangleFromSprite(io_Sprite);

            if (bulletRect.Intersects(spriteRect))
            {
                doWhenHitBulletHitSprite(o_SpriteBulletType, io_Sprite);
                i_CurrentBullet.Dispose();
                Components.Remove(i_CurrentBullet);
            }
        }

        private void doWhenHitBulletHitSprite(eBulletType o_SprtieBulletType, Sprite io_Sprite)
        {
            if (io_Sprite is Bullet)
            {
                io_Sprite.Dispose();
                Components.Remove(io_Sprite);
            }
            else if (o_SprtieBulletType == eBulletType.Spaceship)
            {
                doWhenhitSpaceship();
            }
            else if (o_SprtieBulletType == eBulletType.Enemy)
            {
                doWhenHitEnemy(io_Sprite as EnemyBase);
            }
        }

        private void doWhenhitSpaceship()
        {
            initSpaceShipPosition();
            m_NumberOfSouls--;
            m_Score -= 1900;
        }

        #region Enemy

        private void checkIfAllEnemiesDead()
        {
            if (m_NumberOfHitEnemies == (EnemyCollection.NumOfRows * EnemyCollection.NumOfColumns))
            {
                IsGameOver = true;
            }
        }

        private void doWhenHitEnemy(EnemyBase io_Enemy)
        {
            io_Enemy.Visible = false;

            m_Score += (int)io_Enemy.Type;
            if (io_Enemy is Enemy)
            {
                m_NumberOfHitEnemies++;
                if (m_NumberOfHitEnemies % 3 == 0)
                {
                    EnemyComponent.IsXEnemiesDead = true;
                }
            }
        }

        private bool isEnemyNextMoveIsFloor()
        {
            bool isNextMoveIsFloor = false;
            float nextEnemyYPosition;

            foreach (var enemy in m_EnemiesCollection)
            {
                nextEnemyYPosition = enemy.Position.Y; // + (enemy.TextureEnemy.Height / 2);
                if (nextEnemyYPosition >= this.GraphicsDevice.Viewport.Height - enemy.Texture.Height)
                {
                    isNextMoveIsFloor = true;
                }
            }

            return isNextMoveIsFloor;
        }

        private void checkIfEnemiesIntersectsSpaceship()
        {
            foreach (EnemyComponent enemy in m_EnemiesCollection)
            {
                if (enemy.Visible)
                {
                    if (IsEnemyNextMoveIsSpaceShip(enemy as Enemy))
                    {
                        IsGameOver = true;
                    }
                }
            }
        }

        private bool IsEnemyNextMoveIsSpaceShip(Enemy i_Enemy)
        {
            Rectangle enemyRect = getRectangleFromSprite(i_Enemy);
            Rectangle spaceshipRect = getRectangleFromSprite(m_Spaceship);

            return enemyRect.Intersects(spaceshipRect);
        }

        #endregion
    }
}
