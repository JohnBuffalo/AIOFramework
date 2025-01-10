using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;


namespace AIOFramework
{
    public class ProcedureLoadAssembly : AIOFramework.ProcedureBase
    {
        public override bool UseNativeDialog { get; }

        private string hotUpdateLocationPrefix = "HotUpdate_";
        
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Info("ProcedureLoadAssembly OnEnter");
            
            LoadAssembly("HotUpdate").Forget();
            
            LoadGameEntrance().Forget();
        }
        
        private async UniTask<Assembly> LoadAssembly(string assemblyName)
        {

            var handler = YooAssets.LoadAssetAsync<TextAsset>($"{hotUpdateLocationPrefix}{assemblyName}.dll");
            await handler.ToUniTask();
            
            if (handler.Status != EOperationStatus.Succeed)
            {
                Log.Fatal($"Failed to load assembly: {assemblyName}");
                return null;
            }
            
            if (handler.AssetObject == null)
            {
                Log.Fatal($"Failed to load assembly: {assemblyName}");
                return null;
            }

            TextAsset textAsset = handler.AssetObject as TextAsset;
            if (textAsset == null)
            {
                Log.Fatal($"Failed to load assembly: {assemblyName}");
                return null;
            }
            byte[] assemblyData = textAsset.bytes;
            Assembly assembly = Assembly.Load(assemblyData);
            Log.Info($"Load assembly: {assembly.FullName} success ");
            return assembly;
        }

        private async UniTaskVoid LoadGameEntrance()
        {
            var entrance = YooAssets.LoadAssetAsync<GameObject>($"{hotUpdateLocationPrefix}GameEntrance");
            await entrance.ToUniTask();
            
            var gameEntrance = GameObject.Instantiate(entrance.AssetObject,Vector3.zero,Quaternion.identity);
            GameObject.DontDestroyOnLoad(gameEntrance);            
        }
    }
}