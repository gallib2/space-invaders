using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public abstract class Sprite : DrawableGameComponent
    {
        public Vector2 Position { get; set; }

        public Color Color { get; set; }

        public Texture2D Texture { get; set; }

        public Sprite(Game game) : base(game)
        {
            Visible = true;
            Color = Color.White;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime i_GameTime)
        {
            base.Update(i_GameTime);
        }

        public override void Draw(GameTime i_GameTime)
        {
            (Game as Base2DGame).SpriteBatch.Draw(Texture, Position, Color);
            base.Update(i_GameTime);
        }
    }
}
