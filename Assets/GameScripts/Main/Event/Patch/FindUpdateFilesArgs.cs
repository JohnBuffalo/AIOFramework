using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace AIOFramework
{
    public class FindUpdateFilesArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(FindUpdateFilesArgs).GetHashCode();

        public int TotalCount { get; private set; }
        public long TotalSizeBytes { get; private set; }
        public override int Id => EventId;

        public static FindUpdateFilesArgs Create(int totalCount, long totalSizeBytes)
        {
            var findUpdateFiles = ReferencePool.Acquire<FindUpdateFilesArgs>();
            findUpdateFiles.TotalCount = totalCount;
            findUpdateFiles.TotalSizeBytes = totalSizeBytes;
            return findUpdateFiles;
        }
        
        public override void Clear()
        {
            TotalCount = 0;
            TotalSizeBytes = 0;
        }

    }
}