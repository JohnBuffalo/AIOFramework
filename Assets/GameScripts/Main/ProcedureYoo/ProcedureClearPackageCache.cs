using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace AIOFramework
{
    public class ProcedureClearPackageCache : AIOFramework.ProcedureBase
    {
        public override bool UseNativeDialog => false;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Info("ClearUnusedPackageCache-清理未使用的缓存文件");
            Entrance.Event.Fire(this, PatchStateChangeArgs.Create("ClearUnusedPackageCache-清理未使用的缓存文件"));

            ClearCache(procedureOwner).Forget();
        }


        private async UniTaskVoid ClearCache(ProcedureOwner procedureOwner)
        {
            await UniTask.WaitForSeconds(0.5f);
            
            var packageName = procedureOwner.GetData<VarString>("PackageName");
            var package = YooAssets.GetPackage(packageName);
            var operation = package.ClearCacheBundleFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
            await operation;
            Log.Info("ClearUnusedBundleFiles Completed");
            ChangeState<ProcedureUpdateDone>(procedureOwner);
        }

    } 
}


