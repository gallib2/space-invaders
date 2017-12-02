using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SpaceInvaders
{
    public abstract class EnemyComponent : EnemyBase
    {
        protected const int k_NumberOfColumns = 9;
        private readonly float r_TimePercentBetweenJumps = 0.9f;
        protected double m_TimeToNextBlink;
        protected static int s_CurrentRowOfLoadedEnemies;
        protected static int s_NumberOfLoadedEnemies;
        protected static bool s_IsXEnemiesDead;

        public static bool IsChangeEnemyDirection { get; set; }

        public static bool IsChangeEnemiesIntervalBetweenJumps { get; set; }

        public static bool IsCanEnemyMatrixMoveRegular { get; set; }

        public static bool IsEnemyMoveRight { get; set; }

        public static float speedMovement { get; set; }

        public static bool IsXEnemiesDead
        {
            set
            {
                s_IsXEnemiesDead = value;
            }
        }

        protected static int S_NumberOfLoadedEnemies
        {
            get
            {
                return s_NumberOfLoadedEnemies;
            }

            set
            {
                s_NumberOfLoadedEnemies = value;
            }
        }

        public float GapToWall { get; set; }

        public float Direction { get; set; }

        public eEnemyMovementOptions EnemyMovementStatus { get; set; }

        public EnemyComponent(Game game) : base(game)
        {
            EnemyMovementStatus = eEnemyMovementOptions.MoveRegular;
            Direction = 1f;
            IsCanEnemyMatrixMoveRegular = true;
            Enemy.IsEnemyMoveRight = true;
            Enemy.speedMovement = 0.5f;
        }

        public abstract void Add(EnemyComponent o_enemy);

        public abstract void Remove(EnemyComponent o_enemy);

        protected KeyValuePair<string, Color> getEnemyImageAndColorAndSetType(int i_Row, Enemy io_Enemy)
        {
            KeyValuePair<string, Color> imageAndColorToLoad;

            if (i_Row == 0)
            {
                io_Enemy.Type = eEnemyTypes.Enemy1;
                imageAndColorToLoad = new KeyValuePair<string, Color>(ImagePathProvider.EnemyiesPathImageDictionary[io_Enemy.Type], Color.LightPink);
            }
            else if (i_Row == 1 || i_Row == 2)
            {
                io_Enemy.Type = eEnemyTypes.Enemy2;
                imageAndColorToLoad = new KeyValuePair<string, Color>(ImagePathProvider.EnemyiesPathImageDictionary[io_Enemy.Type], Color.LightBlue);
            }
            else
            {
                io_Enemy.Type = eEnemyTypes.Enemy3;
                imageAndColorToLoad = new KeyValuePair<string, Color>(ImagePathProvider.EnemyiesPathImageDictionary[io_Enemy.Type], Color.White);
            }

            return imageAndColorToLoad;
        }
    }
}
