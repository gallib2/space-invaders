using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public class Bullet : Entity, IDisposable
    {
        public static int s_NumberOfSpaceShipBullets { get; set; }

        public eBulletType BullletType { get; set; }

        private bool disposed = false;

        private const int r_MovementSpeed = 150;

        public Bullet(eBulletType bulletType, SpaceInvaders spaceInveders) : base(spaceInveders)
        {
            this.BullletType = bulletType;
            if(bulletType == eBulletType.SpaceShip)
            {
                s_NumberOfSpaceShipBullets++;
            }
        }

        public override void LoadContent(ContentManager i_content)
        {
            Texture = i_content.Load<Texture2D>(ImagePathProvider.BulletPathImage);
            Color = Color.Red;
        }

        public override void Update(GameTime gameTime)
        {
            if (Position.Y + Texture.Height < 0)
            {
                Dispose();
            }
            else
            {
                Position = new Vector2(Position.X, Position.Y - (r_MovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    //component.Dispose();
                }

                if(this.BullletType == eBulletType.SpaceShip)
                {
                    s_NumberOfSpaceShipBullets--;
                }

                // Note disposing has been done.
                disposed = true;

            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public enum eBulletType
        {
            SpaceShip,
            Enemy
        }
    }
}
