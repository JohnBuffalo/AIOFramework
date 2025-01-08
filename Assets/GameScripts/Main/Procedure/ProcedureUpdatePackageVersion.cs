using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace AIOFramework
{
    public class ProcedureUpdatePackageVersion : AIOFramework.ProcedureBase
    {
        public override bool UseNativeDialog => true;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Entrance.Event.Fire(this, PatchStateChangeArgs.Create("UpdatePackageVersion-获取最新版本"));
            UpdatePackageVersion(procedureOwner).Forget();
        }
        
        private async UniTaskVoid UpdatePackageVersion(ProcedureOwner procedureOwner)
        {
            await UniTask.WaitForSeconds(0.5f);
            var packageName = Entrance.Resource.PackageName;
            var package = YooAssets.GetPackage(packageName);
            var operation = package.RequestPackageVersionAsync();
            Log.Info($"UpdatePackageVersion, packageName : {packageName}");

            await operation.ToUniTask();

            if (operation.Status != EOperationStatus.Succeed)
            {
                Log.Warning(operation.Error);
                Entrance.Event.Fire(this, InitPackageFailedArgs.Create());
            }
            else
            {
                Log.Info($"Request package version : {operation.PackageVersion}");
                procedureOwner.SetData<VarString>("PackageVersion", operation.PackageVersion);
                Entrance.Event.Fire(this, PackageVersionArgs.Create(operation.PackageVersion));
                ChangeState<ProcedureUpdatePackageManifest>(procedureOwner);
            }
        }
    }
}


