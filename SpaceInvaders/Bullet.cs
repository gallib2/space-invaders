using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Infrastructure;

namespace SpaceInvaders
{
    public class Bullet : Sprite
    {
        private const int v_MovementSpeed = 150;
        private bool disposed = false;

        public static int s_NumberOfSpaceShipBullets { get; set; }

        public eBulletType BullletType { get; set; }

        public Bullet(eBulletType bulletType, Game game) : base(game)
        {
            this.BullletType = bulletType;
            Color = Color.Blue;
            
            if(bulletType == eBulletType.Spaceship)
            {
                s_NumberOfSpaceShipBullets++;
                Color = Color.Red;
            }
        }

        protected override void LoadContent()
        {
            Texture = (Game as SpaceInvaders).Content.Load<Texture2D>(ImagePathProvider.BulletPathImage);
        }

        public override void Update(GameTime i_GameTime)
        {
            if (Position.Y + Texture.Height < 0)
            {
                Dispose();
            }
            else if(BullletType == eBulletType.Spaceship)
            {
                Position = new Vector2(Position.X, Position.Y - (v_MovementSpeed * (float)i_GameTime.ElapsedGameTime.TotalSeconds));
            }
            else
            {
                // case bulletType == eBulletType.Enemy
                Position = new Vector2(Position.X, Position.Y + (v_MovementSpeed * (float)i_GameTime.ElapsedGameTime.TotalSeconds));
            }
        }

        protected override void Dispose(bool o_disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                if (this.BullletType == eBulletType.Spaceship)
                {
                    s_NumberOfSpaceShipBullets--;
                }

                // Note disposing has been done.
                disposed = true;
            }

           // base.Dispose();
        }
    }
}
