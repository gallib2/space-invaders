using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public static class InputStateProvider
    {
        public static MouseState? PrevMouseState { get; set; }
        public static KeyboardState? PrevKeyBoardStat { get; set; }

        public static MouseState CurrentMouseState
        {
            get { return Mouse.GetState(); }
        }

        public static KeyboardState CurrKeyboardState
        {
            get { return Keyboard.GetState(); }
        }

        public static GamePadState CurrGamePadState
        {
            get { return GamePad.GetState(PlayerIndex.One); }
        }

        public static Vector2 GetMousePositionDelta()
        {
            Vector2 retVal = Vector2.Zero;

            if (PrevMouseState != null)
            {
                retVal.X = (CurrentMouseState.X - PrevMouseState.Value.X);
                retVal.Y = (CurrentMouseState.Y - PrevMouseState.Value.Y);
            }

            PrevMouseState = CurrentMouseState;

            return retVal;
        }

        public static bool IsLeftMouseButtonClicked()
        {
            bool leftButtonClicked = false;

            if (PrevMouseState != null)
            {
                if (CurrentMouseState.LeftButton == ButtonState.Pressed && PrevMouseState.Value.LeftButton == ButtonState.Released)
                {
                    leftButtonClicked = !leftButtonClicked;
                }
            }

            return leftButtonClicked;
        }

        public static bool IsButtonClicked(Keys key)
        {
            bool ButtonClicked = false;

            if (PrevKeyBoardStat != null)
            {
                if (CurrKeyboardState.IsKeyDown(key) && PrevKeyBoardStat.Value.IsKeyUp(key))
                {
                    ButtonClicked = !ButtonClicked;
                }
            }

            return ButtonClicked;
        }
    }
}
