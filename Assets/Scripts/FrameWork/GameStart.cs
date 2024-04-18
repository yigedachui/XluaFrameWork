using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameMode mode;

    private static GameStart obj;

    // Start is called before the first frame update
    void Awake()
    {
        if (obj == null)
        {
            obj = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(obj);
        AppConst.GameMode = this.mode;
    }

}
