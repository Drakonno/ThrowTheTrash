using System; 
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings; 
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers; 
using ThrowTheTrash.Configuration; 

namespace ThrowTheTrash
{ 
    public class MainMenuViewController : BSMLResourceViewController
    { 
        public override string ResourceName => "ThrowTheTrash.UI.BSML.MainMenuView.bsml";

        [UIParams]
        private BSMLParserParams BSMLParserParams;

        public void UpdateRAMUsageValue()
        {
            long usedMemoryBytes = GC.GetTotalMemory(true);
            _currentGCRAMUsage = usedMemoryBytes / 1024 / 1024;
            _availableRAM = PerformanceUtilities.GetPhysicalAvailableMemoryInMiB();
            _systemRAM = PerformanceUtilities.GetTotalMemoryInMiB();
            _GCRAMDelta = _lastGCRAMUsage - _currentGCRAMUsage;
            this.NotifyPropertyChanged("SystemRAM");
            this.NotifyPropertyChanged("CurrentGCRAMUsage");
            this.NotifyPropertyChanged("AvailableRAM");
            this.NotifyPropertyChanged("GCRAMDelta");
            Plugin.Log?.Debug($"UpdateRAMUsageValue: ({CurrentGCRAMUsage}/{AvailableRAM}/{SystemRAM})[MB]");
            Plugin.Log?.Debug($"Differenece from last update: ({_GCRAMDelta})[MB]"); 
            _lastGCRAMUsage = _currentGCRAMUsage;
        }

        #region RAM
        [UIValue("CurrentGCRAMUsage")]
        private string CurrentGCRAMUsage => $"{_currentGCRAMUsage}";

        [UIValue("AvailableRAM")]
        private string AvailableRAM => $"{_availableRAM}";

        [UIValue("SystemRAM")]
        private string SystemRAM => $"{_systemRAM}";

        [UIValue("GCRAMDelta")]
        private string GCRAMDelta => _GCRAMDelta > 0 ? $"Freed {Math.Abs(_GCRAMDelta)}" : $"Allocated {Math.Abs(_GCRAMDelta)}";

        private long _currentGCRAMUsage = 0;
        private long _lastGCRAMUsage = 0;
        private long _GCRAMDelta = 0;
        private long _availableRAM = 0;
        private long _systemRAM = 0;
        #endregion RAMU

        #region PlayedMapsCount
        [UIValue("PlayedMapsCountSlider")]
        private SliderSetting PlayedMapsCountSlider;

        [UIValue("PlayedMapsCountValue")]
        public float PlayedMapsCountValue
        {
            get
            {
                return (float)PluginConfig.Instance.PlayedMapsCount;
            }
            set
            {
                PluginConfig.Instance.PlayedMapsCount = (int)value;
            }
        }

        [UIAction("SetPlayedMapsCountValue")]
        private void SetPlayedMapsCountValue(float value)
        { 
            PlayedMapsCountValue = (int)value;
            PluginConfig.Instance.Changed();
        }
        #endregion PlayedMapsCount

        #region TriggerGCButton
        [UIAction("TriggerGCButtonClicked")]
        private void TriggerGCButtonClicked()
        {
            ThrowTheTrashController.Instance.TriggerGC();
        }
        #endregion TriggerGCButton

        #region TriggerGCAfterPlayedMaps
        [UIValue("TriggerGCAfterPlayedMaps")]
        private bool TriggerGCAfterPlayedMaps
        {
            get
            {
                return PluginConfig.Instance.TriggerGCAfterPlayedMaps;
            }
            set
            {
                PluginConfig.Instance.TriggerGCAfterPlayedMaps = value;
            }
        }

        [UIAction("SetTriggerGCAfterPlayedMaps")]
        private void SetTriggerGCAfterPlayedMaps(bool value)
        {
            TriggerGCAfterPlayedMaps = value;
            PluginConfig.Instance.Changed();
        }
        #endregion TriggerGCAfterPlayedMaps 
    }
}
