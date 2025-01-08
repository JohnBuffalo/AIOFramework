using Cysharp.Threading.Tasks;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using YooAsset;

namespace AIOFramework
{
    public class ProcedureDownloadPackageFiles : AIOFramework.ProcedureBase
    {
        public override bool UseNativeDialog => true;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Entrance.Event.Fire(this, PatchStateChangeArgs.Create("BeginDownloadFiles-开始下载补丁文件"));
            BeginDownload(procedureOwner).Forget();
        }

        private async UniTaskVoid BeginDownload(ProcedureOwner procedureOwner)
        {
            await UniTask.WaitForSeconds(0.5f);

            ResourceDownloaderOperation downloader = procedureOwner.GetData<VarDownloader>("Downloader");
            downloader.OnDownloadErrorCallback += OnDownloadError;
            downloader.OnDownloadProgressCallback += OnDownloadProgress;
            downloader.BeginDownload();
            await downloader;

            if (downloader.Status != EOperationStatus.Succeed)
            {
                Log.Error("Download package files failed.");
            }

            ChangeState<ProcedureDownloadPackageFinish>(procedureOwner);
        }

        private void OnDownloadError(string name, string error)
        {
            Log.Error("Download error: {0}, {1}", name, error);
            Entrance.Event.Fire(this, DownloadFilesFailedArgs.Create(name, error));
        }

        private void OnDownloadProgress(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes,
            long currentDownloadBytes)
        {
            Log.Info("Download progress: {0}/{1}, {2}/{3}", currentDownloadCount, totalDownloadCount,
                currentDownloadBytes, totalDownloadBytes);
            var args = DownloadProgressArgs.Create(totalDownloadCount, currentDownloadCount, totalDownloadBytes,
                currentDownloadBytes);
            Entrance.Event.Fire(this, args);
        }
    }
}