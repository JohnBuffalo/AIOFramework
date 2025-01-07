using GameFramework;
using GameFramework.Event;

namespace AIOFramework
{
    public class BeginDownloadUpdateFilesArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(BeginDownloadUpdateFilesArgs).GetHashCode();
        public override int Id => EventId;

        public static BeginDownloadUpdateFilesArgs Create()
        {
            BeginDownloadUpdateFilesArgs args = ReferencePool.Acquire<BeginDownloadUpdateFilesArgs>();
            return args;
        }
        
        public override void Clear()
        {
        }
    }
}