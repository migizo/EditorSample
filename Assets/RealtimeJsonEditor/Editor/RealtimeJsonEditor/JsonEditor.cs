using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

using System;
using System.IO;
using System.Linq;
using System.Reflection; //BindingFlagsを使うのに必要
using UnityEngine.Video;
public class RealtimeJsonEditor : EditorWindow
{
  public string jsonPath = "";
  public object json = null;
  float scrollV = 0;
  GUIStyle labelStyle;

  // windowが重複して増えないようにnullの時のみwindow作成
  [MenuItem("Window/RealtimeJsonEditor")]
  static void Open()
  {
    GetWindow<RealtimeJsonEditor>("RealtimeJsonEditor");
  }

  void OnGUI()
  {
    labelStyle = new GUIStyle(GUI.skin.label);
    labelStyle.wordWrap = true;

    Vector2 _scroll = new Vector2(0, scrollV);
    _scroll = EditorGUILayout.BeginScrollView(_scroll);
    scrollV = _scroll.y;

    EditorGUI.BeginChangeCheck();

    GUILayout.BeginHorizontal();
    if (GUILayout.Button("Select Json ..."))
    {
      jsonPath = EditorUtility.OpenFilePanelWithFilters("Select Json", Application.dataPath, new[]{
            "Json files", "json,JSON,Json"
        });

      using (var fs = new StreamReader(jsonPath, System.Text.Encoding.GetEncoding("shift_jis")))
      {
        var jsonStr = fs.ReadToEnd();
        fs.Close();

        json = Json.Deserialize(jsonStr);
      }
    }
    GUILayout.FlexibleSpace();
    EditorGUILayout.LabelField(new GUIContent(jsonPath, jsonPath), labelStyle, GUILayout.ExpandHeight(true));
    GUILayout.EndHorizontal();

    if (json != null) ShowObject(ref json);

    GUILayout.FlexibleSpace();
    EditorGUILayout.EndScrollView();

    if (EditorGUI.EndChangeCheck())
    {
      Save();
    }
  }

  void OnProjectChange()
  {
    jsonPath = "";
    json = null;
  }

  void OnDestroy()
  {
    jsonPath = "";
    json = null;
  }

  void Save()
  {
    var jsonStr = Json.Serialize(json);
    var sw = new StreamWriter(jsonPath, false);
    sw.Write(jsonStr);
    sw.Close();
  }

  // listIndexが-1以外であればリストの[listIndex]番目の要素
  void ShowObject(ref object element, string label = "")
  {
    EditorGUI.indentLevel++;

    Type t = element.GetType();

    // List
    if (t.Equals(typeof(List<object>)))
    {
      EditorGUILayout.BeginVertical(GUI.skin.box);
      {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label == "" ? "< Root >" : label);
        if (((List<object>)element).Count > 0)
        {
          if (GUILayout.Button("Add Array Element[" + ((List<object>)element).Count + "]"))
          {
            ((List<object>)element).Add(((List<object>)element).Last());
          }
        }

        GUILayout.EndHorizontal();

        for (int i = 0; i < ((List<object>)element).Count; i++)
        {
          var e = ((List<object>)element)[i];
          ShowObject(ref e, "[" + i.ToString() + "]");
          ((List<object>)element)[i] = e;
        }

        // GUILayout.Space(EditorGUI.indentLevel * 20);


      }
      EditorGUILayout.EndVertical();

    }

    // Dictionary
    else if (t.Equals(typeof(Dictionary<string, object>)))
    {
      EditorGUILayout.BeginVertical(GUI.skin.box);
      {
        if (label != "") EditorGUILayout.LabelField(label);
        var list = ((Dictionary<string, object>)element).Keys.ToList();
        foreach (var key in list)
        {
          var e = ((Dictionary<string, object>)element)[key];
          ShowObject(ref e, key);
          ((Dictionary<string, object>)element)[key] = e;
        }
      }
      EditorGUILayout.EndVertical();
    }

    else if (label != "")
    {
      if (t.Equals(typeof(Int64)))
      {
        element = (object)EditorGUILayout.LongField(label, (long)element);
      }
      else if (t.Equals(typeof(Double)))
      {
        element = (object)EditorGUILayout.DoubleField(label, (double)element);
      }
      else if (t.Equals(typeof(Boolean)))
      {
        element = (object)EditorGUILayout.Toggle(label, (bool)element);
      }
      else if (t.Equals(typeof(String)))
      {
        element = (object)EditorGUILayout.TextField(label, (string)element).Trim();
      }
      EditorGUI.indentLevel--;
    }
  }
}
