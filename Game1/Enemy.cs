using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class Enemy : Entity
    {
        public bool IsVisible { get; set; }
        public static eEnemyMovementOptions EnemyMovementStatus { get; set; }
        public static bool IsEnemyMoveRight { get; set; }
        public static float speedMovement { get; set; }
        public static readonly float sr_TimePercentBetweenJumps = 0.9f;
        public static float Direction { get; set; }


        public void InitPosition()
        {
            // Init the enemy position
            float x = Texture.Width;
            float y = Texture.Height * 3;

            // Offset:
            x -= Texture.Width / 2;

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
