using GameFramework;
using GameFramework.Event;

namespace AIOFramework
{
    public class DownloadProgressArgs : GameEventArgs
    {
        public static readonly int EventID = typeof(DownloadProgressArgs).GetHashCode();
        public override int Id => EventID;

        public int TotalDownloadCount { get; private set; }
        public int CurrentDownloadCount { get; private set; }
        public long TotalDownloadSizeBytes { get; private set; }
        public long CurrentDownloadSizeBytes { get; private set; }

        public override void Clear()
        {
            TotalDownloadSizeBytes = 0;
            CurrentDownloadCount = 0;
            TotalDownloadCount = 0;
            CurrentDownloadSizeBytes = 0;
        }
        
        public static DownloadProgressArgs Create(int totalDownloadCount, int currentDownloadCount, long totalDownloadSizeBytes, long currentDownloadSizeBytes)
        {
            var args = ReferencePool.Acquire<DownloadProgressArgs>();
            args.TotalDownloadCount = totalDownloadCount;
            args.CurrentDownloadCount = currentDownloadCount;
            args.TotalDownloadSizeBytes = totalDownloadSizeBytes;
            args.CurrentDownloadSizeBytes = currentDownloadSizeBytes;
            return args;
        }
    }
}