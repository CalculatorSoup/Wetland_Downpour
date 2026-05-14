using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSCStage
{
    public class SwampMinibossCompat
    {
        static Vector3 foggySwampSpawn;
        static Vector3 downpourSpawn = new Vector3(-900, 73, -2162); //idk why the gameplay space is so far away from the origin point. i did not know what i was doing when i was making this map. lol

        // grabs the default spawn point value, called when the game finishes loading. just doing this to ensure foggyswamp always uses the correct spawn point in case it is ever changed in an update to SwampMiniboss
        public static void GetFoggySwampSpawnPoint()
        {
            foggySwampSpawn = SwampMiniboss.Plugin.spawnPoint;
        }

        public static void SetSpawnPointValue(string sceneName)
        {
            if (sceneName == "foggyswamp")
            {
                SwampMiniboss.Plugin.spawnPoint = foggySwampSpawn;
            }
            else if (sceneName == "foggyswampdownpour" || sceneName == "itfoggyswampdownpour")
            {
                SwampMiniboss.Plugin.spawnPoint = downpourSpawn;
            }
        }

    }
}
