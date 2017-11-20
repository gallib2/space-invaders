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
    public class Enemy : EnemyBase, IShootable
    {
        public eEnemyMovementOptions EnemyMovementStatus { get; set; }
        public static bool IsEnemyMoveRight { get; set; }
        public static float speedMovement { get; set; }
        public readonly float sr_TimePercentBetweenJumps = 0.9f;
        private double m_TimeToNextBlink;
        //public bool IsHitted { get; set; }
        //public bool IsVisible { get; set; }


        public float Direction { get; set; }

        public Enemy(SpaceInvaders spaceInvaders) : base(spaceInvaders)
        {
            EnemyMovementStatus = eEnemyMovementOptions.MoveRegular;
            Direction = 1f;
            //IsVisible = true;
        }


        public enum eEnemyMovementOptions
        {
            MoveDown,
            MoveGap,
            MoveRegular
        }

        public void Shoot()
        {
            Bullet.eBulletType bulletType = Bullet.eBulletType.Enemy;

            Bullet bullet = new Bullet(bulletType, Game as SpaceInvaders);
            bullet.LoadContent((Game as SpaceInvaders).Content);
            bullet.Position = new Vector2(Position.X + Texture.Width / 2 - 1, Position.Y);
            (Game as SpaceInvaders).GameComponents.Add(bullet);
        }

        public override void Update(GameTime gameTime)
        {
            m_TimeToNextBlink += gameTime.ElapsedGameTime.TotalSeconds;

            if (m_TimeToNextBlink >= Enemy.speedMovement)
            {
                if (IsVisible)
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

        //public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Entity entity)
        //{
        //    spriteBatch.Draw(entity.Texture, entity.Position, entity.Color);
        //}

        //public void enemyMoveGap()
        //{
        //    Position = new Vector2(Position.X + this.Game.GapToWall, Position.Y);
        //}

        //private void enemyMoveDown()
        //{
        //    Position = new Vector2(Position.X, Position.Y + Texture.Height / 2);
        //}

        //private void enemyMoveRegular()
        //{
        //    Position = new Vector2(Position.X + (Enemy.Direction) * (Texture.Width / 2), Position.Y);
        //}

    }
}
