using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public abstract class BassSimpleNode : Node
{
  protected List<Port> inputPortList = new List<Port>();
  protected List<Port> outputPortList = new List<Port>();

  public BassSimpleNode() { }
  public BassSimpleNode(string name, int inTotal, int outTotal)
  {
    title = name;

    // 消されないようにする
    // capabilities -= Capabilities.Deletable; 


    // 入力用のポートを作成
    for (int i = 0; i < inTotal; i++)
    {
      // 第三引数をPort.Capacity.Multipleにすると複数のポートへの接続が可能になる
      var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(float));
      inputPort.portName = "Input" + i.ToString();
      inputContainer.Add(inputPort); // 入力用ポートはinputContainerに追加する
      inputPortList.Add(inputPort);
    }
    // 出力用のポートを作る
    for (int i = 0; i < outTotal; i++)
    {
      var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
      outputPort.portName = "Output" + i.ToString();
      outputContainer.Add(outputPort); // 出力用ポートはoutputContainerに追加する
      inputPortList.Add(outputPort);
    }
  }
  public abstract void Execute();

}

public class ExampleNode : BassSimpleNode
{
  public ExampleNode() : base("Example", 1, 1) { }
  public override void Execute() { }

}

public class ExampleNode2 : BassSimpleNode
{
  public ExampleNode2() : base("Example", 1, 1) { }
  public override void Execute()
  {
    if (inputPortList.Count == 0) return;
    var prevEdge = inputPortList[0].connections.FirstOrDefault();
    var prevNode = prevEdge.output.node as BassSimpleNode;
    if (prevNode == null) return;
    Debug.Log(prevNode.title);
  }

}


