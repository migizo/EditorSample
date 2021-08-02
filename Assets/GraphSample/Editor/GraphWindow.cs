using UnityEditor;

public class GraphWindow : EditorWindow
{
  [MenuItem("Window/Rendering/GraphSample")]
  public static void Open()
  {
    GetWindow<GraphWindow>(ObjectNames.NicifyVariableName(nameof(GraphWindow)));
  }

  void OnEnable()
  {
    var graphView = new GraphEditorView(this);
    rootVisualElement.Add(graphView);
  }
}