using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    public class Enemy : EnemyComponent, IShootable
    {
        //public eEnemyMovementOptions EnemyMovementStatus { get; set; }
        //public static bool IsEnemyMoveRight { get; set; }
        //public static float speedMovement { get; set; }
        //public readonly float sr_TimePercentBetweenJumps = 0.9f;
        //private double m_TimeToNextBlink;
        //public float Direction { get; set; }

        

        public Enemy(Game spaceInvaders) : base(spaceInvaders)
        {
            //EnemyMovementStatus = eEnemyMovementOptions.MoveRegular;
            //Direction = 1f;
            //IsVisible = true;
        }


        //public enum eEnemyMovementOptions
        //{
        //    MoveDown,
        //    MoveGap,
        //    MoveRegular
        //}

        public void Shoot()
        {
            Bullet.eBulletType bulletType = Bullet.eBulletType.Enemy;

            Bullet bullet = new Bullet(bulletType, Game as SpaceInvaders);
            (Game as SpaceInvaders).Components.Add(bullet);
            bullet.Position = new Vector2(Position.X + Texture.Width / 2 - 1, Position.Y);
        }

        public override void Initialize()
        {
            base.Initialize();
            //LoadContent();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            m_NumberOfLoadedEnemies++;

            KeyValuePair<string, Color> imageAndColorToLoad = (Game as SpaceInvaders).getEnemyImageAndColorAndSetType(m_CurrentRowOfLoadedEnemies, this);
            Texture = (Game as SpaceInvaders).Content.Load<Texture2D>(imageAndColorToLoad.Key);
            Color = imageAndColorToLoad.Value;

            if(m_NumberOfLoadedEnemies % m_NumberOfColumns == 0)
            {
                m_CurrentRowOfLoadedEnemies++;
            }
        }

        public override void Update(GameTime gameTime)
        {
            (Game as SpaceInvaders).checkIfBulletHitSprite(this);
            m_TimeToNextBlink += gameTime.ElapsedGameTime.TotalSeconds;

            if (m_TimeToNextBlink >= Enemy.speedMovement)
            {
                if (Visible)
                {
                    switch (EnemyMovementStatus)
                    {
                        case Enemy.eEnemyMovementOptions.MoveDown:
                            Position = new Vector2(Position.X, Position.Y + Texture.Height / 2);
                            Direction *= -1f;
                            EnemyMovementStatus = Enemy.eEnemyMovementOptions.MoveRegular;
                            (Game as SpaceInvaders).IsChangeEnemyDirection = true;
                            (Game as SpaceInvaders).IsChangeEnemiesIntervalBetweenJumps = true;
                            (Game as SpaceInvaders).IsCanEnemyMatrixMoveRegular = true;
                            break;
                        case Enemy.eEnemyMovementOptions.MoveGap:
                            Position = new Vector2(Position.X + (Game as SpaceInvaders).GapToWall, Position.Y);
                            EnemyMovementStatus = Enemy.eEnemyMovementOptions.MoveDown;
                            break;
                        case Enemy.eEnemyMovementOptions.MoveRegular:
                            Position = new Vector2(Position.X + (Direction) * (Texture.Width / 2), Position.Y);
                            break;
                    }
                

                m_TimeToNextBlink -= Enemy.speedMovement;
                }
            }
        }

        public override void Add(EnemyComponent c)
        {
            // TODO
        }

        public override void Remove(EnemyComponent c)
        {
            // TODO
        }

        public override string ToString()
        {
            return "Enemy";
        }
        

    }
}
