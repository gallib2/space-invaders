using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceInvaders
{
    public class MotherShip : EnemyBase
    {
        private const int k_RandomTimeToPass = 200;
        private bool m_IsMotherShipNeedToPass;
        private Vector2 m_InitPosition;

        public static float speedMovement { get; set; }

        public Vector2 InitPosition
        {
            get
            {
                return m_InitPosition;
            }
        }

        public MotherShip(SpaceInvaders spaceInvaders) : base(spaceInvaders)
        {
            Type = eEnemyTypes.MotherShip;
            MotherShip.speedMovement = 120f;
        }

        public override void Initialize()
        {
            base.Initialize();
            m_InitPosition = new Vector2(-Texture.Width, Texture.Height);
        }

        protected override void LoadContent()
        {
            Texture = Game.Content.Load<Texture2D>(ImagePathProvider.EnemyiesPathImageDictionary[eEnemyTypes.MotherShip]);
        }

        public override void Update(GameTime i_GameTime)
        {
            const bool v_MothershipTimeToPass = true;

            if (m_IsMotherShipNeedToPass == !v_MothershipTimeToPass)
            {
                checkIfMotherShipTimeToPass();
            }

            if (m_IsMotherShipNeedToPass)
            {
                if (!motherShipPositionOutOfBoundry())
                {
                    Position = new Vector2(Position.X + (MotherShip.speedMovement * (float)i_GameTime.ElapsedGameTime.TotalSeconds), Position.Y);
                }
                else
                {
                    Position = InitPosition;
                    m_IsMotherShipNeedToPass = false;
                }
            }
        }

        private void checkIfMotherShipTimeToPass()
        {
            bool isTimeForMotherShipToPass = SpaceInvaders.RandomNumber.Next() % k_RandomTimeToPass == 0;

            m_IsMotherShipNeedToPass = isTimeForMotherShipToPass;
        }

        private bool motherShipPositionOutOfBoundry()
        {
            bool isOutOfBoundry = false;

            if (Position.X > this.GraphicsDevice.Viewport.Width)
            {
                isOutOfBoundry = !isOutOfBoundry;
            }

            return isOutOfBoundry;
        }
    }
}
