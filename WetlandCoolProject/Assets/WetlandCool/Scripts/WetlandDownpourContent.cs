using HG;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using RoR2.ExpansionManagement;
using RoR2.Networking;
using RoR2BepInExPack.GameAssetPaths;
using ShaderSwapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using static RoR2.Console;
using static UnityEngine.UI.Image;

namespace FSCStage.Content
{
    public static class FSCContent
    {

        internal const string ScenesAssetBundleFileName = "WetlandCoolScene";
        internal const string AssetsAssetBundleFileName = "WetlandCoolAssets";

        private static AssetBundle _scenesAssetBundle;
        private static AssetBundle _assetsAssetBundle;

        internal static UnlockableDef[] UnlockableDefs;
        internal static SceneDef[] SceneDefs;

        internal static SceneDef FSCSceneDef;
        internal static Sprite FSCSceneDefPreviewSprite;
        internal static Material FSCBazaarSeer;

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
            }));

            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<SceneDef[]>)((assets) =>
            {
                SceneDefs = assets;
                FSCSceneDef = SceneDefs.First(sd => sd.baseSceneNameOverride == "foggyswampdownpour");
                Log.Debug(FSCSceneDef.nameToken);
                contentPack.sceneDefs.Add(assets);
            }));

            FSCSceneDef.portalMaterial = R2API.StageRegistration.MakeBazaarSeerMaterial((Texture2D)FSCSceneDef.previewTexture);

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

            if (FSCStage.loopVariant.Value)
            {
                R2API.StageRegistration.RegisterSceneDefToNormalProgression(FSCSceneDef, 0, false, true);  // Downpour replaces Wetland Aspect via script, so setting weight to 0 to not make Wetland variants more common
            } else if (FSCStage.replaceFoggyswamp.Value)
            {
                R2API.StageRegistration.RegisterSceneDefToNormalProgression(FSCSceneDef, 0);
            } else
            {
                R2API.StageRegistration.RegisterSceneDefToNormalProgression(FSCSceneDef);
            }
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
