using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    private void Awake()
    {
        Debug.Log("awake");
    }

    void Start()
    {
        Debug.Log("start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
