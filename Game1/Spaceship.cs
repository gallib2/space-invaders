using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class Spaceship : Sprite, IShootable, Ivulnerable
    {
        public float Direction { get; set; }

        public bool IsPossibleToShoot { get; set; }
        public bool IsHitted { get; set; }

        private const int m_MinPossibleFlyingBullets = 4;


        public Spaceship(Game spaceInvaders) : base(spaceInvaders)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Texture = (Game as SpaceInvaders).Content.Load<Texture2D>(ImagePathProvider.SpaceShipPathImage);
            Color = Color.White;
        }

        public override void Update(GameTime gameTime)
        {
            shootStatus();

            // move the ship using the mouse:
            Position = new Vector2((Position.X + InputStateProvider.GetMousePositionDelta().X), Position.Y);

            if ( InputStateProvider.CurrKeyboardState.IsKeyDown(Keys.Right) /*&& m_PrevKeyBoardStat != null && m_PrevKeyBoardStat.Value.IsKeyUp(Keys.Right)*/)
            {
                Position = new Vector2((Position.X + 115 * (float)gameTime.ElapsedGameTime.TotalSeconds), Position.Y);
            }
            if (InputStateProvider.CurrKeyboardState.IsKeyDown(Keys.Left) /*&& m_PrevKeyBoardStat != null && m_PrevKeyBoardStat.Value.IsKeyUp(Keys.Right)*/)
            {
                Position = new Vector2((Position.X - 115 * (float)gameTime.ElapsedGameTime.TotalSeconds), Position.Y);
            }
            InputStateProvider.PrevKeyBoardStat = InputStateProvider.CurrKeyboardState;


            // clam the position between screen boundries:
            Position = new Vector2(MathHelper.Clamp(Position.X, 0, this.Game.GraphicsDevice.Viewport.Width - Texture.Width), Position.Y);

            // if we hit the wall, lets change direction:
            if (Position.X == 0 || Position.X == this.Game.GraphicsDevice.Viewport.Width - Texture.Width)
            {
                Direction *= -1f;
            }
        }

        public void Shoot()
        {
            Bullet.eBulletType bulletType = Bullet.eBulletType.Spaceship;

            Bullet bullet = new Bullet(bulletType, Game as SpaceInvaders);
            //bullet.LoadContent((Game as SpaceInvaders).Content);
            (Game as SpaceInvaders).Components.Add(bullet);
            bullet.Position = new Vector2(Position.X + Texture.Width / 2 - 1, Position.Y);
        }

        private void shootStatus()
        {
            if ((InputStateProvider.IsLeftMouseButtonClicked() || InputStateProvider.IsButtonClicked(Keys.Enter)) && isPossibleToShoot())
            {
                Shoot();
            }

        }

        private bool isPossibleToShoot()
        {
            return Bullet.s_NumberOfSpaceShipBullets < m_MinPossibleFlyingBullets;
        }

        public override string ToString()
        {
            return "Spaceship";
        }
    }
}
