using UnityEngine;
using YooAsset;

namespace AIOFramework
{
    public partial class Entrance
    {
        public static BuiltinDataComponent BuiltinData
        {
            get;
            private set;
        }
        
        void InitCustomComponents()
        {
            YooAssets.Initialize();
            // 注册自定义组件
            BuiltinData = UnityGameFramework.Runtime.GameEntry.GetComponent<BuiltinDataComponent>();
        }
        
        
    }
}