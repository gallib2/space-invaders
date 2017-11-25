using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class Background: Sprite
    {
        public Background(Game game): base(game)
        {
        }

        protected override void LoadContent()
        {
            Texture = Game.Content.Load<Texture2D>(ImagePathProvider.BackgroundPathImage);
            //base.LoadContent();
        }
    }
}
