using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class Bullet : Entity, IDisposable
    {
        public static int s_NumberOfSpaceShipBullets { get; set; }

        public eBulletType BullletType { get; set; }

        private bool disposed = false;

        public Bullet(eBulletType bulletType)
        {
            this.BullletType = bulletType;
            if(bulletType == eBulletType.SpaceShip)
            {
                s_NumberOfSpaceShipBullets++;
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
