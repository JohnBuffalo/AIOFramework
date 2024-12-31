using UnityEngine;

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
            // 注册自定义组件
            BuiltinData = UnityGameFramework.Runtime.GameEntry.GetComponent<BuiltinDataComponent>();
        }
        
        
    }
}