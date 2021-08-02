using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class SearchMenuWindowProvider : ScriptableObject, ISearchWindowProvider
{
  private GraphView view;
  private EditorWindow window;

  public void Initialize(GraphView _view, EditorWindow _window)
  {
    view = _view;
    window = _window;
  }

  List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
  {
    var entries = new List<SearchTreeEntry>();
    entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Node")));

    // Exampleというグループを追加
    entries.Add(new SearchTreeGroupEntry(new GUIContent("Example")) { level = 1 });

    // Exampleグループの下に各ノードを作るためのメニューを追加
    entries.Add(new SearchTreeEntry(new GUIContent(nameof(ExampleNode))) { level = 2, userData = typeof(ExampleNode) });
    entries.Add(new SearchTreeEntry(new GUIContent(nameof(ExampleNode2))) { level = 2, userData = typeof(ExampleNode2) });

    // entries.Add(new SearchTreeEntry(new GUIContent(nameof(AddNode))) { level = 2, userData = typeof(AddNode) });
    // entries.Add(new SearchTreeEntry(new GUIContent(nameof(OutputNode))) { level = 2, userData = typeof(OutputNode) });

    return entries;
  }

  bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
  {
    var type = searchTreeEntry.userData as Type;
    var node = Activator.CreateInstance(type) as Node;

    // マウスの位置にノードを追加
    var worldMousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent, context.screenMousePosition - window.position.position);
    var localMousePosition = view.contentViewContainer.WorldToLocal(worldMousePosition);
    node.SetPosition(new Rect(localMousePosition, new Vector2(100, 100)));

    view.AddElement(node);
    return true;
  }
}