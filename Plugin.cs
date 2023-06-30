using IPA;
using IPA.Config;
using IPA.Config.Stores; 
using UnityEngine; 
using IPALogger = IPA.Logging.Logger;

namespace ThrowTheTrash
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        private ThrowTheTrashController _throwTheTrashController;

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger)
        {
            Instance = this;
            Log = logger;
            Log.Info("ThrowTheTrash initialized.");
        }

        #region BSIPA Config

        [Init]
        public void InitWithConfig(Config configuration)
        {
            Configuration.PluginConfig.Instance = GeneratedStore.Generated<Configuration.PluginConfig>(configuration, true);
            Log.Debug("Config loaded");
        }

        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");
            _throwTheTrashController = new GameObject("ThrowTheTrashController").AddComponent<ThrowTheTrashController>();

        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit");

        }
    }
}
