﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static ResourceManager _resource;

    public static ResourceManager Resource
    {
        get { return _resource; }
    }

    private static LuaManager _lua;
    public static LuaManager Lua
    {
        get { return _lua; }
    }

    private static LuaBehavior _behavior;
    public static LuaBehavior Behavior
    {
        get { return _behavior; }
    }

    private void Awake()
    {
        _resource = this.gameObject.AddComponent<ResourceManager>();
        _lua = this.gameObject.AddComponent<LuaManager>();
        Resource.ParesDependFile();
        Lua.Init(DoLua);

        _behavior = this.gameObject.AddComponent<LuaBehavior>();

    }

    void DoLua()
    {
        Lua.StartLua("main");
    }

}
