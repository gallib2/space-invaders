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
    public abstract class Sprite : DrawableGameComponent
    {
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public Texture2D Texture { get; set; }

        //public Game Game { get; set; }

        public bool IsVisible { get; set; }

        public Sprite(Game game) : base(game)
        {
            //this.Game = game;
            IsVisible = true;
        }

        //protected override void LoadContent(ContentManager i_content)
        //{

        //}

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        //public override void Initialize()
        //{
        //    base.Initialize();
        //}



        //public void Initialize()
        //{
        //    throw new NotImplementedException();
        //}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            (Game as SpaceInvaders).SpriteBatch.Draw(Texture, Position, Color);
            base.Update(gameTime);
        }

    }
}
