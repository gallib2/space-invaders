using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceInvaders
{
    public class Enemy : EnemyComponent, IShootable
    {
        private static int s_Width;
        private static int s_Height;

        public static int Width
        {
            get
            {
                return s_Width;
            }
        }

        public static int Height
        {
            get
            {
                return s_Height;
            }
        }

        public Enemy(Game spaceInvaders) : base(spaceInvaders)
        {
        }

        public void Shoot()
        {
            eBulletType bulletType = eBulletType.Enemy;

            Bullet bullet = new Bullet(bulletType, Game as SpaceInvaders);
            (Game as SpaceInvaders).Components.Add(bullet);
            bullet.Position = new Vector2(Position.X + (Texture.Width / 2) - 1, Position.Y);
        }

        public override void Initialize()
        {
            base.Initialize();
            s_Width = Texture.Width;
            s_Height = Texture.Height;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            s_NumberOfLoadedEnemies++;

            KeyValuePair<string, Color> imageAndColorToLoad = getEnemyImageAndColorAndSetType(s_CurrentRowOfLoadedEnemies, this);
            Texture = (Game as SpaceInvaders).Content.Load<Texture2D>(imageAndColorToLoad.Key);
            Color = imageAndColorToLoad.Value;

            if (s_NumberOfLoadedEnemies % k_NumberOfColumns == 0)
            {
                s_CurrentRowOfLoadedEnemies++;
            }
        }

        public override void Update(GameTime gameTime)
        {
            (Game as SpaceInvaders).CheckIfBulletHitSprite(this);
            m_TimeToNextBlink += gameTime.ElapsedGameTime.TotalSeconds;

            if (m_TimeToNextBlink >= Enemy.speedMovement)
            {
                if (Visible)
                {
                    switch (EnemyMovementStatus)
                    {
                        case eEnemyMovementOptions.MoveDown:
                            Position = new Vector2(Position.X, Position.Y + (Texture.Height / 2));
                            Direction *= -1f;
                            EnemyMovementStatus = eEnemyMovementOptions.MoveRegular;
                            IsChangeEnemyDirection = true;
                            IsChangeEnemiesIntervalBetweenJumps = true;
                            IsCanEnemyMatrixMoveRegular = true;
                            break;
                        case eEnemyMovementOptions.MoveGap:
                            Position = new Vector2(Position.X + GapToWall, Position.Y);
                            EnemyMovementStatus = eEnemyMovementOptions.MoveDown;
                            break;
                        case eEnemyMovementOptions.MoveRegular:
                            Position = new Vector2(Position.X + (Direction * (Texture.Width / 2)), Position.Y);
                            break;
                    }

                    m_TimeToNextBlink -= Enemy.speedMovement;
                }
            }
        }

        public override void Add(EnemyComponent c)
        {
            // TODO - cant add to a leaf
        }

        public override void Remove(EnemyComponent c)
        {
            // TODO - cant remove from a leaf
        }

        public override string ToString()
        {
            return "Enemy";
        }
    }
}
