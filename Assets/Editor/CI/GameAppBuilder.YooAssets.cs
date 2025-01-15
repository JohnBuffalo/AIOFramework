using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using YooAsset;
using YooAsset.Editor;

namespace AIOFramework.Editor.CI
{
    public partial class GameAppBuilder
    {
        /// <summary>
        /// ScriptBuildPipeline构建
        /// </summary>
        public static bool RunBuildBundle_SBP(string version, BuildTarget buildTarget, EBuildinFileCopyOption copyOption)
        {
            var packageNames = GetBuildPackageNames();
            if (packageNames.Count == 0)
            {
                Debug.LogError("not found any package");
                return false;
            }

            var _buildPackage = packageNames[0];
            EBuildPipeline _buildPipeline = EBuildPipeline.ScriptableBuildPipeline;
            var fileNameStyle = AssetBundleBuilderSetting.GetPackageFileNameStyle(_buildPackage, _buildPipeline);
            var buildinFileCopyOption = copyOption;
            var buildinFileCopyParams = ""; //可指定拷贝的tags
            var compressOption = ECompressOption.LZ4;
            var clearBuildCache = true;
            var useAssetDependencyDB = true;

            ScriptableBuildParameters buildParameters = new ScriptableBuildParameters();
            buildParameters.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            buildParameters.BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
            buildParameters.BuildPipeline = _buildPipeline.ToString();
            buildParameters.BuildBundleType = (int)EBuildBundleType.AssetBundle;
            buildParameters.BuildTarget = buildTarget;
            buildParameters.PackageName = _buildPackage;
            buildParameters.PackageVersion = version;
            buildParameters.EnableSharePackRule = true;
            buildParameters.VerifyBuildingResult = true;
            buildParameters.FileNameStyle = fileNameStyle;
            buildParameters.BuildinFileCopyOption = buildinFileCopyOption;
            buildParameters.BuildinFileCopyParams = buildinFileCopyParams;
            buildParameters.CompressOption = compressOption;
            buildParameters.ClearBuildCacheFiles = clearBuildCache;
            buildParameters.UseAssetDependencyDB = useAssetDependencyDB;
            buildParameters.EncryptionServices = CreateEncryptionInstance(_buildPackage, _buildPipeline);

            ScriptableBuildPipeline pipeline = new ScriptableBuildPipeline();
            var buildResult = pipeline.Run(buildParameters, true);
            return buildResult.Success;
        }

        private static List<string> GetBuildPackageNames()
        {
            List<string> result = new List<string>();
            foreach (var package in AssetBundleCollectorSettingData.Setting.Packages)
            {
                result.Add(package.PackageName);
            }

            return result;
        }
        private static IEncryptionServices CreateEncryptionInstance(string packageName, EBuildPipeline buildPipeline)
        {
            var encyptionClassName = AssetBundleBuilderSetting.GetPackageEncyptionClassName(packageName, buildPipeline);
            var encryptionClassTypes = EditorTools.GetAssignableTypes(typeof(IEncryptionServices));
            var classType = encryptionClassTypes.Find(x => x.FullName.Equals(encyptionClassName));
            if (classType != null)
                return (IEncryptionServices)Activator.CreateInstance(classType);
            else
                return null;
        }
    }
}