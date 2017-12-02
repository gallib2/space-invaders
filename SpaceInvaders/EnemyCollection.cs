using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Microsoft.Xna.Framework;

namespace SpaceInvaders
{
    public class EnemyCollection : EnemyComponent, IEnumerable<EnemyComponent>
    {
        private const int k_RandomTimeToShoot = 200;
        private const int k_NumOfRows = 5;
        private const int k_NumOfColumns = 9;
        private List<EnemyComponent> m_EnemiesMatrix;
        private bool m_IsEnemyTimeToShoot;

        public EnemyCollection(Game game) : base(game)
        {
            m_EnemiesMatrix = new List<EnemyComponent>();
        }

        public EnemyComponent this[int index]
        {
            get
            {
                return m_EnemiesMatrix[index];
            }
        }

        public static int NumOfRows
        {
            get
            {
                return k_NumOfRows;
            }
        }

        public static int NumOfColumns
        {
            get
            {
                return k_NumOfColumns;
            }
        }

        public IEnumerator<EnemyComponent> GetEnumerator()
        {
            foreach (EnemyComponent enemy in m_EnemiesMatrix)
            {
                yield return enemy;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override void Add(EnemyComponent enemy)
        {
            m_EnemiesMatrix.Add(enemy);
        }

        public override void Remove(EnemyComponent enemy)
        {
            m_EnemiesMatrix.Remove(enemy);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            foreach (var item in m_EnemiesMatrix)
            {
                item.Initialize();
            }
        }

        public override void Draw(GameTime i_GameTime)
        {
            foreach (EnemyComponent enemy in m_EnemiesMatrix)
            {
                if (enemy.Visible)
                {
                    enemy.Draw(i_GameTime);
                }
            }

            // base.Draw(gameTime);
        }

        public override void Update(GameTime i_GameTime)
        {
            base.Update(i_GameTime);

            const bool v_EnemyTimeToShoot = true;

            if (m_IsEnemyTimeToShoot == !v_EnemyTimeToShoot)
            {
                if (isEnemyTimeToShoot())
                {
                    KeyValuePair<int, int> randomRowAndCol = getRandomRowAndCol();
                    EnemyComponent enemy = m_EnemiesMatrix[randomRowAndCol.Key * randomRowAndCol.Value];
                    (enemy as Enemy).Shoot();
                    m_IsEnemyTimeToShoot = !v_EnemyTimeToShoot;
                }
            }

            checkIfEnemyNeedMoveGapAndUpdate();

            foreach (EnemyComponent enemy in m_EnemiesMatrix)
            {
                enemy.Update(i_GameTime);
            }

            if (IsChangeEnemiesIntervalBetweenJumps)
            {
                Enemy.speedMovement = Enemy.speedMovement * 0.91f;
                IsChangeEnemiesIntervalBetweenJumps = false;
            }

            if (s_IsXEnemiesDead)
            {
                Enemy.speedMovement = Enemy.speedMovement * 0.94f;
                s_IsXEnemiesDead = false;
            }

            if (IsChangeEnemyDirection)
            {
                Enemy.IsEnemyMoveRight = !Enemy.IsEnemyMoveRight;
                IsChangeEnemyDirection = false;
            }
        }

        private bool isEnemyTimeToShoot()
        {
            bool isTimeForEnemyToShoot = SpaceInvaders.RandomNumber.Next() % k_RandomTimeToShoot == 0;

            m_IsEnemyTimeToShoot = isTimeForEnemyToShoot;

            return isTimeForEnemyToShoot;
        }

        private KeyValuePair<int, int> getRandomRowAndCol()
        {
            KeyValuePair<int, int> randomRowAndCol;
            int counter = 100;

            do
            {
                int row = SpaceInvaders.RandomNumber.Next(0, EnemyCollection.NumOfRows);
                int col = SpaceInvaders.RandomNumber.Next(0, EnemyCollection.NumOfColumns);
                randomRowAndCol = new KeyValuePair<int, int>(row, col);
                counter--;
            }
            while (!m_EnemiesMatrix[randomRowAndCol.Key * randomRowAndCol.Value].Visible || (counter > 0));

            return randomRowAndCol;
        }

        private void checkIfEnemyNeedMoveGapAndUpdate()
        {
            if (IsCanEnemyMatrixMoveRegular)
            {
                isEnemyNextMoveIsWallAndUpdateGap();
                if (!IsCanEnemyMatrixMoveRegular)
                {
                    foreach (EnemyComponent enemy in m_EnemiesMatrix)
                    {
                        enemy.EnemyMovementStatus = eEnemyMovementOptions.MoveGap;
                    }
                }
            }
        }

        private bool isEnemyNextMoveIsWallAndUpdateGap()
        {
            bool isNextMoveIsWall = false;
            float nextEnemyPosition;

            if (Enemy.IsEnemyMoveRight)
            {
                foreach (EnemyComponent enemy in m_EnemiesMatrix)
                {
                    if (enemy.Visible)
                    {
                        nextEnemyPosition = enemy.Position.X + (enemy.Texture.Width / 2);
                        if (nextEnemyPosition > this.GraphicsDevice.Viewport.Width - enemy.Texture.Width)
                        {
                            isNextMoveIsWall = true;
                            enemy.EnemyMovementStatus = eEnemyMovementOptions.MoveGap;
                            this.GapToWall = this.GraphicsDevice.Viewport.Width - enemy.Position.X - enemy.Texture.Width;
                            IsCanEnemyMatrixMoveRegular = false;
                        }
                    }
                }
            }
            else
            {
                foreach (EnemyComponent enemy in m_EnemiesMatrix)
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
    }
}
