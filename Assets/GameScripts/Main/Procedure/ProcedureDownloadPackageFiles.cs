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
            downloader.DownloadErrorCallback += OnDownloadError;
            downloader.DownloadUpdateCallback += OnDownloadProgress;
            downloader.BeginDownload();
            await downloader;

            if (downloader.Status != EOperationStatus.Succeed)
            {
                Log.Error("Download package files failed.");
            }

            ChangeState<ProcedureDownloadPackageFinish>(procedureOwner);
        }

        private void OnDownloadError(DownloadErrorData data)
        {
            Log.Error("Download error: Package:{0}, File:{1}, Error:{2}", data.PackageName, data.FileName, data.ErrorInfo);
            Entrance.Event.Fire(this, DownloadFilesFailedArgs.Create(data.PackageName,data.FileName, data.ErrorInfo));
        }

        private void OnDownloadProgress(DownloadUpdateData data)
        {
            var currentDownloadCount = data.CurrentDownloadCount;
            var totalDownloadCount = data.TotalDownloadCount;
            var currentDownloadBytes = data.CurrentDownloadBytes;
            var totalDownloadBytes = data.TotalDownloadBytes;
            Log.Info("Download progress: {0}/{1}, {2}/{3}", currentDownloadCount, totalDownloadCount,
                currentDownloadBytes, totalDownloadBytes);
            var args = DownloadProgressArgs.Create(totalDownloadCount, currentDownloadCount, totalDownloadBytes,
                currentDownloadBytes);
            Entrance.Event.Fire(this, args);
        }
    }
}