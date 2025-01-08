using GameFramework;
using GameFramework.Event;

namespace AIOFramework
{
    public class DownloadFilesFailedArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(DownloadFilesFailedArgs).GetHashCode();
        public override int Id => EventId;

        public string FileName { get; private set; }
        public string Error { get; private set; }
        public string PackageName { get; private set; }
        
        public static DownloadFilesFailedArgs Create(string package, string fileName, string error)
        {
            var args = ReferencePool.Acquire<DownloadFilesFailedArgs>();
            args.FileName = fileName;
            args.Error = error;
            args.PackageName = package;
            return args;
        }
        
        public override void Clear()
        {
            FileName = null;
            Error = null;
        }
    }
}