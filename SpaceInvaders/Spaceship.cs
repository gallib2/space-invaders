using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Infrastructure;

namespace SpaceInvaders
{
    public class Spaceship : Sprite, IShootable, Ivulnerable
    {
        private const int m_MinPossibleFlyingBullets = 4;

        public float Direction { get; set; }

        public bool IsPossibleToShoot { get; set; }

        public Spaceship(Game spaceInvaders) : base(spaceInvaders)
        {
            Direction = 1f;
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

        public override void Update(GameTime i_GameTime)
        {
            shootStatus();

            // move the ship using the mouse:
            Position = new Vector2(Position.X + InputStateProvider.GetMousePositionDelta().X, Position.Y);

            if (InputStateProvider.CurrKeyboardState.IsKeyDown(Keys.Right))
            {
                Position = new Vector2(Position.X + (115 * (float)i_GameTime.ElapsedGameTime.TotalSeconds), Position.Y);
            }

            if (InputStateProvider.CurrKeyboardState.IsKeyDown(Keys.Left))
            {
                Position = new Vector2(Position.X - (115 * (float)i_GameTime.ElapsedGameTime.TotalSeconds), Position.Y);
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
            eBulletType bulletType = eBulletType.Spaceship;

            Bullet bullet = new Bullet(bulletType, Game as SpaceInvaders);
            (Game as SpaceInvaders).Components.Add(bullet);
            bullet.Position = new Vector2((Position.X + (Texture.Width / 2)) - 1, Position.Y);
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
