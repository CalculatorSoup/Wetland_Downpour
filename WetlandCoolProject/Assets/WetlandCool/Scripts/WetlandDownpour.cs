using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using FSCStage.Content;
using HG;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Diagnostics;
using RoR2.Artifacts;
using UnityEngine.SceneManagement;
using ContentProvider = FSCStage.Content.ContentProvider;
//Copied from Fogbound Lagoon copied from Nuketown


#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete
[assembly: HG.Reflection.SearchableAttribute.OptIn]


namespace FSCStage
{
    [BepInPlugin(GUID, Name, Version)]
    public class FSCStage : BaseUnityPlugin
    {
        public const string Author = "wormsworms";

        public const string Name = "WetlandDownpour";

        public const string Version = "1.1.1";

        public const string GUID = Author + "." + Name;

        public static FSCStage instance;

        public static ConfigEntry<bool> loopVariant;
        public static ConfigEntry<bool> replaceFoggyswamp;
        public static ConfigEntry<bool> regularEnabled;
        public static ConfigEntry<bool> waterMuffle;

        public static ConfigEntry<bool> simulacrumEnabled;
        public static ConfigEntry<bool> simulacrumStage1;

        public static ConfigEntry<bool> toggleWayfarer;
        public static ConfigEntry<bool> toggleFollower;
        public static ConfigEntry<bool> toggleAcidBug;

        public static ConfigEntry<bool> toggleLynxTotem;
        public static ConfigEntry<bool> toggleLynxScout;
        public static ConfigEntry<bool> toggleLynxShaman;
        public static ConfigEntry<bool> toggleSpitter;
        public static ConfigEntry<bool> toggleArcherBugER;

        private void Awake()
        {
            instance = this;

            Log.Init(Logger);

            ConfigSetup();

            ContentManager.collectContentPackProviders += GiveToRoR2OurContentPackProviders;

            RoR2.Language.collectLanguageRootFolders += CollectLanguageRootFolders;

            //On.RoR2.Run.PickNextStageScene += ReplaceWetlandAspect;

            RoR2.Run.onRunStartGlobal += InitializeBazaarSeerValues;

            RoR2.RoR2Application.onLoad += OnLoad;

            SceneManager.sceneLoaded += SceneSetup;
        }

        public static void OnLoad()
        {
            if (IsStarstorm2.enabled)
            {
                Starstorm2Compat.AddEnemies(); //Wayfarer, Follower, Archer Wasp
            }

            if (IsEnemiesReturns.enabled)
            {
                EnemiesReturnsCompat.AddEnemies(); //Lynx Totem, Lynx Scout, Spitter
            }

            if (IsSwampMiniboss.enabled)
            {
                SwampMinibossCompat.GetFoggySwampSpawnPoint();
            }

            // As far as I can tell, R2API / LoP / etc. don't really have any tools for easily / cleanly setting up a post-loop variant for a vanilla map.
            // If they do, please tell me so I can replace all of this.

            // This code changes Wetland Aspect's weight depending on the config options selected. Downpour is registered and its weight is set in the WetlandDownpourContent script.

            SceneCollection sgStage2 = Addressables.LoadAssetAsync<SceneCollection>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_SceneGroups.sgStage2_asset).WaitForCompletion();
            SceneCollection loopSgStage2 = Addressables.LoadAssetAsync<SceneCollection>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_SceneGroups.loopSgStage2_asset).WaitForCompletion();
            RoR2.SceneDef foggyswamp = RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswamp");

            if (regularEnabled.Value)
            {
                if (loopVariant.Value && !replaceFoggyswamp.Value)
                {
                    FSCContent.SetSceneWeight(foggyswamp, 1, sgStage2);
                    FSCContent.SetSceneWeight(foggyswamp, 0, loopSgStage2);
                    RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswamp").loopedSceneDef = RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswampdownpour");
                    Log.Debug("Wetland Downpour registered (replace Wetland Aspect after looping)");

                }
                else if (replaceFoggyswamp.Value)
                {
                    FSCContent.SetSceneWeight(foggyswamp, 0, sgStage2);
                    FSCContent.SetSceneWeight(foggyswamp, 0, loopSgStage2);
                    foggyswamp.filterOutOfBazaar = true;

                    Log.Debug("Wetland Downpour registered (always replace Wetland Aspect)");
                }
                else
                {
                    FSCContent.SetSceneWeight(foggyswamp, StageRegistration.defaultWeight / 2, sgStage2);
                    FSCContent.SetSceneWeight(foggyswamp, StageRegistration.defaultWeight / 2, loopSgStage2);

                    Log.Debug("Wetland Downpour registered (both Downpour and Aspect always in stage rotation. Weights for Aspect and Downpour halved)");
                }
            }

        }

        public void InitializeBazaarSeerValues(RoR2.Run run)
        {
            if (loopVariant.Value && !replaceFoggyswamp.Value)
            {
                RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswamp").filterOutOfBazaar = false;
                RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswampdownpour").filterOutOfBazaar = true;
            } else if (replaceFoggyswamp.Value)
            {
                RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswamp").filterOutOfBazaar = true;
                RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswampdownpour").filterOutOfBazaar = false;
            }
        }

// old code for swapping out Wetland Aspect for Wetland Downpour. This is no longer needed, but left commented out here just in case I might need to revert back to this

/*
public void ReplaceWetlandAspect(On.RoR2.Run.orig_PickNextStageScene orig, RoR2.Run self, WeightedSelection<RoR2.SceneDef> choices)
        {
            orig.Invoke(self, choices);

            if (FSCStage.loopVariant.Value && !FSCStage.replaceFoggyswamp.Value)
            {
                //if you are going to foggyswamp and more than 3 stages have been cleared, replace it with Wetland Downpour and remove it from Bazaar seer selection
                if (RoR2.Run.instance.stageClearCount > 3 && RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswamp").filterOutOfBazaar != true)
                {
                    RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswamp").filterOutOfBazaar = true;
                    RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswampdownpour").filterOutOfBazaar = false;
                    RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswamp").loopedSceneDef = RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswampdownpour");
                }
                if ((object)self.nextStageScene.baseSceneName != null && (self.nextStageScene.baseSceneName == "foggyswamp" && RoR2.Run.instance.stageClearCount > 3))
                {
                    self.nextStageScene = RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswampdownpour");

                    Log.Debug("Wetland Aspect annihilated by orbital missile; we did it, (read this in a cool voice)");

                }
            }
            else if (FSCStage.replaceFoggyswamp.Value)
            {
                //if you are going to foggyswamp at any stage, replace it with Wetland Downpour
                    if ((object)self.nextStageScene.baseSceneName != null && (self.nextStageScene.baseSceneName == "foggyswamp"))
                    {
                        self.nextStageScene = RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswampdownpour");

                        Log.Debug("Wetland Aspect annihilated by orbital missile; we did it, (read this in a cool voice)");
                    }
            }
        }
*/
        public void SceneSetup(Scene newScene, LoadSceneMode loadSceneMode)
        {
            //Set weight and bazaar seer filters for each stage depending on whether they should be active
            if (regularEnabled.Value && loopVariant.Value && !replaceFoggyswamp.Value && RoR2.Run.instance)
            {
                if (RoR2.Run.instance.stageClearCount == 5 && RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswamp").filterOutOfBazaar != true)
                {
                    RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswamp").filterOutOfBazaar = true;
                    RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswampdownpour").filterOutOfBazaar = false;
                    Log.Debug("Bazaar filter values for foggyswamp and foggyswampdownpour swapped");
                }
            }

            //SwampMiniboss compatibility stuff. Set spawn point depending on scene name, and unhide the altar skeleton if the new scene is Downpour
            if (IsSwampMiniboss.enabled)
            {
                SwampMinibossCompat.SetSpawnPointValue(newScene.name);

                if (newScene.name == "foggyswampdownpour" || newScene.name == "itfoggyswampdownpour")
                {
                    GameObject didYouSeeItCommaCommaComma = GameObject.Find("HOLDER: Altar Skeleton/Skeleton").transform.GetChild(0).gameObject;
                    UnityEngine.Object.Destroy((UnityEngine.Object)(object)didYouSeeItCommaCommaComma.GetComponent<GameObjectUnlockableFilter>());
                    didYouSeeItCommaCommaComma.SetActive(true);
                }
            }

            if (newScene.name == "foggyswampdownpour")
            {
                GameObject ambience = GameObject.Find("SceneInfo/Ambience");
                AkBank bank = ambience.GetComponent<AkBank>();
                AkAmbient[] ambientList = ambience.GetComponents<AkAmbient>();
                AkAmbient ambient1 = ambientList[0];
                AkAmbient ambient2 = ambientList[1];
                if (bank)
                {
                    WwiseBankReference rainSound = Addressables.LoadAssetAsync<WwiseBankReference>("Wwise/CD00105A-AA3B-43F5-882A-C29812E886C8.asset").WaitForCompletion();
                    WwiseEventReference startRain = Addressables.LoadAssetAsync<WwiseEventReference>("Wwise/7B5141F8-EB05-455E-92EF-46A479D8612C.asset").WaitForCompletion();
                    WwiseEventReference stopSound = Addressables.LoadAssetAsync<WwiseEventReference>("Wwise/6F2ADD1C-BD55-431F-A62F-80CCD5F9631D.asset").WaitForCompletion();
                    bank.data.WwiseObjectReference = rainSound;
                    ambient1.data.WwiseObjectReference = startRain;
                    ambient2.data.WwiseObjectReference = stopSound;
                }

            }

            if (newScene.name == "itfoggyswampdownpour")
            {
                GameObject terrainObject = GameObject.Find("FSFloor/SubMesh_0");
                terrainObject.TryGetComponent(out MeshRenderer terrainRenderer);
                
                Material terrainMaterial = terrainRenderer.material;
                Material voidMaterial = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/GameModes/InfiniteTowerRun/ITAssets/matInfiniteTowerBooleanEffect1.mat").WaitForCompletion();
                
                Material[] materialList = { terrainMaterial, voidMaterial };
                
                terrainRenderer.SetSharedMaterials(materialList, 2);
            }
        }

        private void Destroy()
        {
            RoR2.Language.collectLanguageRootFolders -= CollectLanguageRootFolders;
        }

        private static void GiveToRoR2OurContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new ContentProvider());
        }

        public void CollectLanguageRootFolders(List<string> folders)
        {
            folders.Add(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(base.Info.Location), "Language"));
            folders.Add(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(base.Info.Location), "Plugins/Language"));
        }

        private void ConfigSetup()
        {
            loopVariant =
                base.Config.Bind<bool>("Settings",
                                       "Post-Loop Variant",
                                       true,
                                       "If true, Wetland Downpour behaves like a vanilla loop variant, entirely replacing Wetland Aspect after looping. If false, it does not replace Wetland Aspect and it can appear before or after looping, like a normal stage.");
            replaceFoggyswamp =
                base.Config.Bind<bool>("Settings",
                                       "Replace Wetland Aspect",
                                       false,
                                       "If true, Wetland Downpour will always replace Wetland Aspect, before and after looping, regardless of what value Loop Variant is set to.");
            regularEnabled =
                base.Config.Bind<bool>("Settings",
                                       "Enable Wetland Downpour",
                                       true,
                                       "If true, Wetland Downpour can appear in regular runs.");
            waterMuffle =
                base.Config.Bind<bool>("Settings",
                                       "Underwater Music Muffling",
                                       true,
                                       "If true, music will get muffled while the camera is underwater in Wetland Downpour.");
            simulacrumEnabled =
                base.Config.Bind<bool>("Settings - Simulacrum",
                                       "Enable Simulacrum Variant",
                                       true,
                                       "If true, Wetland Downpour can appear in the Simulacrum.");
            simulacrumStage1 =
                base.Config.Bind<bool>("Settings - Simulacrum",
                                       "Enable on Stage 1",
                                       true,
                                       "If true, Wetland Downpour can appear as the first stage in the Simulacrum. If false, it can only appear on stage 2 or higher, like Commencement.");
            toggleWayfarer =
                base.Config.Bind<bool>("Settings - Starstorm 2",
                                       "Wayfarer",
                                       true,
                                       "If true, Wayfarers can appear in Wetland Downpour.");
            toggleFollower =
                base.Config.Bind<bool>("Settings - Starstorm 2",
                                       "Follower",
                                       true,
                                       "If true, Followers can appear in Wetland Downpour.");
            toggleAcidBug =
                base.Config.Bind<bool>("Settings - Starstorm 2",
                                       "Archer Bug",
                                       true,
                                       "If true, Archer Bugs can appear in Wetland Downpour (after clearing 5 stages).");
            toggleLynxTotem =
                base.Config.Bind<bool>("Settings - EnemiesReturns",
                                       "Lynx Totem",
                                       true,
                                       "If true, Lynx Totems can appear in Wetland Downpour.");
            toggleLynxScout =
                base.Config.Bind<bool>("Settings - EnemiesReturns",
                                       "Lynx Scout",
                                       true,
                                       "If true, Lynx Scouts can appear in Wetland Downpour.");
            toggleLynxShaman =
                base.Config.Bind<bool>("Settings - EnemiesReturns",
                                       "Lynx Shaman",
                                       false,
                                       "If true, Lynx Shamans can appear in Wetland Downpour.");
            toggleSpitter =
                base.Config.Bind<bool>("Settings - EnemiesReturns",
                                       "Spitter",
                                       true,
                                       "If true, Spitters can appear in Wetland Downpour.");
            toggleArcherBugER =
                base.Config.Bind<bool>("Settings - EnemiesReturns",
                                       "Archer Bug",
                                       false,
                                       "If true, Archer Bugs can appear in Wetland Downpour (after clearing 5 stages).");
        }
    }
}
