using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class Enemy
    {
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public bool IsVisible { get; set; }

        public Texture2D TextureEnemy { get; set; }
        public static eEnemyMovementOptions EnemyMovementStatus { get; set; }

        public static bool IsEnemyMoveRight { get; set; }
        public static float Direction { get; set; }

        public static float speedMovement { get; set; }

        public static readonly float sr_TimePercentBetweenJumps = 0.9f;

        public void InitPosition()
        {
            // Init the enemy position
            float x = TextureEnemy.Width;
            float y = TextureEnemy.Height * 3;

            // Offset:
            x -= TextureEnemy.Width / 2;

            Position = new Vector2(x, y);
            Direction = 1f;
        }
        


        public enum eEnemyMovementOptions
        {
            MoveDown,
            MoveGap,
            MoveRegular
        }



    }
}
