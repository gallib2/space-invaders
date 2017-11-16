using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public class MotherShip : Entity
    {
        public static float speedMovement { get; set; }

        public MotherShip(SpaceInvaders spaceInvaders) : base(spaceInvaders)
        {

        }

        public override void LoadContent(ContentManager i_content)
        {
            Texture = i_content.Load<Texture2D>(ImagePathProvider.EnemyiesPathImageDictionary[ImagePathProvider.eEnemyTypes.MotherShip]);
        }

    }
}
