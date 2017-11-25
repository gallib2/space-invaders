using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public abstract class EnemyComponent: EnemyBase
    {
        public eEnemyMovementOptions EnemyMovementStatus { get; set; }
        public static bool IsEnemyMoveRight { get; set; }
        public static float speedMovement { get; set; }
        public readonly float sr_TimePercentBetweenJumps = 0.9f;
        protected double m_TimeToNextBlink;
        public float Direction { get; set; }

        protected const int m_NumberOfColumns = 9;
        protected static int m_CurrentRowOfLoadedEnemies;
        protected static int m_NumberOfLoadedEnemies;


        public EnemyComponent(Game game): base(game)
        {
            EnemyMovementStatus = eEnemyMovementOptions.MoveRegular;
            Direction = 1f;
        }

        public enum eEnemyMovementOptions
        {
            MoveDown,
            MoveGap,
            MoveRegular
        }

        public abstract void Add(EnemyComponent c);
        public abstract void Remove(EnemyComponent c);

        //public abstract void LoadContent();

    }
}
