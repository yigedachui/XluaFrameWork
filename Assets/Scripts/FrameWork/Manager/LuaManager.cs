using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class LuaManager : MonoBehaviour
{

    public List<string> LuaName = new List<string>();

    public Dictionary<string, byte[]> m_LuaScripts;

    public LuaEnv LuaEnv;

    private Action OnInitComplete;

    public void Init(Action action)
    {
        m_LuaScripts = new Dictionary<string, byte[]>();
        LuaEnv = new LuaEnv();
        LuaEnv.AddLoader(Loader);
        OnInitComplete += action;
        if (AppConst.GameMode == GameMode.EditorMode)
        {
#if UNITY_EDITOR
            EditorLoadLuaScripts();
#endif
        }
        else
        {
            LoadLuaScripts();
        }
    }

    public void StartLua(string name)
    {
        LuaEnv.DoString(string.Format("require '{0}'", name));
    }

    byte[] Loader(ref string Name)
    {
        return GetLuaScript(Name);
    }

    public byte[] GetLuaScript(string name)
    {
        name = name.Replace(".", "/");
        string fileName = PathUtility.GetLuaPath(name);
        if (m_LuaScripts.TryGetValue(fileName, out byte[] LuaScript))
        {
            return LuaScript;
        }
        else
        {            
            Debug.LogError("Lua not exist: " + fileName);
            return null;
        }
        
    }

    public void LoadLuaScripts()
    {
        foreach (var name in LuaName)
        {
            Manager.Resource.LoadLua(name, (UnityEngine.Object obj) => {
                byte[] data = (obj as TextAsset).bytes;
                AddLuaScript(name, data);
                if (m_LuaScripts.Count >= LuaName.Count)
                {
                    OnInitComplete?.Invoke();
                    LuaName.Clear();
                    LuaName = null;
                }

            });
        }
    }

    public void AddLuaScript(string name, byte[] LuaScript)
    {
        m_LuaScripts[name] = LuaScript;
    }

#if UNITY_EDITOR
    public void EditorLoadLuaScripts()
    {
        string[] LuaScripts = Directory.GetFiles(PathUtility.LuaPath, "*.bytes", SearchOption.AllDirectories);
        for (int i = 0; i < LuaScripts.Length; i++)
        {
            string stand = PathUtility.GetStandPath(LuaScripts[i]);
            byte[] b = File.ReadAllBytes(stand);
            AddLuaScript(stand, b);
        }
        OnInitComplete?.Invoke();
    }

#endif

    private void Update()
    {
        if (LuaEnv != null)
        {
            LuaEnv.Tick();
        }
    }

    private void OnDestroy()
    {
        LuaEnv.Dispose();
        LuaEnv = null;
    }

}