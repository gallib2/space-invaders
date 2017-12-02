using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Infrastructure;

namespace SpaceInvaders
{
    public abstract class EnemyBase : Sprite, Ivulnerable
    {
        public eEnemyTypes Type { get; set; }

        public bool IsHit { get; set; }

        public EnemyBase(Game game) : base(game)
        {
        }

        public override string ToString()
        {
            return "Enemy";
        }
    }
}
