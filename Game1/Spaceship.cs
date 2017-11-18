﻿using Microsoft.Xna.Framework;
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
    public class Spaceship : Entity, IShootable
    {
        public float Direction { get; set; }

        public bool IsPossibleToShoot { get; set; }

        private const int m_MinPossibleFlyingBullets = 4;


        public Spaceship(SpaceInvaders spaceInvaders) : base(spaceInvaders)
        {

        }

        public override void LoadContent(ContentManager i_content)
        {
            Texture = i_content.Load<Texture2D>(ImagePathProvider.SpaceShipPathImage);
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
            Bullet.eBulletType bulletType = Bullet.eBulletType.SpaceShip;

            Bullet bullet = new Bullet(bulletType, Game as SpaceInvaders);
            bullet.LoadContent((Game as SpaceInvaders).Content);
            bullet.Position = new Vector2(Position.X + Texture.Width / 2 - 1, Position.Y);
            (Game as SpaceInvaders).GameComponents.Add(bullet);
        }

        private void shootStatus()
        {
            if ( (InputStateProvider.IsLeftMouseButtonClicked() || InputStateProvider.IsButtonClicked(Keys.Enter)) && isPossibleToShoot())
            {
                 Shoot();
            }
                  
        }

        private bool isPossibleToShoot()
        {
            return Bullet.s_NumberOfSpaceShipBullets < m_MinPossibleFlyingBullets;
        }
    }
}
