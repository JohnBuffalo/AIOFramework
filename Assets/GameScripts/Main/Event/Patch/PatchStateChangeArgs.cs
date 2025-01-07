using GameFramework;
using GameFramework.Event;

namespace AIOFramework
{
    public class PatchStateChangeArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(PatchStateChangeArgs).GetHashCode();

        public override int Id => EventId;

        public string Tips { get; private set; }

        public static PatchStateChangeArgs Create(string tips)
        {
            var args = ReferencePool.Acquire<PatchStateChangeArgs>();
            args.Tips = tips;
            return args;
        }
        
        public override void Clear()
        {
            Tips = null;
        }
    }
}