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
        
        //Todo 切换成YooAsset版本管理信息模板
        public void InitBuildInfo()
        {
            Log.Error("Todo 切换成YooAsset版本管理信息模板");
        }
        
        public void InitDefaultDictionary()
        {
            Log.Error("Todo 切换成YooAsset包体资源信息");
        }
    }
}