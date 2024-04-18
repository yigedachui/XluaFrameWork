using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    
    public class BundleInfo
    {
        public string AssetName;
        public string BundleName;
        public List<string> AssetsDepends;
    }

    public Dictionary<string, BundleInfo> m_BundleInfo = new Dictionary<string, BundleInfo>();

    public void ParesDependFile()
    {
        string paresPath = Path.Combine(PathUtility.BundleResourcePath, AppConst.FileListName);

        string[] files = File.ReadAllLines(paresPath);

        for (int i = 0; i < files.Length; i++)
        {
            string[] info = files[i].Split('|');
            BundleInfo bundleInfo = new BundleInfo();
            bundleInfo.AssetName = info[0];
            bundleInfo.BundleName = info[1];

            bundleInfo.AssetsDepends = new List<string>(info.Length - 2);
            for (int j = 2; j < info.Length; j++)
            {
                bundleInfo.AssetsDepends.Add(info[j]);
            }
            m_BundleInfo.Add(bundleInfo.AssetName, bundleInfo);

            if (info[0].Contains("LuaScript"))
            {
                Manager.Lua.LuaName.Add(info[0]);
            }
        }

    }

    private IEnumerator LoadAssetBundleAsync(string assetName, Action<UnityEngine.Object> OnLoadAssetCompleted = null)
    {
        string bundle = m_BundleInfo[assetName].BundleName;
        string bundlePath = Path.Combine(PathUtility.BundleResourcePath, bundle);

        List<string> depends = m_BundleInfo[assetName].AssetsDepends;
        if (depends != null && depends.Count > 0)
        {
            for (int i = 0; i < depends.Count; i++)
            {
                yield return LoadAssetBundleAsync(depends[i]);
            }            
        }

        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return request;

        AssetBundleRequest asset = request.assetBundle.LoadAssetAsync(assetName);

        OnLoadAssetCompleted?.Invoke(asset.asset);
    }

#if UNITY_EDITOR
    public void EditorLoadAsset(string assetName, Action<UnityEngine.Object> action)
    {
        UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetName, typeof(UnityEngine.Object));
        if (!obj)
        {
            throw new ArgumentNullException(assetName);
        }
        action?.Invoke(obj);
    }
# endif
    public void LoadAsset(string assetName,Action<UnityEngine.Object> action)
    {
        if (AppConst.GameMode == GameMode.EditorMode)
        {
#if UNITY_EDITOR
            EditorLoadAsset(assetName, action);
#endif
        }
        else
        {
            StartCoroutine(LoadAssetBundleAsync(assetName, action));
        }
        
    }

    public void LoadUI(string name, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtility.GetUIPath(name), action);
    }

    public void LoadEffect(string name, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtility.GetEffectPath(name), action);
    }

    public void LoadMusic(string name, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtility.GetMusicPath(name), action);
    }  
    
    public void LoadSound(string name, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtility.GetSoundPath(name), action);
    }

    public void LoadSprite(string name, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtility.GetSpritePath(name), action);
    }

    public void LoadLua(string name, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(name, action);
    }
}
