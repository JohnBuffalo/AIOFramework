using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace AIOFramework
{
    public class ProcedureUpdateDone : AIOFramework.ProcedureBase
    {
        public override bool UseNativeDialog => false;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Info("Update Done");
            Entrance.Event.Fire(this, PatchStateChangeArgs.Create("Update Done"));
        }
    }
}