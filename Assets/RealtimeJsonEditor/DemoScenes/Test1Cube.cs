using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using System;
using System.IO;

namespace Test1
{

  public class Test1Cube : MonoBehaviour
  {
    string filePath;
    Test1[] test1 = null;
    [SerializeField] int _currentIndex = 0;
    public int currentIndex
    {
      private get
      {
        if (test1 == null) return 0;
        else return Mathf.Clamp(_currentIndex, 0, test1.Length - 1);
      }
      set { _currentIndex = value; }
    }
    private FileSystemWatcher fileSystemWatcher = null;

    void Start()
    {
      filePath = Application.dataPath + "/RealtimeJsonEditor/DemoScenes/Json/test1.json";
      LoadJson();

      fileSystemWatcher = new FileSystemWatcher();
      fileSystemWatcher.Path = Application.dataPath + "/RealtimeJsonEditor/DemoScenes/Json";
      fileSystemWatcher.Filter = "*.json";
      fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
      fileSystemWatcher.Changed += new FileSystemEventHandler(OnFileChanged);
      fileSystemWatcher.EnableRaisingEvents = true;

    }

    void Update()
    {
      Vector3 rot = Vector3.zero;
      rot.x = test1[currentIndex].rotate[0] * test1[currentIndex].speed;
      rot.y = test1[currentIndex].rotate[1] * test1[currentIndex].speed;
      rot.z = test1[currentIndex].rotate[2] * test1[currentIndex].speed;
      transform.localEulerAngles += rot;
    }

    void LoadJson()
    {
      string jsonstr = LoadJsonText(filePath);
      jsonstr = "{" + $"\"root\":{jsonstr}" + "}";
      test1 = JsonUtility.FromJson<Test1Parent>(jsonstr).root;

      Debug.Log(test1[currentIndex].log);
    }

    string LoadJsonText(string path)
    {
      StreamReader reader = new StreamReader(path);
      string datastr = reader.ReadToEnd();
      reader.Close();
      return datastr;
    }

    void OnFileChanged(System.Object source, FileSystemEventArgs e)
    {
      LoadJson();
    }
  }

  //====================================================================

  [System.Serializable]
  public class Test1Parent
  {
    public Test1[] root;
  }
  [System.Serializable]
  public class Test1
  {
    public string log;
    public float speed;
    public float[] rotate;
  }

}

