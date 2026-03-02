using HG;
using R2API;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FSCStage.Content
{
    public static class Simulacrum
    {
        // Registers a given stage to all Simulacrum scene collections and creates its destination group. Can exclude from the Stage 1 collection if desired.

        public static void RegisterSceneToSimulacrum(SceneDef sceneDef, bool canBeStage1 = true, float weight = R2API.StageRegistration.defaultWeight)
        {
            SceneCollection fullCollection = Addressables.LoadAssetAsync<SceneCollection>("RoR2/DLC1/GameModes/InfiniteTowerRun/SceneGroups/sgInfiniteTowerStageX.asset").WaitForCompletion();
            SceneCollection stage1Collection = Addressables.LoadAssetAsync<SceneCollection>("RoR2/DLC1/GameModes/InfiniteTowerRun/SceneGroups/sgInfiniteTowerStage1.asset").WaitForCompletion();
            List<SceneCollection> stageDestGroups = new List<SceneCollection>();

            // Using 'sgInfiniteTowerStageX' to collect each stage's scene collection 
            foreach (SceneCollection.SceneEntry sceneEntry in fullCollection.sceneEntries)
            {
                if (sceneEntry.sceneDef.hasAnyDestinations)
                {
                    stageDestGroups.Add(sceneEntry.sceneDef.destinationsGroup);
                }
            }

            // Create new destination group for the given stage
            //var sceneDestinationGroup = new SceneCollection();
            SceneCollection sceneDestinationGroup = ScriptableObject.CreateInstance<SceneCollection>();
            sceneDestinationGroup._sceneEntries = fullCollection._sceneEntries;
            sceneDef.destinationsGroup = sceneDestinationGroup;


            // Create new SceneEntry for the given stage, then append to all collections

            SceneCollection.SceneEntry value = new SceneCollection.SceneEntry
            {
                sceneDef = sceneDef,
                weightMinusOne = weight - 1f
            };

            ref SceneCollection.SceneEntry[] fullEntries = ref fullCollection._sceneEntries;
            ArrayUtils.ArrayAppend(ref fullEntries, in value);

            if (canBeStage1)
            {
                ref SceneCollection.SceneEntry[] stage1Entries = ref stage1Collection._sceneEntries;
                ArrayUtils.ArrayAppend(ref stage1Entries, in value);
            }

            foreach (SceneCollection destGroup in stageDestGroups)
            {
                ref SceneCollection.SceneEntry[] destGroupEntries = ref destGroup._sceneEntries;
                ArrayUtils.ArrayAppend(ref destGroupEntries, in value);
            }
        }
    }
}