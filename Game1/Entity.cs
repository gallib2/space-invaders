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
    public abstract class Entity
    {
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public Texture2D Texture { get; set; }

        public SpaceInvaders Game { get; set; }

        public Entity(SpaceInvaders spaceInveders)
        {
            this.Game = spaceInveders;
        }

        public virtual void LoadContent(ContentManager i_content)
        {

        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public virtual void Update(GameTime gameTime)
        {
            
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }
        
    }
}
