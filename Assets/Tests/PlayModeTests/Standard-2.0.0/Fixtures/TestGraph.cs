using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject]
public struct Pin
{
	[JsonProperty("id")]
	public string Id;
	[JsonProperty("type")]
	public string Type;
	[JsonProperty("value")]
	[CanBeNull] public object Value;
}

[JsonObject]
public class Node
{
	[JsonProperty("id")]
	public string Id;
	[JsonProperty("type")]
	public string Type;
	[JsonProperty("inputs")]
	public List<Pin> Inputs;
	[JsonProperty("outputs")]
	public List<Pin> Outputs;
}

[JsonObject]
public struct Connection
{
	[JsonProperty("source")]
	public string Source;
	[JsonProperty("sourceKey")]
	public string SourceKey;
	[JsonProperty("target")]
	public string Target;
	[JsonProperty("targetKey")]
	public string TargetKey;
}

[JsonObject]
public struct Binding
{
	[JsonProperty("id")]
	public string Id;
	[JsonProperty("scope")]
	public string Scope;
	[JsonProperty("type")]
	public string Type;
	[JsonProperty("value")]
	public object Value;
}

[JsonObject]
public struct Function
{
	[JsonProperty("nodes")]
	public List<Node> Nodes;
	[JsonProperty("connections")]
	public List<Connection> Connections;
	[JsonProperty("bindings")]
	public List<Binding> Bindings;
}

[JsonObject(MemberSerialization.Fields)]
public class TestGraph
{
    public delegate void BuilderFunc(ref TestGraph builder);
    
    public static string Create(BuilderFunc build)
    {
        var testGraph = new TestGraph(BlueprintVersion.Version);
        build(ref testGraph);
        return testGraph.Build();
    }
    
    [JsonProperty("version")]
	private string _version;
	[JsonProperty("nodes")]
	private readonly List<Node> _nodes = new();
	[JsonProperty("connections")]
	private readonly List<Connection> _connections = new();
	[JsonProperty("bindings")]
	private readonly List<Binding> _bindings = new();
	[JsonProperty("fns")] 
	private readonly Dictionary<string, Function> _functions = new();

	[JsonIgnore] private Node _entryNode;
	[JsonIgnore] private Dictionary<string, Node> _bindingNodes = new();

	public T AddNode<T>(T node) where T: Node
	{
		_nodes.Add(node);
		return node;
	}

	public void ConnectNodes(
		Node fromNode,
		string fromPin,
		Node toNode,
		string toPin)
	{
		_connections.Add(
			new Connection()
			{
				Source = fromNode.Id,
				SourceKey = fromPin,
				Target = toNode.Id,
				TargetKey = toPin,
			}
		);
	}

	public void ConnectExecution(Node fromNode, Node toNode)
	{
		ConnectNodes(
			fromNode,
			"Exec",
			toNode,
			"Exec"
		);
	}

	public void ConnectEntry(Node toNode, string toPin = "Exec")
	{
		ConnectNodes(
			_entryNode,
			"Exec",
			toNode,
			toPin
		);
	}

	private void AddBinding(string id, string type, string scope, object defaultValue = null)
	{
		_bindings.Add(
			new Binding
			{
				Id = id,
				Scope = scope,
				Type = type,
				Value = defaultValue,
			}
		);
	}

	public void AddInput(string id, string type, object defaultValue = null)
	{
		AddBinding(id, type, "input", defaultValue);
	}
	
	public void AddOutput(string id, string type)
	{
		AddBinding(id, type, "output");
	}

	public Get<T> AddInputWithNode<T>(string id, string type, object defaultValue = null)
	{
		AddInput(id, type, defaultValue);
		var node = new Get<T>(type, id);
		_bindingNodes.Add(id, node);
		return AddNode(node);
	}
	
	public Set AddOutputWithNode(string id, string type, object value = null)
	{
		AddOutput(id, type);
		var node = new Set(id, value);
		_bindingNodes.Add(id, node);
		return AddNode(node);
	}

	public void PassInputToNode(string bindingName, Node toNode, string toPin)
	{
		if (_bindingNodes.TryGetValue(bindingName, out var node))
		{
			ConnectNodes(node, Get<int>.Out.Value, toNode, toPin);
		}
	}
	
	public void SetOutputFromNode(Node fromNode, string fromPin, string bindingName)
	{
		if (_bindingNodes.TryGetValue(bindingName, out var node))
		{
			ConnectNodes(fromNode, fromPin, node, Set.In.Value);
		}
	}
	
	public void AddLocalVariable(string id, string type, object defaultValue = null)
	{
		AddBinding(id, type, "var", defaultValue);
	}

	private TestGraph(string version)
	{
		if (!Version.TryParse(version, out _))
		{
			throw new Exception("Invalid version for test graph builder");
		}

		_version = version;
		_entryNode = new Entry();
		_nodes.Add(_entryNode);
	}

	// TODO: Implement function builder
	public void AddFunction(string id)
	{
		throw new NotImplementedException();
	}

	public static string EmptyGraph(string version)
	{
		var graph = new TestGraph(version);
		graph._nodes.Clear();
		return graph.Build();
	}

	private string Build()
	{
		var output = JsonConvert.SerializeObject(this);
		Debug.Log(output);
		return output;
	}
}

public static class UBFTypes
{
    public const string String = "string";
    public const string Int = "int";
    public const string Float = "float";
    public const string Boolean = "boolean";
    public const string Unknown = "unknown";
    public const string Json = "Json";
    public const string Color = "Color";
    public const string MeshResource = "Resource<Mesh>";
    public const string GLBResource = "Resource<GLB>";
    public const string TextureResource = "Resource<Texture>";
    public const string BlueprintResource = "Resource<Blueprint>";
    public const string GenericT = "T";
    public const string GenericU = "U";
    public const string GenericV = "V";
    public const string Exec = "exec";
    public const string Rig = "Rig";
    public const string MeshRenderer = "MeshRenderer";
    public const string Material = "Material";
    public const string SceneNode = "SceneNode";
    public const string MeshConfig = "MeshConfig";
    public const string ArrayString = "Array<string>";
    public const string ArrayInt = "Array<int>";
    public const string ArrayFloat = "Array<float>";
    public const string ArrayBoolean = "Array<boolean>";
    public const string ArrayUnknown = "Array<unknown>";
    public const string ArrayJson = "Array<Json>";
    public const string ArrayColor = "Array<Color>";
    public const string ArrayMeshResource = "Array<Resource<Mesh>>";
    public const string ArrayGLBResource = "Array<Resource<GLB>>";
    public const string ArrayTextureResource = "Array<Resource<Texture>>";
    public const string ArrayBlueprintResource = "Array<Resource<Blueprint>>";
    public const string ArrayGenericT = "Array<T>";
    public const string ArrayGenericU = "Array<U>";
    public const string ArrayGenericV = "Array<V>";
    public const string ArrayRig = "Array<Rig>";
    public const string ArrayMeshRenderer = "Array<MeshRenderer>";
    public const string ArrayMaterial = "Array<Material>";
    public const string ArraySceneNode = "Array<SceneNode>";
    public const string ArrayMeshConfig = "Array<MeshConfig>";
}