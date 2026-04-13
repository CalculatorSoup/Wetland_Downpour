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
            List<SceneCollection> stageSpecificCollections = new List<SceneCollection>();

            foreach (SceneCollection.SceneEntry sceneEntry in fullCollection.sceneEntries)
            {
                if (sceneEntry.sceneDef == sceneDef)
                {
                    Log.Error($"SceneDef {sceneDef.cachedName} is already registered into the Simulacrum");
                    return;
                }
                if (sceneEntry.sceneDef.hasAnyDestinations)
                {
                    stageSpecificCollections.Add(sceneEntry.sceneDef.destinationsGroup);
                }
            }

            SceneCollection sceneDestinationGroup = ScriptableObject.CreateInstance<SceneCollection>();
            sceneDestinationGroup.name = "sgInfiniteTowerStageX" + sceneDef.cachedName;
            sceneDestinationGroup._sceneEntries = fullCollection._sceneEntries;
            sceneDef.destinationsGroup = sceneDestinationGroup;

            SceneCollection.SceneEntry newSceneEntry = new SceneCollection.SceneEntry
            {
                sceneDef = sceneDef,
                weightMinusOne = weight - 1f
            };

            ref SceneCollection.SceneEntry[] allSceneEntries = ref fullCollection._sceneEntries;
            ArrayUtils.ArrayAppend(ref allSceneEntries, in newSceneEntry);

            if (canBeStage1)
            {
                ref SceneCollection.SceneEntry[] stage1Entries = ref stage1Collection._sceneEntries;
                ArrayUtils.ArrayAppend(ref stage1Entries, in newSceneEntry);
            }
            foreach (SceneCollection destinationGroup in stageSpecificCollections)
            {
                ref SceneCollection.SceneEntry[] destGroupEntries = ref destinationGroup._sceneEntries;
                ArrayUtils.ArrayAppend(ref destGroupEntries, in newSceneEntry);
            }
        }
    }
}