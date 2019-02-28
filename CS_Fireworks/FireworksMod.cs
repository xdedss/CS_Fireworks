using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ICities;
using ColossalFramework;

namespace CS_Fireworks
{
    public class FireworksMod : IUserMod
    {
        public static string settingsfilename = "DynamicFireworksModSettings";
        
        public string Description
        {
            get
            {
                return "by sqrl";
            }
        }

        public string Name
        {
            get
            {
                return "Dynamic Fireworks!";
            }
        }
    }

    public class FireworksLoading : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            switch (mode)
            {
                case LoadMode.LoadGame:
                case LoadMode.NewGame:
                    if (FireworksManager.instance == null)
                    {
                        new GameObject("FireworksManager").AddComponent<FireworksManager>();
                    }
                    break;
            }
        }

        public override void OnLevelUnloading()
        {
            FireworksManager manager = FireworksManager.instance;
            if (manager != null)
            {
                UnityEngine.Object.Destroy(manager.gameObject);
            }
        }
    }
}
