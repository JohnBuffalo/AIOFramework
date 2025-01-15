using System.IO;
using Cysharp.Threading.Tasks;
using GameFramework;
using UnityEditor;
using UnityEngine;
using HybridCLR.Editor.Commands;
using YooAsset.Editor;


namespace AIOFramework.Editor.CI
{
    /// <summary>
    /// 游戏打包工具类
    /// </summary>
    public partial class GameAppBuilder
    {
        private static readonly string BuildSettingPath = "Settings/GlobalSettings";
        private static readonly string HotfixAssetPath = "AssetArt/HotUpdate";

        
        /// <summary>
        /// 大版本出包
        /// </summary>
        [MenuItem("GameBuilder/MajorVersion 大版本", priority = 0)]
        public static void MajorVersion()
        {
            if (EditorUtility.DisplayDialog("提示", $"开始构建大版本,目标版本号 : {GetNextMajorVersion()}！", "Yes", "No"))
            {
                EditorTools.ClearUnityConsole();
                EditorApplication.delayCall += MajorVersionInternal;
            }
            else
            {
                Debug.LogWarning("[Build] 打包已经取消");
            }
        }

        private static void MajorVersionInternal()
        {
            var settings = SettingsUtils.GlobalSettings.FrameworkGlobalSettings;
            var nextVersion = GetNextMajorVersion();
            //构造patch packages
            var buildPackageSuccess = BuildPackages(nextVersion, EBuildinFileCopyOption.ClearAndCopyAll);
            if (!buildPackageSuccess)
            {
                Debug.LogError("Build Package failed");
                AssetDatabase.Refresh();
                return;
            }

            //生成app包体
            settings.ScriptVersion = nextVersion;
            //资源发布
            CopyBundlesToCdn();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 小版本热更
        /// </summary>
        [MenuItem("GameBuilder/MajorVersion 小版本", priority = 1)]
        public static void MinorVersion()
        {
            var packageName = AssetBundleCollectorSettingData.Setting.Packages[0].PackageName;
            var builtinPath = Application.streamingAssetsPath + $"/yoo/{packageName}";
            
            if (!Directory.Exists(builtinPath) || Directory.GetFiles(builtinPath).Length==0)
            {
                Debug.LogError("请先构建大版本");
                return;
            }
            
            if (EditorUtility.DisplayDialog("提示", $"开始构建热更版本,目标版本号 : {GetNextMinorVersion()}！", "Yes", "No"))
            {
                EditorTools.ClearUnityConsole();
                EditorApplication.delayCall += MinorVersionInternal;
            }
            else
            {
                Debug.LogWarning("[Build] 打包已经取消");
            }
        }

        private static void MinorVersionInternal()
        {
            var settings = SettingsUtils.GlobalSettings.FrameworkGlobalSettings;
            var nextVersion = GetNextMinorVersion();
            //构造patch packages
            var buildPackageSuccess = BuildPackages(nextVersion, EBuildinFileCopyOption.None);
            if (!buildPackageSuccess)
            {
                Debug.LogError("Build Package failed");
                AssetDatabase.Refresh();
                return;
            }

            settings.ScriptVersion = nextVersion;
            //资源发布
            CopyBundlesToCdn();
            AssetDatabase.Refresh();
        }

        private static bool BuildPackages(string nextVersion, EBuildinFileCopyOption copyOption)
        {
            //1.编译C#代码
            PrebuildCommand.GenerateAll();
            //2.拷贝HotUpdate和AOT到指定目录
            CopyDLLToAssets();
            //3.打包资源
            AssetBundleCollectorSettingData.FixFile();
            var buildBundleResult = RunBuildBundle_SBP(nextVersion, EditorUserBuildSettings.activeBuildTarget, copyOption);
            if (!buildBundleResult)
            {
                Debug.LogError("Build Bundle Failed");
                return false;
            }

            return true;
        }

        private static string GetNextMinorVersion()
        {
            var settings = SettingsUtils.GlobalSettings.FrameworkGlobalSettings;
            var version = settings.ScriptVersion.Split('.');
            Debug.Log($"当前版本 : {settings.ScriptVersion}");
            var year = version[0];
            var major = version[1];
            var minor = int.Parse(version[2]);
            var nextVersion = $"{year}.{major}.{minor + 1}";
            return nextVersion;
        }

        private static string GetNextMajorVersion()
        {
            var settings = SettingsUtils.GlobalSettings.FrameworkGlobalSettings;
            var version = settings.ScriptVersion.Split('.');
            Debug.Log($"当前版本 : {settings.ScriptVersion}");
            var year = version[0];
            int major = int.Parse(version[1]);
            var nextVersion = $"{year}.{major + 1}.{0}";
            return nextVersion;
        }

        /// <summary>
        /// 将热更资源拷贝到CDN
        /// </summary>
        // [MenuItem("GameBuilder/CopyBundlesToCdn", priority = 2)]
        private static void CopyBundlesToCdn()
        {
            //note正式项目会将资源拷贝到临时位置. 发布版本时再拷贝到正式目录
            //演示项目只支持本地回环测试
            CopyBundlesToCdnInternal();
        }

        private static void CopyBundlesToCdnInternal()
        {
            var settings = SettingsUtils.GlobalSettings.FrameworkGlobalSettings;
            var destDirectoryPath = GetCdnURL() + "/" + settings.ScriptVersion;
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var packageName = AssetBundleCollectorSettingData.Setting.Packages[0].PackageName;
            var srcDirectoryPath = YooAsset.PathUtility.RegularPath(Application.dataPath +
                                       $"/../Bundles/{buildTarget}/{packageName}/{settings.ScriptVersion}");
            // Debug.Log(destDirectoryPath);
            // Debug.Log(srcDirectoryPath);
            if (Directory.Exists(destDirectoryPath))
            {
                var files = Directory.GetFiles(destDirectoryPath);
                for (int i = 0; i < files.Length; i++)
                {
                    File.Delete(files[i]);
                }
            }
            else
            {
                Directory.CreateDirectory(destDirectoryPath);
            }

            if (!Directory.Exists(srcDirectoryPath))
            {
                Debug.LogError($"Source Directory is Invalid {srcDirectoryPath}");
                return;
            }

            var srcFiles = Directory.GetFiles(srcDirectoryPath);
            for (int i = 0; i < srcFiles.Length; i++)
            {
                var fileName = Path.GetFileName(srcFiles[i]);
                File.Copy(srcFiles[i], GameFramework.Utility.Path.GetRegularPath($"{destDirectoryPath}/{fileName}"));
            }
        }

        private static string GetCdnURL()
        {
            string url = "http://127.0.0.1";
            string platFormName = "Android";
            var settings = SettingsUtils.GlobalSettings.FrameworkGlobalSettings;

            switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget)
            {
                case UnityEditor.BuildTarget.Android:
                    url = settings.AndroidAppUrl;
                    platFormName = "Android";
                    break;
                case UnityEditor.BuildTarget.iOS:
                    url = settings.IOSAppUrl;
                    platFormName = "IOS";
                    break;
                case UnityEditor.BuildTarget.StandaloneWindows64:
                    url = settings.WindowsAppUrl;
                    platFormName = "PC";
                    break;
                case UnityEditor.BuildTarget.StandaloneOSX:
                    url = settings.MacOSAppUrl;
                    platFormName = "MacOS";
                    break;
                case UnityEditor.BuildTarget.StandaloneWindows:
                    url = settings.WindowsAppUrl;
                    platFormName = "PC";
                    break;
            }

            if (settings.AppStage == AppStageEnum.Debug)
            {
                return $"D:/UnityReferences/StarForceServer/CDN/{platFormName}";
            }
            else
            {
                return $"{url}/CDN/{platFormName}";
            }
        }
    }
}