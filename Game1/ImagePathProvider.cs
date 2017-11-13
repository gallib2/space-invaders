using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public static class ImagePathProvider
    {
        public static Dictionary<eEnemyTypes, string> EnemyiesPathImageDictionary { get; set; }


        public static void InitalizeEnemiesDictionary()
        {
            EnemyiesPathImageDictionary = new Dictionary<eEnemyTypes, string>();
            EnemyiesPathImageDictionary[eEnemyTypes.Enemy1] = @"Sprites\Enemy0101_32x32";
            EnemyiesPathImageDictionary[eEnemyTypes.Enemy2] = @"Sprites\Enemy0201_32x32";
            EnemyiesPathImageDictionary[eEnemyTypes.Enemy3] = @"Sprites\Enemy0301_32x32";
            EnemyiesPathImageDictionary[eEnemyTypes.MotherShip] = @"Sprites\MotherShip_32x120";
        }


        public enum eEnemyTypes
        {
            Enemy1,
            Enemy2,
            Enemy3,
            MotherShip
        }
    }
}
