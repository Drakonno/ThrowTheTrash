using System; 
using BeatSaberMarkupLanguage.GameplaySetup;  
using ThrowTheTrash.Configuration;
using UnityEngine; 

namespace ThrowTheTrash
{ 
    public class ThrowTheTrashController : MonoBehaviour
    {
        public static ThrowTheTrashController Instance { get; private set; } 

        private MainMenuViewController _mainMenuViewController;
        private static int _currentMapsCount;

        public void TriggerGC()
        {
            _currentMapsCount = 0;

            Resources.UnloadUnusedAssets();

            var oldMode = UnityEngine.Scripting.GarbageCollector.GCMode;
            UnityEngine.Scripting.GarbageCollector.GCMode = UnityEngine.Scripting.GarbageCollector.Mode.Enabled;
            for (var i = 1; i <= PluginConfig.Instance.GCIterationsCount; i++)
            { 
                GC.Collect(); 
            }
            GC.WaitForPendingFinalizers();
            GC.Collect();
            UnityEngine.Scripting.GarbageCollector.GCMode = oldMode;

            _mainMenuViewController.UpdateRAMUsageValue();
        }

        private void Awake()
        { 
            if (Instance != null)
            {
                Plugin.Log?.Warn($"Instance of {GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this);
            Instance = this;
            Plugin.Log?.Debug($"{name}: Awake()");
        }

        private void Start()
        {
            Plugin.Log?.Debug($"{name}: Start()");
            _mainMenuViewController = new MainMenuViewController();
            PersistentSingleton<GameplaySetup>.instance.AddTab("ThrowTheTrash", "ThrowTheTrash.UI.BSML.MainMenuView.bsml", _mainMenuViewController, MenuType.Solo);
            BS_Utils.Utilities.BSEvents.LevelFinished += OnLevelFinished;
            BS_Utils.Utilities.BSEvents.menuSceneLoaded += OnMenuSceneLoaded;
            _mainMenuViewController.UpdateRAMUsageValue();
        }

        private void OnMenuSceneLoaded()
        {
            Plugin.Log?.Debug($"{name}: OnMenuSceneLoaded, current maps count: {_currentMapsCount}");
            if(_currentMapsCount >= PluginConfig.Instance.PlayedMapsCount && PluginConfig.Instance.TriggerGCAfterPlayedMaps)
            {
                TriggerGC(); 
            }
            else
            {
                _mainMenuViewController.UpdateRAMUsageValue();
            }
        }

        private void OnLevelFinished(object sender, BS_Utils.Utilities.LevelFinishedEventArgs e)
        {
            _currentMapsCount++;
            Plugin.Log?.Debug($"{name}: OnLevelFinished, current maps count: {_currentMapsCount}");
            _mainMenuViewController.UpdateRAMUsageValue();
        }

        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            BS_Utils.Utilities.BSEvents.LevelFinished -= OnLevelFinished;
            BS_Utils.Utilities.BSEvents.menuSceneLoaded -= OnMenuSceneLoaded;
            if (Instance == this)
            {
                Instance = null;
            }
        } 
    }
}
