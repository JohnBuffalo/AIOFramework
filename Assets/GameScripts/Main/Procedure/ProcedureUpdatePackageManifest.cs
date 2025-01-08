using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using YooAsset;

namespace AIOFramework
{
    public class ProcedureUpdatePackageManifest : AIOFramework.ProcedureBase
    {
        public override bool UseNativeDialog => true;


        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Entrance.Event.Fire(this, PatchStateChangeArgs.Create("UpdatePackageManifest-更新资源清单"));
            UpdateManifest(procedureOwner).Forget();
        }

        private async UniTaskVoid UpdateManifest(ProcedureOwner procedureOwner)
        {
            await UniTask.WaitForSeconds(0.5f);

            var packageName = procedureOwner.GetData<VarString>("PackageName");
            var packageVersion = procedureOwner.GetData<VarString>("PackageVersion");
            var package = YooAssets.GetPackage(packageName);
            var operation = package.UpdatePackageManifestAsync(packageVersion);
            await operation.ToUniTask();

            if (operation.Status != EOperationStatus.Succeed)
            {
                Log.Warning(operation.Error);
                Entrance.Event.Fire(this, InitPackageFailedArgs.Create());
            }
            else
            {
                Log.Info("UpdateManifest Succeed");
                ChangeState<ProcedurePackageDownloader>(procedureOwner);
            }
        }
    } 
}

