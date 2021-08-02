using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GraphEditorView : GraphView
{
  public GraphEditorView(EditorWindow editorWindow)
  {
    // 初期ノードを追加
    AddElement(new ExampleNode());

    // 親のサイズに合わせてGraphViewのサイズを設定
    this.StretchToParentSize();
    /*
      同じく親のUIに合わせてサイズ設定する
      style.flexGrow = 1;
      style.flexShrink = 1;
    */

    // MMBスクロールでズームインアウトができるように
    SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

    // 背景
    Insert(0, new GridBackground());

    // MMBドラッグで描画範囲を動かせるように
    this.AddManipulator(new ContentDragger());
    // LMBドラッグで選択した要素を動かせるように
    this.AddManipulator(new SelectionDragger());
    // LMBドラッグで範囲選択ができるように
    this.AddManipulator(new RectangleSelector());

    // 右クリックメニューを追加
    var menuWindowProvider = ScriptableObject.CreateInstance<SearchMenuWindowProvider>();
    menuWindowProvider.Initialize(this, editorWindow);
    nodeCreationRequest += context =>
    {
      SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), menuWindowProvider);
    };
  }


  // 接続可能なポートリストを返す。
  public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
  {
    var compatiblePorts = new List<Port>();

    compatiblePorts.AddRange(ports.ToList().Where(port =>
    {
      // 同じノードには繋げない
      if (startPort.node == port.node)
        return false;

      // Input同士、Output同士は繋げない
      if (port.direction == startPort.direction)
        return false;

      // ポートの型が一致していない場合は繋げない
      if (port.portType != startPort.portType)
        return false;

      return true;
    }));

    return compatiblePorts;
  }
}