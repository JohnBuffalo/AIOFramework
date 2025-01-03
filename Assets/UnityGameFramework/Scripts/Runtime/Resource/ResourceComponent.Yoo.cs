using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace UnityGameFramework.Runtime
{
    public partial class ResourceComponent : GameFrameworkComponent
    {
        [SerializeField] private EPlayMode playMode = EPlayMode.EditorSimulateMode;

        public string PackageName = "Default";
        
        public EPlayMode PlayMode => playMode;
        
        
    }
}