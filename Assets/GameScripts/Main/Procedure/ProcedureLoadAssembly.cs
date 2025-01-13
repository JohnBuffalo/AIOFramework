using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using HybridCLR;
using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;


namespace AIOFramework
{
    public class ProcedureLoadAssembly : AIOFramework.ProcedureBase
    {
        public override bool UseNativeDialog { get; }

        private const string HotUpdateLocationPrefix = "HotUpdate_";

        private static Dictionary<string, byte[]> s_assetDatas = new Dictionary<string, byte[]>();
        private const string AssemblyFileSuffix = ".dll.bytes";

        private static List<string> AOTMetaAssemblyFiles { get; } = new List<string>()
        {
            "mscorlib",
            "System",
            "System.Core",
        };

        private static List<string> HotUpdateAssemblyFiles { get; } = new List<string>()
        {
            "HotUpdate",
        };

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Info("ProcedureLoadAssembly OnEnter");
            _ = LoadAssemblies();
        }

        private byte[] ReadBytesFromCache(string dllName)
        {
            return s_assetDatas[dllName];
        }

        private async UniTask LoadAssemblies()
        {
            await CacheAssembliesBytes();
            Log.Info("CacheAssembliesBytes Finish");
            await LoadMetadataForAOTAssemblies();
            Log.Info("LoadMetadataForAOTAssemblies Finish");
            await LoadHotUpdateAssembly();
            Log.Info("LoadHotUpdateAssembly Finish");
            await LoadGameEntrance();
            Log.Info("LoadGameEntrance Finish");
        }

        private async UniTask CacheAssembliesBytes()
        {
            var totalFileNames = HotUpdateAssemblyFiles.Concat(AOTMetaAssemblyFiles);
            var tasks = new List<UniTask>();

            foreach (var fileName in totalFileNames)
            {
                tasks.Add(LoadAssemblyBytes(fileName));
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask LoadAssemblyBytes(string fileName)
        {
            string location = $"{HotUpdateLocationPrefix}{fileName}.dll";
            var handler = YooAssets.LoadAssetAsync<TextAsset>(location);
            await handler.ToUniTask();
            if (handler.Status != EOperationStatus.Succeed)
            {
                Log.Fatal($"Failed to load assembly: {fileName}");
                return;
            }

            if (handler.AssetObject == null)
            {
                Log.Fatal($"Failed to load assembly: {fileName}");
                return;
            }

            TextAsset textAsset = handler.AssetObject as TextAsset;
            if (textAsset == null)
            {
                Log.Fatal($"Failed to load assembly: {fileName}");
                return;
            }

            s_assetDatas.Add(fileName, textAsset.bytes);
            Log.Info($"Load {fileName}.dll.bytes success");
        }

        private async UniTask LoadMetadataForAOTAssemblies()
        {
            HomologousImageMode mode = HomologousImageMode.SuperSet;
            foreach (var aotDllName in AOTMetaAssemblyFiles)
            {
                byte[] dllBytes = ReadBytesFromCache(aotDllName);
                // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
                Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
            }
        }

        private async UniTask LoadHotUpdateAssembly()
        {
            Assembly assembly;
#if !UNITY_EDITOR
            byte[] assemblyData = ReadBytesFromCache("HotUpdate");
            assembly = Assembly.Load(assemblyData);
#else
            assembly = System.AppDomain.CurrentDomain.GetAssemblies().First(a=>a.GetName().Name == "HotUpdate");
#endif
            Log.Info($"Load assembly: {assembly.GetName()} success ");

            Type entryType = assembly.GetType("AIOFramework.GameEntrance"); //必须带上命名空间,不然找不到
            if (entryType == null)
            {
                Log.Warning("Can't find GameEntrance in assembly");
            }
            else
            {
                MethodInfo method = entryType.GetMethod("Main",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                method.Invoke(null, null);
            }
        }

        private async UniTask LoadGameEntrance()
        {
            var entrance = YooAssets.LoadAssetAsync<GameObject>($"{HotUpdateLocationPrefix}GameEntrance");
            await entrance.ToUniTask();

            var gameEntrance = GameObject.Instantiate(entrance.AssetObject, Vector3.zero, Quaternion.identity);
            GameObject.DontDestroyOnLoad(gameEntrance);
        }
    }
}