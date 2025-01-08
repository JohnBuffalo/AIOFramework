using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace AIOFramework
{
    public class ProcedurePackageDownloader : AIOFramework.ProcedureBase
    {
        public override bool UseNativeDialog => true;
        ProcedureOwner procedureOwner;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            AddListeners();
            this.procedureOwner = procedureOwner;
            Entrance.Event.Fire(this, PatchStateChangeArgs.Create("CreatePackageDownloader-创建补丁下载器"));
            CreateDownloader(procedureOwner).Forget();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            RemoveListeners();
        }

        private void AddListeners()
        {
            Entrance.Event.Subscribe(BeginDownloadUpdateFilesArgs.EventId, OnBeginDownloadUpdateFiles);
        }
        
        private void RemoveListeners()
        {
            Entrance.Event.Unsubscribe(BeginDownloadUpdateFilesArgs.EventId, OnBeginDownloadUpdateFiles);
        }

        private void OnBeginDownloadUpdateFiles(object sender, GameEventArgs e)
        {
            ChangeState<ProcedureDownloadPackageFiles>(procedureOwner);
        }
        
        private async UniTaskVoid CreateDownloader(ProcedureOwner procedureOwner)
        {
            await UniTask.WaitForSeconds(1f);

            var packageName = procedureOwner.GetData<VarString>("PackageName");
            var package = YooAssets.GetPackage(packageName);
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
            procedureOwner.SetData<VarDownloader>("Downloader", downloader);

            if (downloader.TotalDownloadCount == 0)
            {
                Log.Warning("Not Find any download files");
                ChangeState<ProcedureUpdateDone>(procedureOwner);
            }
            else
            {
                int totalDownloadCount = downloader.TotalDownloadCount;
                long totalDownloadBytes = downloader.TotalDownloadBytes;
                Entrance.Event.Fire(this, FindUpdateFilesArgs.Create(totalDownloadCount, totalDownloadBytes));
                // CheckDiskSpace(totalDownloadBytes); 
            }
        }

        private void CheckDiskSpace(long spaceNeeded)
        {
            long leftSpace = 0L;
#if UNITY_STANDALONE_WIN
            leftSpace = PCSpace();
#elif UNITY_ANDROID
            leftSpace = AndroidSpace();
#elif UNITY_IOS
            leftSpace = IOSSpace();
#endif
            Log.Warning($"Todo CheckDiskSpace need:{spaceNeeded} , left:{leftSpace}");
        }

        private long PCSpace()
        {
            string gamePath = Application.dataPath;
            string driveLetter = Path.GetPathRoot(gamePath);
            DriveInfo driveInfo = new DriveInfo(driveLetter);
            long availableSpaceInBytes = driveInfo.AvailableFreeSpace; //IL2CPP不支持
            return availableSpaceInBytes;
        }

        private long AndroidSpace()
        {
            return 0L;
        }

        private long IOSSpace()
        {
            return 0L;
        }
    }
}