using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.Reflection;
using UnityEngine;

namespace NervesOfDarkness
{
    public class NervesOfDarkness : ModBehaviour
    {
        public static NervesOfDarkness Instance;
        public INewHorizons NewHorizons;

        public void Awake()
        {
            Instance = this;
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.
        }

        public void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"My mod {nameof(NervesOfDarkness)} is loaded!", MessageType.Success);

            // Get the New Horizons API and load configs
            NewHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            NewHorizons.LoadConfigs(this);
            NewHorizons.GetBodyLoadedEvent().AddListener(OnBodyLoaded);

            new Harmony("AnonymousStrangerOW.NervesOfDarkness").PatchAll(Assembly.GetExecutingAssembly());

            // Example of accessing game code.
            OnCompleteSceneLoad(OWScene.TitleScreen, OWScene.TitleScreen); // We start on title screen
            LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;
        }

        private void OnBodyLoaded(string body)
        {
            if (body == "Divine Devourer")
            {
                var divineDevourer = NewHorizons.GetPlanet("Divine Devourer").transform;
                var ringRenderer = divineDevourer.Find("Sector/Ring").GetComponent<MeshRenderer>();
                ringRenderer.sharedMaterial.name = "DivineDevourerRing";
                ringRenderer.sharedMaterial.renderQueue = 3000;
                var blackHoleRenderer = divineDevourer.Find("Sector/BlackHole/BlackHoleRenderer").GetComponent<MeshRenderer>();
                blackHoleRenderer.sharedMaterial.name = "DivineDevourerBlackHole";
                blackHoleRenderer.sharedMaterial.shader = Shader.Find("Outer Wilds/Effects/Singularity");
                blackHoleRenderer.sharedMaterial.renderQueue = 3001;
            }
        }

        public void OnCompleteSceneLoad(OWScene previousScene, OWScene newScene)
        {
            if (newScene != OWScene.SolarSystem) return;
            ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
        }
    }

}
