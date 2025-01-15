using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace AIOFramework
{
    public class ProcedureStartGame : AIOFramework.ProcedureBase
    {
        public override bool UseNativeDialog { get; }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            LoadGameEntrance().Forget();
        }
        
        private async UniTask LoadGameEntrance()
        {
            var entrance = YooAssets.LoadAssetAsync<GameObject>($"{Constant.HotUpdate.HotUpdateLocationPrefix}GameEntrance");
            await entrance.ToUniTask();

            var gameEntrance = GameObject.Instantiate(entrance.AssetObject, Vector3.zero, Quaternion.identity);
            GameObject.DontDestroyOnLoad(gameEntrance);
        }
    }
}