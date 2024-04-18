using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUtility 
{
    public static readonly string AssetPath = Application.dataPath;

    public static readonly string BuildResourcePath = AssetPath + "/BuildResources/";

    public static readonly string BundleOutPath = Application.streamingAssetsPath;

    public static readonly string ReadPath = Application.streamingAssetsPath;

    public static readonly string ReadWritePath = Application.persistentDataPath;

    public static readonly string LuaPath = "Assets/BuildResources/LuaScript";

    public static string BundleResourcePath
    {
        get {
            if (AppConst.GameMode == GameMode.UpdateMode)
            {
                return ReadWritePath;
            }
            return ReadPath;
        }
    }

    public static string GetUnityPath(string Path)
    {
        if (string.IsNullOrEmpty(Path)) { return string.Empty; }

        return GetStandPath(Path).Substring(Path.IndexOf("Assets"));
    }

    public static string GetStandPath(string Path)
    {
        if (string.IsNullOrEmpty(Path)) { return string.Empty; }
        return Path.Trim().Replace("\\", "/");
    }

    public static string GetLuaPath(string name)
    {
        return string.Format("Assets/BuildResources/LuaScript/{0}.bytes", name);
    }

    public static string GetUIPath(string name)
    {
        return string.Format("Assets/BuildResources/UI/Prefab/{0}.prefab", name);
    }

    public static string GetMusicPath(string name)
    {
        return string.Format("Assets/BuildResources/Audio/Music/{0}", name);
    }

    public static string GetSoundPath(string name)
    {
        return string.Format("Assets/BuildResources/Audio/Sound/{0}", name);
    }

    public static string GetEffectPath(string name)
    {
        return string.Format("Assets/BuildResources/Effect/Prefab/{0}.prefab", name);
    }

    public static string GetSpritePath(string name)
    {
        return string.Format("Assets/BuildResources/Sprite/{0}", name);
    }

    public static string GetScenePath(string name)
    {
        return string.Format("Assets/BuildResources/Scene/{0}.unity", name);
    }
}
