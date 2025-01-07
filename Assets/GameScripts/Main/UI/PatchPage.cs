using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace AIOFramework
{
    public class PatchPage : MonoBehaviour
    {
        private class MessageBox
        {
            private GameObject _cloneObject;
            private TextMeshProUGUI _content;
            private Button _btnOK;
            private System.Action _clickOK;

            public bool ActiveSelf
            {
                get { return _cloneObject.activeSelf; }
            }

            public void Create(GameObject cloneObject)
            {
                _cloneObject = cloneObject;
                _content = cloneObject.transform.Find("Content").GetComponent<TextMeshProUGUI>();
                _btnOK = cloneObject.transform.Find("Button").GetComponent<Button>();
                _btnOK.onClick.AddListener(OnClickYes);
            }

            public void Show(string content, System.Action clickOK)
            {
                _content.text = content;
                _clickOK = clickOK;
                _cloneObject.SetActive(true);
                _cloneObject.transform.SetAsLastSibling();
            }

            public void Hide()
            {
                _content.text = string.Empty;
                _clickOK = null;
                _cloneObject.SetActive(false);
            }

            private void OnClickYes()
            {
                _clickOK?.Invoke();
                Hide();
            }
        }

        private GameObject _messageBoxObj;
        private Slider slider;
        private TextMeshProUGUI ver_txt;
        private TextMeshProUGUI info_txt;
        private readonly List<MessageBox> _msgBoxList = new List<MessageBox>();

        private void Awake()
        {
            BindComponents();
            AddListeners();
        }

        private void OnDestroy()
        {
            RemoveAllListeners();
        }

        private void BindComponents()
        {
            slider = transform.Find("Progress").GetComponent<Slider>();
            ver_txt = transform.Find("Version").GetComponent<TextMeshProUGUI>();
            info_txt = transform.Find("Info").GetComponent<TextMeshProUGUI>();
            info_txt.text = "AIO Launch";
            _messageBoxObj = transform.Find("MessageBox").gameObject;
            _messageBoxObj.SetActive(false);
        }

        private void AddListeners()
        {
            Entrance.Event.Subscribe(PatchStateChangeArgs.EventId, OnPatchStateChange);
            Entrance.Event.Subscribe(FindUpdateFilesArgs.EventId, OnFindUpdateFiles);
            Entrance.Event.Subscribe(PackageVersionArgs.EventId, OnPackageVersion);
            Entrance.Event.Subscribe(InitPackageFailedArgs.EventId, OnInitPackageFailed);
            Entrance.Event.Subscribe(DownloadFilesFailedArgs.EventId, OnDownloadFilesFailed);
            Entrance.Event.Subscribe(DownloadProgressArgs.EventID, OnDownloadProgress);
        }

        private void RemoveAllListeners()
        {
            Entrance.Event.Unsubscribe(PatchStateChangeArgs.EventId, OnPatchStateChange);
            Entrance.Event.Unsubscribe(FindUpdateFilesArgs.EventId, OnFindUpdateFiles);
            Entrance.Event.Unsubscribe(PackageVersionArgs.EventId, OnPackageVersion);
            Entrance.Event.Unsubscribe(InitPackageFailedArgs.EventId, OnInitPackageFailed);
            Entrance.Event.Unsubscribe(DownloadFilesFailedArgs.EventId, OnDownloadFilesFailed);
            Entrance.Event.Unsubscribe(DownloadProgressArgs.EventID, OnDownloadProgress);
        }

        void OnPatchStateChange(object sender, GameEventArgs gameEventArgs)
        {
            PatchStateChangeArgs args = gameEventArgs as PatchStateChangeArgs;
            info_txt.text = args.Tips;
        }

        void OnFindUpdateFiles(object sender, GameEventArgs gameEventArgs)
        {
            FindUpdateFilesArgs args = gameEventArgs as FindUpdateFilesArgs;
            Action ok = () =>
            {
                Entrance.Event.Fire(this, BeginDownloadUpdateFilesArgs.Create());
            };
            float sizeMB = args.TotalCount / 1048576f;
            sizeMB = Mathf.Clamp(sizeMB, 0.1f, float.MaxValue);
            string totalSizeMB = sizeMB.ToString("f1");
            ShowMessageBox(
                $"Update now? \n Total count = {args.TotalCount}, Total size = {totalSizeMB}MB",
                ok);
            ;
        }

        void OnPackageVersion(object sender, GameEventArgs gameEventArgs)
        {
            PackageVersionArgs args = gameEventArgs as PackageVersionArgs;
            ver_txt.text = args.PackageVersion;
        }

        void OnInitPackageFailed(object sender, GameEventArgs gameEventArgs)
        {
            Action callback = () =>
            {
                Log.Info("Application Quit");
                Application.Quit();
            };
            ShowMessageBox("Failed to initialize the package, please check the network and try again.", callback);
        }
        
        void OnDownloadFilesFailed(object sender, GameEventArgs gameEventArgs)
        {
            DownloadFilesFailedArgs args = gameEventArgs as DownloadFilesFailedArgs;
            Action callback = () =>
            {
                // Entrance.Event.Fire(this, BeginDownloadUpdateFilesArgs.Create());
                Log.Error("TODO , DownloadFilesFailed");
                
            };
            ShowMessageBox($"Download failed, please check the network and try again. \n {args.Error}", callback);
        }
        
        void OnDownloadProgress(object sender, GameEventArgs gameEventArgs)
        {
            DownloadProgressArgs args = gameEventArgs as DownloadProgressArgs;
            slider.value = (float)args.CurrentDownloadCount / args.TotalDownloadCount;
            string currentSizeMB = (args.CurrentDownloadSizeBytes / 1048576f).ToString("f1");
            string totalSizeMB = (args.TotalDownloadSizeBytes / 1048576f).ToString("f1");
            info_txt.text = $"{args.CurrentDownloadCount}/{args.TotalDownloadCount} {currentSizeMB}MB/{totalSizeMB}MB";
        }


        /// <summary>
        /// 显示对话框
        /// </summary>
        private void ShowMessageBox(string content, System.Action ok)
        {
            // 尝试获取一个可用的对话框
            MessageBox msgBox = null;
            for (int i = 0; i < _msgBoxList.Count; i++)
            {
                var item = _msgBoxList[i];
                if (item.ActiveSelf == false)
                {
                    msgBox = item;
                    break;
                }
            }

            // 如果没有可用的对话框，则创建一个新的对话框
            if (msgBox == null)
            {
                msgBox = new MessageBox();
                var cloneObject = GameObject.Instantiate(_messageBoxObj, _messageBoxObj.transform.parent);
                msgBox.Create(cloneObject);
                _msgBoxList.Add(msgBox);
            }

            // 显示对话框
            msgBox.Show(content, ok);
        }
    }
}