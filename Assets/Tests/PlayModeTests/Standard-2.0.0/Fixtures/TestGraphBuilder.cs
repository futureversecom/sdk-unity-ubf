// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace UnitTests.PlayModeTests.Utils
{
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
	public struct Node
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
	public partial class TestGraphBuilder
	{
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

		public TestGraphBuilder AddNode(Node node)
		{
			_nodes.Add(node);
			return this;
		}

		public TestGraphBuilder ConnectNodes(
			string fromNode,
			string fromPin,
			string toNode,
			string toPin)
		{
			_connections.Add(
				new Connection()
				{
					Source = fromNode,
					SourceKey = fromPin,
					Target = toNode,
					TargetKey = toPin,
				}
			);
			return this;
		}

		public TestGraphBuilder ConnectExecution(string fromNode, string toNode)
		{
			return ConnectNodes(
				fromNode,
				"Exec",
				toNode,
				"Exec"
			);
		}

		public TestGraphBuilder ConnectEntry(string toNode, string toPin = "Exec")
		{
			return ConnectNodes(
				"e",
				"Exec",
				toNode,
				toPin
			);
		}

		private TestGraphBuilder AddBinding(string id, string type, string scope, object defaultValue = null)
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
			return this;
		}

		public TestGraphBuilder AddInput(string id, string type, object defaultValue = null)
		{
			return AddBinding(id, type, "input", defaultValue);
		}
		
		public TestGraphBuilder AddOutput(string id, string type)
		{
			return AddBinding(id, type, "output");
		}

		public TestGraphBuilder AddInputWithNode<T>(string id, string type, object defaultValue = null)
		{
			return AddInput(id, type, defaultValue)
				.AddNode(Get<T>($"Get_{id}", type, id));
		}
		
		public TestGraphBuilder AddOutputWithNode(string id, string type, object value = null)
		{
			return AddOutput(id, type)
				.AddNode(Set($"Set_{id}", id, value));
		}

		public TestGraphBuilder PassInputToNode(string inputId, string toNode, string toPin)
		{
			return ConnectNodes($"Get_{inputId}", "Value", toNode, toPin);
		}
		
		public TestGraphBuilder SetOutputFromNode(string fromNode, string fromPin, string outputName)
		{
			return ConnectNodes(fromNode, fromPin, $"Set_{outputName}", "Value");
		}
		
		public TestGraphBuilder AddLocalVariable(string id, string type, object defaultValue = null)
		{
			return AddBinding(id, type, "var", defaultValue);
		}

		public TestGraphBuilder(string version)
		{
			if (!Version.TryParse(version, out _))
			{
				throw new Exception("Invalid version for test graph builder");
			}

			_version = version;
			_nodes.Add(Entry("e"));
		}

		// TODO: Implement function builder
		public TestGraphBuilder AddFunction(string id)
		{
			throw new NotImplementedException();
		}

		public static string EmptyGraph(string version)
		{
			var builder = new TestGraphBuilder(version);
			builder._nodes.Clear();
			return builder.Build();
		}

		public string Build()
		{
			var output = JsonConvert.SerializeObject(this);
			Debug.Log(output);
			return output;
		}
	}
}