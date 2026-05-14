using R2API;
using RoR2;
using RoR2.ContentManagement;
using ShaderSwapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace FSCStage.Content
{
    public static class FSCContent
    {

        internal const string ScenesAssetBundleFileName = "wetlanddownpourscenes";
        internal const string AssetsAssetBundleFileName = "wetlanddownpourassets";

        private static AssetBundle _scenesAssetBundle;
        private static AssetBundle _assetsAssetBundle;

        internal static UnlockableDef[] UnlockableDefs;
        internal static SceneDef[] SceneDefs;

        //Wetland Downpour
        internal static SceneDef FSCSceneDef;
        internal static Sprite FSCSceneDefPreviewSprite;
        internal static Material FSCBazaarSeer;
        //Simulacrum Variant
        internal static SceneDef SimuSceneDef;
        internal static Sprite SimuSceneDefPreviewSprite;
        internal static Material SimuBazaarSeer;

        public static List<Material> SwappedMaterials = new List<Material>();

        internal static IEnumerator LoadAssetBundlesAsync(AssetBundle scenesAssetBundle, AssetBundle assetsAssetBundle, IProgress<float> progress, ContentPack contentPack)
        {
            _scenesAssetBundle = scenesAssetBundle;
            _assetsAssetBundle = assetsAssetBundle;

            var upgradeStubbedShaders = _assetsAssetBundle.UpgradeStubbedShadersAsync();
            while (upgradeStubbedShaders.MoveNext())
            {
                yield return upgradeStubbedShaders.Current;
            }

            yield return LoadAllAssetsAsync(assetsAssetBundle, progress, (Action<UnlockableDef[]>)((assets) =>
            {
                contentPack.unlockableDefs.Add(assets);
            }));


            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<Sprite[]>)((assets) =>
            {
                FSCSceneDefPreviewSprite = assets.First(a => a.name == "texFSCScenePreview");
                SimuSceneDefPreviewSprite = assets.First(a => a.name == "texFSCScenePreview");
            }));

            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<SceneDef[]>)((assets) =>
            {
                SceneDefs = assets;
                FSCSceneDef = SceneDefs.First(sd => sd.baseSceneNameOverride == "foggyswampdownpour");
                SimuSceneDef = SceneDefs.First(sd => sd.baseSceneNameOverride == "itfoggyswampdownpour");
                Log.Debug(FSCSceneDef.nameToken);
                Log.Debug(SimuSceneDef.nameToken);
                contentPack.sceneDefs.Add(assets);
            }));

            FSCSceneDef.portalMaterial = R2API.StageRegistration.MakeBazaarSeerMaterial((Texture2D)FSCSceneDef.previewTexture);
            SimuSceneDef.portalMaterial = R2API.StageRegistration.MakeBazaarSeerMaterial((Texture2D)FSCSceneDef.previewTexture);

            var mainTrackDefRequest = Addressables.LoadAssetAsync<MusicTrackDef>("RoR2/Base/Common/MusicTrackDefs/muFULLSong06.asset");
            while (!mainTrackDefRequest.IsDone)
            {
                yield return null;
            }
            var bossTrackDefRequest = Addressables.LoadAssetAsync<MusicTrackDef>("RoR2/Base/Common/MusicTrackDefs/muSong22.asset");
            while (!bossTrackDefRequest.IsDone)
            {
                yield return null;
            }
            
            FSCSceneDef.mainTrack = mainTrackDefRequest.Result;
            FSCSceneDef.bossTrack = bossTrackDefRequest.Result;

            SimuSceneDef.mainTrack = mainTrackDefRequest.Result;
            SimuSceneDef.bossTrack = bossTrackDefRequest.Result;

            // register Wetland Downpour
            if (FSCStage.regularEnabled.Value)
            {
                if (FSCStage.loopVariant.Value && !FSCStage.replaceFoggyswamp.Value)
                {
                    R2API.StageRegistration.RegisterSceneDefToNormalProgression(FSCSceneDef, StageRegistration.defaultWeight, false, true);
                } else if (FSCStage.replaceFoggyswamp.Value)
                {
                    R2API.StageRegistration.RegisterSceneDefToNormalProgression(FSCSceneDef, StageRegistration.defaultWeight);
                }
                else
                {
                    R2API.StageRegistration.RegisterSceneDefToNormalProgression(FSCSceneDef, StageRegistration.defaultWeight / 2);
                }
            }

            // Register Simulacrum variant
            if (FSCStage.simulacrumEnabled.Value && FSCStage.simulacrumStage1.Value)
            {
                Simulacrum.RegisterSceneToSimulacrum(SimuSceneDef);
            } else if (FSCStage.simulacrumEnabled.Value && !FSCStage.simulacrumStage1.Value)
            {
                Simulacrum.RegisterSceneToSimulacrum(SimuSceneDef, false);
            }

        }

        //this was copied from Goorakh's WeightWrite mod!
        public static void SetSceneWeight(RoR2.SceneDef scene, float weight, RoR2.SceneCollection _sceneCollection)
        {
            if (!scene)
                return;

            for (int i = 0; i < _sceneCollection._sceneEntries.Length; i++)
            {
                ref RoR2.SceneCollection.SceneEntry entry = ref _sceneCollection._sceneEntries[i];
                if (entry.sceneDef == scene)
                {
                    Log.Debug($"Updating {scene} weight in {_sceneCollection}: {entry.weight}->{weight}");
                    entry.weight = weight;
                    //return;
                }
            }

            Log.Warning($"Failed to find '{scene}' in {_sceneCollection}");
        }


        internal static void Unload()
        {
            _assetsAssetBundle.Unload(true);
            _scenesAssetBundle.Unload(true);
        }

        private static IEnumerator LoadAllAssetsAsync<T>(AssetBundle assetBundle, IProgress<float> progress, Action<T[]> onAssetsLoaded) where T : UnityEngine.Object
        {
            var sceneDefsRequest = assetBundle.LoadAllAssetsAsync<T>();
            while (!sceneDefsRequest.isDone)
            {
                progress.Report(sceneDefsRequest.progress);
                yield return null;
            }

            onAssetsLoaded(sceneDefsRequest.allAssets.Cast<T>().ToArray());

            yield break;
        }
    }
}
