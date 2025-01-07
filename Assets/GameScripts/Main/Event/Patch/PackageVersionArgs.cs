using GameFramework;
using GameFramework.Event;

namespace AIOFramework
{
    public class PackageVersionArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(PackageVersionArgs).GetHashCode();
        public override int Id => EventId;

        public string PackageVersion { get; private set; }


        public static PackageVersionArgs Create(string version)
        {
            var args = ReferencePool.Acquire<PackageVersionArgs>();
            args.PackageVersion = version;
            return args;
        }
        public override void Clear()
        {
            PackageVersion = null;
        }

    }
}