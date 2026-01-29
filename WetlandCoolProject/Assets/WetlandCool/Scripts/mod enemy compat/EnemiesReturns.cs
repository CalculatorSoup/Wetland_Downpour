using EnemiesReturns;
using EnemiesReturns.Configuration;
using EnemiesReturns.Configuration.LynxTribe;
using EnemiesReturns.Enemies.LynxTribe;
using EnemiesReturns.Enemies.LynxTribe.Scout;
using EnemiesReturns.Enemies.LynxTribe.Shaman;
using EnemiesReturns.Enemies.LynxTribe.Totem;
using EnemiesReturns.Enemies.Spitter;
using R2API;
using RoR2;
using SS2.Monsters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSCStage
{
    public class EnemiesReturnsCompat
    {
        public static void AddEnemies()
        {
            // Lynx Totem
            if (FSCStage.toggleLynxTotem.Value && LynxTotem.Enabled.Value && !LynxTotem.DefaultStageList.Value.Contains("foggyswampdownpour")) //Checking whether default stage list has Lynx Totems to avoid adding a duplicate spawn card
            {
                var totemCard = new RoR2.DirectorCard()
                {
                    spawnCard = (RoR2.SpawnCard)(object)TotemBody.SpawnCards.cscLynxTotemDefault,
                    spawnDistance = RoR2.DirectorCore.MonsterSpawnDistance.Standard,
                    selectionWeight = LynxTotem.SelectionWeight.Value,
                    minimumStageCompletions = LynxTotem.MinimumStageCompletion.Value
                };

                var totemHolder = new DirectorAPI.DirectorCardHolder
                {
                    Card = totemCard,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(totemHolder, false, DirectorAPI.Stage.Custom, "foggyswampdownpour");
                Log.Info("Lynx Totem added to foggyswampdownpour's spawn pool.");

            }

            // Lynx Scout
            if (FSCStage.toggleLynxScout.Value && (LynxTotem.Enabled.Value || LynxShaman.Enabled.Value) && !LynxScout.DefaultStageList.Value.Contains("foggyswampdownpour")) //Lynx Scouts don't have enable/disable config, so just adding these if at least one Lynx enemy is enabled
            {
                var scoutCard = new RoR2.DirectorCard()
                {
                    spawnCard = (RoR2.SpawnCard)(object)ScoutBody.SpawnCards.cscLynxScoutDefault,
                    spawnDistance = RoR2.DirectorCore.MonsterSpawnDistance.Standard,
                    selectionWeight = LynxScout.SelectionWeight.Value,
                    minimumStageCompletions = LynxScout.MinimumStageCompletion.Value
                };

                var scoutHolder = new DirectorAPI.DirectorCardHolder
                {
                    Card = scoutCard,
                    MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(scoutHolder, false, DirectorAPI.Stage.Custom, "foggyswampdownpour");
                Log.Info("Lynx Scout added to foggyswampdownpour's spawn pool.");

            }

            // Lynx Shaman
            if (FSCStage.toggleLynxShaman.Value && LynxShaman.Enabled.Value && !LynxShaman.DefaultStageList.Value.Contains("foggyswampdownpour"))
            {
                var shamanCard = new RoR2.DirectorCard()
                {
                    spawnCard = (RoR2.SpawnCard)(object)ShamanBody.SpawnCards.cscLynxShamanDefault,
                    spawnDistance = RoR2.DirectorCore.MonsterSpawnDistance.Standard,
                    selectionWeight = LynxShaman.SelectionWeight.Value,
                    minimumStageCompletions = LynxShaman.MinimumStageCompletion.Value
                };

                var shamanHolder = new DirectorAPI.DirectorCardHolder
                {
                    Card = shamanCard,
                    MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(shamanHolder, false, DirectorAPI.Stage.Custom, "foggyswampdownpour");
                Log.Info("Lynx Shaman added to foggyswampdownpour's spawn pool.");

            }


            // Spitter
            if (FSCStage.toggleSpitter.Value && Spitter.Enabled.Value && !Spitter.DefaultStageList.Value.Contains("foggyswampdownpour"))
            {
                var spitterCard = new RoR2.DirectorCard()
                {
                    spawnCard = (RoR2.SpawnCard)(object)SpitterBody.SpawnCards.cscSpitterDefault,
                    spawnDistance = RoR2.DirectorCore.MonsterSpawnDistance.Standard,
                    selectionWeight = Spitter.SelectionWeight.Value,
                    minimumStageCompletions = Spitter.MinimumStageCompletion.Value
                };

                var spitterHolder = new DirectorAPI.DirectorCardHolder
                {
                    Card = spitterCard,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(spitterHolder, false, DirectorAPI.Stage.Custom, "foggyswampdownpour");
                Log.Info("Spitter added to foggyswampdownpour's spawn pool.");
            }
        }
    }
}