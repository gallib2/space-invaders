using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class Spaceship : Entity
    {
        public float Direction { get; set; }

        public bool IsPossibleToShoot { get; set; }

        public Spaceship(SpaceInvaders spaceInvaders) : base(spaceInvaders)
        {

        }

        public override void LoadContent(ContentManager i_content)
        {
            Texture = i_content.Load<Texture2D>(ImagePathProvider.SpaceShipPathImage);
        }
    }
}
