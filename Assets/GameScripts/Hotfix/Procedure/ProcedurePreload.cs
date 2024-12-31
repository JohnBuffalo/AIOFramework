using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace AIOFramework
{
    public class ProcedurePreload : AIOFramework.ProcedureBase
    {
        public override bool UseNativeDialog => true;
    }
}