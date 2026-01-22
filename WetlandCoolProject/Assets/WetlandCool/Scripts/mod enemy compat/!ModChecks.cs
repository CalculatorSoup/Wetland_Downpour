using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.AddressableAssets;
using static R2API.DirectorAPI;

namespace FSCStage
{
    public class IsStarstorm2
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm.MSU") && BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm"));
                }
                return (bool)_enabled;
            }
        }
    }
    public class IsEnemiesReturns
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Viliger.EnemiesReturns");
                }
                return (bool)_enabled;
            }
        }
    }
}