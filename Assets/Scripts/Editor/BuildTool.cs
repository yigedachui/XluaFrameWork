using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class BuildTool : Editor
{

    [MenuItem("Tool/BuildWindowTarget")]
    public static void Build()
    {
        BuildAssets();
    }

    public static void BuildAssets()
    {
        string[] files = Directory.GetFiles(Application.dataPath + "/BuildResources", "*", SearchOption.AllDirectories);

        List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();
        List<string> dependsList = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {            
            if (files[i].EndsWith(".meta"))
            {
                continue;
            }

            AssetBundleBuild buildInfo = new AssetBundleBuild();
            string assetName = PathUtility.GetUnityPath(files[i]);
            buildInfo.assetNames = new string[] { assetName };
            string bundleName = PathUtility.GetStandPath(files[i]).Replace(PathUtility.BuildResourcePath, "").ToLower();
            buildInfo.assetBundleName = bundleName + ".ab";
            buildMap.Add(buildInfo);

            List<string> depends = GetDepends(assetName);

            string fileInfo = assetName + "|" + buildInfo.assetBundleName;
            if (depends.Count > 0)
            {
                fileInfo = fileInfo + "|" + string.Join("|", depends);
            }
            dependsList.Add(fileInfo);
        }

        if (Directory.Exists(PathUtility.BundleOutPath))
        {
            Directory.Delete(PathUtility.BundleOutPath,true);
        }
        Directory.CreateDirectory(PathUtility.BundleOutPath);

        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, buildMap.ToArray(),
            BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

        File.WriteAllLines(Path.Combine(PathUtility.BundleOutPath, AppConst.FileListName), dependsList);

        AssetDatabase.Refresh();
    }

    private static List<string> GetDepends(string bundle)
    {
        string[] depends = AssetDatabase.GetDependencies(bundle);
        if (depends == null)
        {
            return null;
        }
        List<string> files = depends.Where(depend => !depend.EndsWith(".cs") && !depend.Equals(bundle)).ToList();

        return files;
    }

}
