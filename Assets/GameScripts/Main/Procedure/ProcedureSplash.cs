using GameFramework.Fsm;
using GameFramework.Procedure;
using GameFramework.Resource;
using UnityGameFramework.Runtime;

namespace AIOFramework
{
    public class ProcedureSplash : AIOFramework.ProcedureBase
    {
        public override bool UseNativeDialog => true;
        private bool m_SplashFinished = false;
        private bool m_SplashStarted = false;
        
        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, 
            float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!m_SplashStarted)
            {
                PlaySplash();
            }
            
            if (!m_SplashFinished)
            {
                return;
            }
            
            if(Entrance.Base.EditorResourceMode)
            {
                // ChangeState<ProcedurePreload>(procedureOwner);
            }
            else if(Entrance.Resource.ResourceMode == ResourceMode.Package)
            {
                // ChangeState<ProcedureInitResources>(procedureOwner);
            }
            else
            {
                //热更新
                ChangeState<ProcedureInitPackage>(procedureOwner);
            }
            
        }

        private void PlaySplash()
        {
            m_SplashStarted = true;
            Log.Info("Todo Play Splash Logo");
            m_SplashFinished = true;
        }
    }
}