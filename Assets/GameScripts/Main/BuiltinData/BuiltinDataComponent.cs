using UnityGameFramework.Runtime;
using GameFramework;
using UnityEngine;

namespace AIOFramework
{
    public class BuiltinDataComponent : GameFrameworkComponent
    {
        private BuildInfo m_BuildInfo = null;

        public BuildInfo BuildInfo
        {
            get
            {
                return m_BuildInfo;
            }
        }
        
        public void InitBuildInfo()
        {
            var settings = SettingsUtils.GlobalSettings;
            var assetSettings = settings.FrameworkGlobalSettings;
            var scriptsSettings = settings.HybridClrCustomGlobalSettings;
        }
        
        public void InitDefaultDictionary()
        {
            Log.Warning("Todo InitDefaultDictionary");
        }
    }
}