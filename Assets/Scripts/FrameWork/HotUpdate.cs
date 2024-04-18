using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class HotUpdate : MonoBehaviour
{
    public class DownFileInfo
    {
        public string url;
        public string fileName;
        public DownloadHandler FileData;
    }

    byte[] FileListData;
    byte[] RemoteFileListData;

    private void Start()
    {
        if (IsFirstInstall())
        {
            ReleaseResources();
        }
        else
        {
            CheckUpdate();
        }
    }


    IEnumerator LoadFile(DownFileInfo info, Action<DownFileInfo> action)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(info.url);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.LogError("error");
            yield break;
        }
        yield return new WaitForSeconds(0.2f);
             

        info.FileData = webRequest.downloadHandler;
        action?.Invoke(info);
        webRequest.Dispose();
    }

    IEnumerator LoadFile(List<DownFileInfo> info, Action<DownFileInfo> action, Action AllComplete)
    {
        foreach (var fileInfo in info)
        {
            yield return LoadFile(fileInfo, action);
        }
        AllComplete?.Invoke();
    }

    private List<DownFileInfo> GetFileInfo(string fileData, string path)
    {
        string content = fileData.Trim().Replace("\r", "");
        List<DownFileInfo> list = new List<DownFileInfo>();
        string[] subContent = content.Split('\n');
        for (int i = 0; i < subContent.Length; i++)
        {
            string[] subTwoContent = subContent[i].Split('|');
            DownFileInfo downFileInfo = new DownFileInfo();
            for (int j = 0; j < subTwoContent.Length; j++)
            {
                downFileInfo.fileName = subTwoContent[1];
                downFileInfo.url = Path.Combine(path, subTwoContent[1]);
            }
            list.Add(downFileInfo);
        }

        return list;
    }

    private bool IsFirstInstall()
    {
        bool isExitsReadPath = FileUtility.IsExits(Path.Combine(PathUtility.ReadPath, AppConst.FileListName));
        bool isExitsReadWritePath = FileUtility.IsExits(Path.Combine(PathUtility.ReadWritePath, AppConst.FileListName));
        return isExitsReadPath && (isExitsReadWritePath == false);
    }

    private void ReleaseResources()
    {
        string path = Path.Combine(PathUtility.ReadPath, AppConst.FileListName);
        path = PathUtility.GetStandPath(path);
        DownFileInfo info = new DownFileInfo();
        info.url = path;
        StartCoroutine(LoadFile(info, OnFileListDownload));
    }

    private void OnFileListDownload(DownFileInfo info)
    {
        FileListData = info.FileData.data;
        string fileList = info.FileData.text;
        List<DownFileInfo> files = GetFileInfo(fileList, PathUtility.ReadPath);
        StartCoroutine(LoadFile(files, OnReleaseFileComplete, AllReleaseFileComplete));
    }
    private void OnReleaseFileComplete(DownFileInfo info)
    {
        Debug.Log("ReleaseFile From:" + info.url);
        string writeFile = Path.Combine(PathUtility.ReadWritePath, info.fileName);
        FileUtility.WriteFile(writeFile, info.FileData.data);
    }

    private void AllReleaseFileComplete()
    {
        FileUtility.WriteFile(Path.Combine(PathUtility.ReadWritePath, AppConst.FileListName), FileListData);
        EnterGame();
    }

    private void CheckUpdate()
    {
        string fileList = Path.Combine(AppConst.ResourceUrl, AppConst.FileListName);
        fileList = PathUtility.GetStandPath(fileList);
        DownFileInfo info = new DownFileInfo() { url = fileList };
        StartCoroutine(LoadFile(info, OnDownLoadServerFile));
    }

    private void OnDownLoadServerFile(DownFileInfo info)
    {
        RemoteFileListData = info.FileData.data;
        List<DownFileInfo> fileInfo = GetFileInfo(info.FileData.text, AppConst.ResourceUrl);
        List<DownFileInfo> DownListFiles = new List<DownFileInfo>();
        for (int i = 0; i < fileInfo.Count; i++)
        {
            string localFile = Path.Combine(PathUtility.ReadWritePath, fileInfo[i].fileName);
            if (!FileUtility.IsExits(localFile))
            {
                fileInfo[i].url = Path.Combine(AppConst.ResourceUrl, fileInfo[i].fileName);

                DownListFiles.Add(fileInfo[i]);
            }
        }

        if (DownListFiles.Count > 0)
        {
            StartCoroutine(LoadFile(DownListFiles, OnUpdateFileComplete, OnAllComplete));
        }
        else
        {
            EnterGame();
        }
    }

    private void OnAllComplete()
    {
        FileUtility.WriteFile(Path.Combine(PathUtility.ReadWritePath, AppConst.FileListName), RemoteFileListData);
        EnterGame();
    }

    private void OnUpdateFileComplete(DownFileInfo info)
    {
        Debug.Log("UpdateFile From:" + info.url);

        FileUtility.WriteFile(Path.Combine(PathUtility.ReadWritePath, info.fileName), info.FileData.data);
    }


    private void EnterGame()
    {
        Manager.Resource.ParesDependFile();
        Manager.Resource.LoadUI("Canvas", OnLoadComplete);
    }

    private void OnLoadComplete(UnityEngine.Object obj)
    {
        Instantiate(obj);
    }
}
