using R2API;
using RoR2;
using SS2;
using SS2.Survivors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSCStage
{
    public class Starstorm2Compat
    {
        public static string FindEnemyConfig(string monsterName) //Lamp, Lamp Boss, Acid Bug
        {
            var defstring = "00 - Enemy Disabling.Disable Enemy: " + monsterName;
            var monsterConfig = SS2Config.ConfigMonster.GetConfigEntries();
            foreach (var entry in monsterConfig)
            {
                if (entry.Definition.ToString() == defstring)
                {
                    var configValue = entry.GetSerializedValue();
                    return configValue;
                }
            }
            return "false";

        }

        public static void AddEnemies()
        {
            // Wayfarer
            var wayfarerValue = FindEnemyConfig("Lamp Boss");

            if (FSCStage.toggleWayfarer.Value && wayfarerValue == "false")
            {
                var wayfarerCard = new RoR2.DirectorCard()
                {
                    spawnCard = SS2Assets.LoadAsset<RoR2.SpawnCard>("scLampBoss", (SS2Bundle)17),
                    spawnDistance = RoR2.DirectorCore.MonsterSpawnDistance.Standard,
                    selectionWeight = 1
                };

                var wayfarerHolder = new DirectorAPI.DirectorCardHolder
                {
                    Card = wayfarerCard,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(wayfarerHolder, false, DirectorAPI.Stage.Custom, "foggyswampdownpour");
                Log.Info("Wayfarer added to foggyswampdownpour's spawn pool.");
            }

            // Follower
            var followerValue = FindEnemyConfig("Lamp");

            if (FSCStage.toggleFollower.Value && followerValue == "false")
            {
                var followerCard = new RoR2.DirectorCard()
                {
                    spawnCard = SS2Assets.LoadAsset<RoR2.SpawnCard>("scLamp", (SS2Bundle)17),
                    spawnDistance = RoR2.DirectorCore.MonsterSpawnDistance.Far,
                    selectionWeight = 2
                };

                var followerHolder = new DirectorAPI.DirectorCardHolder
                {
                    Card = followerCard,
                    MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(followerHolder, false, DirectorAPI.Stage.Custom, "foggyswampdownpour");
                Log.Info("Follower added to foggyswampdownpour's spawn pool.");
            }

            // Beta-exclusive enemies
            if (SS2Config.enableBeta.value)
            {
                // Archer Bug/Wasp
                var acidBugValue = FindEnemyConfig("Acid Bug");

                if (FSCStage.toggleAcidBug.Value && acidBugValue == "false")
                {
                    var acidBugCard = new RoR2.DirectorCard()
                    {
                        spawnCard = SS2Assets.LoadAsset<RoR2.SpawnCard>("cscAcidBug", (SS2Bundle)17),
                        spawnDistance = RoR2.DirectorCore.MonsterSpawnDistance.Standard,
                        selectionWeight = 2,
                        minimumStageCompletions = 5
                    };

                    var acidBugHolder = new DirectorAPI.DirectorCardHolder
                    {
                        Card = acidBugCard,
                        MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters
                    };
                    DirectorAPI.Helpers.AddNewMonsterToStage(acidBugHolder, false, DirectorAPI.Stage.Custom, "foggyswampdownpour");
                    Log.Info("Archer Wasp added to foggyswampdownpour's spawn pool.");

                }
            }

        }

    }

}
