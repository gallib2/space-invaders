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
    public class Enemy : Entity
    {
        public bool IsVisible { get; set; }
        public eEnemyMovementOptions EnemyMovementStatus { get; set; }
        public static bool IsEnemyMoveRight { get; set; }
        public static float speedMovement { get; set; }
        public readonly float sr_TimePercentBetweenJumps = 0.9f;
        private double m_TimeToNextBlink;

        public float Direction { get; set; }

        public Enemy(SpaceInvaders spaceInvaders) : base(spaceInvaders)
        {
            EnemyMovementStatus = eEnemyMovementOptions.MoveRegular;
            Direction = 1f;
        }

        public void InitPosition()
        {
            // Init the enemy position
            float x = Texture.Width;
            float y = Texture.Height * 3;

            // Offset:
            x -= Texture.Width / 2;

            Position = new Vector2(x, y);
            Direction = 1f;
        }


        public enum eEnemyMovementOptions
        {
            MoveDown,
            MoveGap,
            MoveRegular
        }

        public override void Update(GameTime gameTime)
        {
            m_TimeToNextBlink += gameTime.ElapsedGameTime.TotalSeconds;

            if (m_TimeToNextBlink >= Enemy.speedMovement)
            {
                switch (EnemyMovementStatus)
                {
                    case Enemy.eEnemyMovementOptions.MoveDown:
                        Position = new Vector2(Position.X, Position.Y + Texture.Height / 2);
                        Direction *= -1f;
                        EnemyMovementStatus = Enemy.eEnemyMovementOptions.MoveRegular;
                        Game.IsChangeEnemyDirection = true;
                        Game.IsChangeEnemiesIntervalBetweenJumps = true;
                        Game.IsCanEnemyMatrixMoveRegular = true;
                        break;
                    case Enemy.eEnemyMovementOptions.MoveGap:
                        Position = new Vector2(Position.X + this.Game.GapToWall, Position.Y);
                        EnemyMovementStatus = Enemy.eEnemyMovementOptions.MoveDown;
                        break;
                    case Enemy.eEnemyMovementOptions.MoveRegular:
                        Position = new Vector2(Position.X + (Direction) * (Texture.Width / 2), Position.Y);
                        break;
                }

                m_TimeToNextBlink -= Enemy.speedMovement;
            }
        }

        public void enemyMoveGap()
        {
            Position = new Vector2(Position.X + this.Game.GapToWall, Position.Y);
        }

        private void enemyMoveDown()
        {
            Position = new Vector2(Position.X, Position.Y + Texture.Height / 2);
        }

        //private void enemyMoveRegular()
        //{
        //    Position = new Vector2(Position.X + (Enemy.Direction) * (Texture.Width / 2), Position.Y);
        //}

        //private bool isEnemyNextMoveIsWallAndUpdateGap()
        //{
        //    bool isNextMoveIsWall = false;
        //    float nextEnemyPosition;

        //    if (Enemy.IsEnemyMoveRight)
        //    {
        //        // isNextMoveIsWall = checkConditionOnEveryEnemy(isEnemyNextRightMoveIsWallAndUpdateGap);
        //        nextEnemyPosition = Position.X + (Texture.Width / 2);
        //        if (nextEnemyPosition > this.Game.GraphicsDevice.Viewport.Width - Texture.Width)
        //        {
        //            isNextMoveIsWall = true;
        //            this.GapToWall = this.Game.GraphicsDevice.Viewport.Width - Position.X - Texture.Width;
        //        }
        //    }
        //    else
        //    {
        //        //isNextMoveIsWall = checkConditionOnEveryEnemy(isEnemyNextLeftMoveIsWallAndUpdateGap);
        //        nextEnemyPosition = Position.X - (Texture.Width / 2);
        //        if (nextEnemyPosition < 0)
        //        {
        //            isNextMoveIsWall = true;
        //            this.GapToWall = -Position.X;
        //        }
        //    }

        //    return isNextMoveIsWall;
        //}




    }
}
