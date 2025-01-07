using GameFramework;
using GameFramework.Event;

namespace AIOFramework
{
    public class InitPackageFailedArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(InitPackageFailedArgs).GetHashCode();

        public static InitPackageFailedArgs Create()
        {
            var args = ReferencePool.Acquire<InitPackageFailedArgs>();
            return args;
        }
        
        public override void Clear()
        {
            
        }

        public override int Id =>EventId;
    }
}