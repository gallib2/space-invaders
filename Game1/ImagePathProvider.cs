using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public static class ImagePathProvider
    {
        public static Dictionary<EnemyBase.eEnemyTypes, string> EnemyiesPathImageDictionary { get; set; }
        public static string BulletPathImage { get; set; }
        public static string SpaceShipPathImage { get; set; }
        public static string BackgroundPathImage { get; set; }

        public static void InitializeImagesPath()
        {
            InitalizeEnemiesDictionary();
            BulletPathImage = @"Sprites\Bullet";
            SpaceShipPathImage = @"Sprites\Ship01_32x32";
            BackgroundPathImage = @"Sprites\BG_Space01_1024x768";
        }

        private static void InitalizeEnemiesDictionary()
        {
            EnemyiesPathImageDictionary = new Dictionary<EnemyBase.eEnemyTypes, string>();
            EnemyiesPathImageDictionary[EnemyBase.eEnemyTypes.Enemy1] = @"Sprites\Enemy0101_32x32";
            EnemyiesPathImageDictionary[EnemyBase.eEnemyTypes.Enemy2] = @"Sprites\Enemy0201_32x32";
            EnemyiesPathImageDictionary[EnemyBase.eEnemyTypes.Enemy3] = @"Sprites\Enemy0301_32x32";
            EnemyiesPathImageDictionary[EnemyBase.eEnemyTypes.MotherShip] = @"Sprites\MotherShip_32x120";
        }

        public static void InitializeImagesPathStarWars()
        {
            InitalizeEnemiesStartWarsDictionary();
            BulletPathImage = @"Sprites\Bullet";
            SpaceShipPathImage = @"StarWars\bb-8-12864X64t"; //@"StarWars\bb-8-64X64";
            BackgroundPathImage = @"StarWars\star-wars-death-star-1024×768";
        }

        private static void InitalizeEnemiesStartWarsDictionary()
        {
            EnemyiesPathImageDictionary = new Dictionary<EnemyBase.eEnemyTypes, string>();
            EnemyiesPathImageDictionary[EnemyBase.eEnemyTypes.Enemy1] = @"StarWars\Star_Wars_-_Darth_Vader";
            EnemyiesPathImageDictionary[EnemyBase.eEnemyTypes.Enemy2] = @"StarWars\Star_Wars_-_Darth_Vader";
            EnemyiesPathImageDictionary[EnemyBase.eEnemyTypes.Enemy3] = @"StarWars\Star_Wars_-_Darth_Vader";
            EnemyiesPathImageDictionary[EnemyBase.eEnemyTypes.MotherShip] = @"Sprites\MotherShip_32x120";
        }
       
    }
}
