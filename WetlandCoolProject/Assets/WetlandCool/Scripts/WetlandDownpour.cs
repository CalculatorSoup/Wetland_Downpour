using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using FSCStage.Content;
using HG;
using IL.RoR2;
using On.RoR2;
using R2API;
using R2API.AddressReferencedAssets;
using RoR2;
using RoR2.ContentManagement;
using RoR2BepInExPack.GameAssetPaths;
using RoR2BepInExPack.GameAssetPathsBetter;
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

        public const string Version = "0.1.0";

        public const string GUID = Author + "." + Name;

        public static FSCStage instance;

        public static ConfigEntry<bool> loopVariant;
        public static ConfigEntry<bool> replaceFoggyswamp;

        private void Awake()
        {
            instance = this;

            Log.Init(Logger);

            ConfigSetup();

            ContentManager.collectContentPackProviders += GiveToRoR2OurContentPackProviders;

            RoR2.Language.collectLanguageRootFolders += CollectLanguageRootFolders;

            On.RoR2.Run.PickNextStageScene += ReplaceWetlandAspect;

            RoR2.Run.onRunStartGlobal += InitializeBazaarSeerValues;

        }

// As far as I can tell, R2API / LoP / etc. don't really have any tools for easily / cleanly setting up a post-loop variant for a vanilla map.
// If they do, please tell me so I can replace all of this.
// But basically, I just do some hokey jank ass code instead.

// Wetland Downpour is registered via R2API.StageRegistration in WetlandDownpourContent, but given 0 weight so it can never appear naturally.
// After progressing through a certain number of stages, if the game picks Wetland Aspect as the next stage, I override it so it goes to Wetland Downpour instead.
// That only works for normally going through stages but *not* for the bazaar, so I also have it set the 'filterOutOfBazaar' values for both maps accordingly.
// Fun

public void InitializeBazaarSeerValues(RoR2.Run run)
        {
            if (FSCStage.loopVariant.Value && !FSCStage.replaceFoggyswamp.Value)
            {
                RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswamp").filterOutOfBazaar = false;
                RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswampdownpour").filterOutOfBazaar = true;
            } else if (FSCStage.replaceFoggyswamp.Value)
            {
                RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswamp").filterOutOfBazaar = true;
                RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswampdownpour").filterOutOfBazaar = false;
            }
        }
public void ReplaceWetlandAspect(On.RoR2.Run.orig_PickNextStageScene orig, RoR2.Run self, WeightedSelection<RoR2.SceneDef> choices)
        {
            orig.Invoke(self, choices);

            if (FSCStage.loopVariant.Value && !FSCStage.replaceFoggyswamp.Value)
            {
                /* if you are going to foggyswamp and more than 3 stages have been cleared, replace it with Wetland Downpour and remove it from Bazaar seer selection */
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
                /* if you are going to foggyswamp at any stage, replace it with Wetland Downpour */
                    if ((object)self.nextStageScene.baseSceneName != null && (self.nextStageScene.baseSceneName == "foggyswamp"))
                    {
                        self.nextStageScene = RoR2.SceneCatalog.GetSceneDefFromSceneName("foggyswampdownpour");

                        Log.Debug("Wetland Aspect annihilated by orbital missile; we did it, (read this in a cool voice)");
                    }
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
        }
    }
}
