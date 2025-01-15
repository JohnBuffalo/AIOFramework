using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameFramework;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace AIOFramework
{
    public class ProcedureInitPackage : AIOFramework.ProcedureBase
    {
        // 指示是否使用原生对话框
        public override bool UseNativeDialog => true;

        // 当进入该流程时调用
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Entrance.Event.Fire(this, PatchStateChangeArgs.Create("InitPackage-初始化Package"));
            InitPackage(procedureOwner).Forget();
        }

        private string GetHostServerURL()
        {
            var settings = SettingsUtils.GlobalSettings.FrameworkGlobalSettings;
            var version = settings.ScriptVersion;
            string url = "http://127.0.0.1";
            string platFormName = "Android";

#if UNITY_EDITOR
            switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget)
            {
                case UnityEditor.BuildTarget.Android:
                    url = settings.AndroidAppUrl;
                    platFormName = "Android";
                    break;
                case UnityEditor.BuildTarget.iOS:
                    url = settings.IOSAppUrl;
                    platFormName = "IOS";
                    break;
                case UnityEditor.BuildTarget.StandaloneWindows64:
                    url = settings.WindowsAppUrl;
                    platFormName = "PC";
                    break;
                case UnityEditor.BuildTarget.StandaloneOSX:
                    url = settings.MacOSAppUrl;
                    platFormName = "MacOS";
                    break;
                case UnityEditor.BuildTarget.StandaloneWindows:
                    url = settings.WindowsAppUrl;
                    platFormName = "PC";
                    break;
            }
#else
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    url = settings.AndroidAppUrl;
                    platFormName = "Android";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    url = settings.IOSAppUrl;
                    platFormName = "IOS";
                    break;
                case RuntimePlatform.WindowsPlayer:
                    url = settings.WindowsAppUrl;
                    platFormName = "PC";
                    break;
            }

#endif
            return $"{url}/CDN/{platFormName}/{version}";
        }

        // 初始化资源包的方法
        private async UniTaskVoid InitPackage(ProcedureOwner procedureOwner)
        {
            // 获取当前的播放模式和包名
            var playMode = Entrance.Resource.PlayMode;
            var packageName = Entrance.Resource.PackageName;
            
            procedureOwner.SetData<VarInt32>("PlayMode", (int)playMode);
            procedureOwner.SetData<VarString>("PackageName", packageName);
            
            Log.Info($"InitPackage , playMode : {playMode}, packageName : {packageName}");

            // 尝试获取现有的资源包，如果不存在则创建一个新的
            var package = YooAssets.TryGetPackage(packageName);
            if (package == null)
            {
                package = YooAssets.CreatePackage(packageName);
            }
            YooAssets.SetDefaultPackage(package);
            InitializationOperation initializationOperation = null;
            InitializeParameters initParameters = null;
            // 根据播放模式进行初始化
            if (playMode == EPlayMode.EditorSimulateMode)
            {
                // 创建模拟构建参数
                var simulateBuildParam = new EditorSimulateBuildParam(packageName);
                // 执行模拟构建并获取结果
                var simulateBuildResult = EditorSimulateModeHelper.SimulateBuild(simulateBuildParam);
                var packageRoot = simulateBuildResult.PackageRootDirectory;
                // 创建编辑器模拟模式参数
                var createParameters = new EditorSimulateModeParameters();
                createParameters.EditorFileSystemParameters =
                    FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
                initParameters = createParameters;
            }

            //单机模式运行
            if (playMode == EPlayMode.OfflinePlayMode)
            {
                var createParameters = new OfflinePlayModeParameters();
                createParameters.BuildinFileSystemParameters =
                    FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                initParameters = createParameters;
            }

            // 网络模式运行
            if (playMode == EPlayMode.HostPlayMode)
            {
                string defaultHostServer = GetHostServerURL();
                string fallbackHostServer = GetHostServerURL();
                Log.Info("defaultHostServer: {0}", defaultHostServer);
                Log.Info("fallbackHostServer: {0}", defaultHostServer);
                IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                var createParameters = new HostPlayModeParameters();
                createParameters.BuildinFileSystemParameters =
                    FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                createParameters.CacheFileSystemParameters =
                    FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
                initParameters = createParameters;
            }

            initializationOperation = package.InitializeAsync(initParameters);
            await initializationOperation.ToUniTask();
            
            if (initializationOperation.Status != EOperationStatus.Succeed)
            {
                Log.Warning($"{initializationOperation.Error}");
                Entrance.Event.Fire(this, InitPackageFailedArgs.Create());
            }
            ChangeState<ProcedureUpdatePackageVersion>(procedureOwner);
        }
    }
}