using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        public override void Update(GameTime gameTime)
        {
            KeyboardState currKeyboardState = Keyboard.GetState();
            //shootStatus();

            // move the ship using the mouse:
            Position = new Vector2((Position.X + Game.GetMousePositionDelta().X), Position.Y);

            if (currKeyboardState.IsKeyDown(Keys.Right) /*&& m_PrevKeyBoardStat != null && m_PrevKeyBoardStat.Value.IsKeyUp(Keys.Right)*/)
            {
                Position = new Vector2((Position.X + 115 * (float)gameTime.ElapsedGameTime.TotalSeconds), Position.Y);
            }
            if (currKeyboardState.IsKeyDown(Keys.Left) /*&& m_PrevKeyBoardStat != null && m_PrevKeyBoardStat.Value.IsKeyUp(Keys.Right)*/)
            {
                Position = new Vector2((Position.X - 115 * (float)gameTime.ElapsedGameTime.TotalSeconds), Position.Y);
            }
            Game.PrevKeyBoardStat = currKeyboardState;


            // clam the position between screen boundries:
            Position = new Vector2(MathHelper.Clamp(Position.X, 0, this.Game.GraphicsDevice.Viewport.Width - Texture.Width), Position.Y);

            // if we hit the wall, lets change direction:
            if (Position.X == 0 || Position.X == this.Game.GraphicsDevice.Viewport.Width - Texture.Width)
            {
                Direction *= -1f;
            }
        }
    }
}
