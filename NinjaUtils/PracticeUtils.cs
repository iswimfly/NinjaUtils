using BepInEx;
using UnityEngine;

namespace PracticeUtils
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Utils : BaseUnityPlugin
    {
        public const string pluginGuid = "speedrunning.brc.practiceutils";
        public const string pluginName = "PracticeUtils";
        public const string pluginVersion = "0.0.3";

        private GameObject _mod;
        private PracticeGUI _practiceGUI;
        private PracticeCalls _practiceCalls;
        private PracticeFunction _practiceFunction;
        private PracticeUpdater _practiceUpdater;
        private TriggerTools _practiceTools;
        private void Awake()
        {
            _practiceCalls = new PracticeCalls();
            _practiceFunction = new PracticeFunction();
            _practiceUpdater = new PracticeUpdater();
            _practiceTools = new TriggerTools();
            _practiceGUI = new PracticeGUI();

            _mod = new GameObject();
            _mod.AddComponent<PracticeCalls>();
            _mod.AddComponent<PracticeFunction>();
            _mod.AddComponent<PracticeUpdater>();
            _mod.AddComponent<TriggerTools>();
            _mod.AddComponent<PracticeGUI>();
            GameObject.DontDestroyOnLoad(_mod);
        }
    }
}