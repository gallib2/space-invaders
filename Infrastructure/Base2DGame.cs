using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Infrastructure
{
    public class Base2DGame : Game
    {
        private GraphicsDeviceManager m_Graphics; // TODO

        public SpriteBatch SpriteBatch { get; set; }

        public Base2DGame()
        {
            m_Graphics = new GraphicsDeviceManager(this);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Draw(GameTime gameTime)
        {
            m_Graphics.GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin();

            base.Draw(gameTime);

            SpriteBatch.End();
        }
    }
}
