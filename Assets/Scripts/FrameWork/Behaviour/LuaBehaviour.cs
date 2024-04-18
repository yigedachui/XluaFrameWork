using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaBehavior : MonoBehaviour
{
    protected LuaEnv luaEnv = Manager.Lua.LuaEnv;
    protected LuaTable scriptEnv;

    public Action awake;
    public Action start;
    public Action update;

    private void Awake()
    {

    }

    private void Start()
    {
        scriptEnv = luaEnv.NewTable();
        LuaTable meta = luaEnv.NewTable();
        meta.Set("__Index", luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        scriptEnv.Set("Self", this);

        scriptEnv.Get("awake", out awake);
        scriptEnv.Get("start", out start);
        scriptEnv.Get("update", out update);

        if (awake != null)
            awake?.Invoke();

        if (start != null)
        {
            start.Invoke();
        }
    }

    private void Update()
    {

        if (update!=null)
        {
            update?.Invoke();
        }

        if (luaEnv != null)
        {
            luaEnv.Tick();
        }
    }

    protected virtual void Close()
    {
        luaEnv = null;
        scriptEnv = null;

        awake = null;
        start = null;
        update = null;       
    }

    private void OnDestroy()
    {
        Close();
    }

    private void OnApplicationQuit()
    {
        Close();
    }

}
