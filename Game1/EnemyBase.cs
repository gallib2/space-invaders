using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public abstract class EnemyBase : Entity, Ivulnerable
    {
        public eEnemyTypes Type { get; set; }
        //public bool IsVisible { get; set; }
        public bool IsHitted { get; set; }

        public EnemyBase(SpaceInvaders game): base(game)
        {
        }

        public enum eEnemyTypes
        {
            Enemy1 = 280,
            Enemy2 = 200,
            Enemy3 = 115,
            MotherShip = 900
        }
    }
}
