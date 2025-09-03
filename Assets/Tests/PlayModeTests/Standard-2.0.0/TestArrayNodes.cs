// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using Futureverse.UBF.Runtime;
using Futureverse.UBF.Runtime.Execution;
using NUnit.Framework;
using UnitTests.PlayModeTests.Utils;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

public class TestArrayNodes
{
	[TearDown]
	public void TearDown()
	{
		var go = GameObject.Find("Root");
		if (go != null)
			Object.Destroy(go);
	}
	
	[UnityTest]
	public IEnumerator TestFirst()
	{
		var list = new List<int>
		{
			1,
			2,
			3,
			4,
		};
		const string inputName = "Array";
		const string outputName = "Result";

		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			_ = g.AddInputWithNode<List<int>>(inputName, UBFTypes.ArrayInt, list);
			var setResultNode = g.AddOutputWithNode(outputName, UBFTypes.Int);
			var firstNode = g.AddNode(new First<int>(UBFTypes.Int));
			g.ConnectEntry(setResultNode);
			g.PassInputToNode(inputName, firstNode, First<int>.In.Array);
			g.SetOutputFromNode(firstNode, First<int>.Out.Element, outputName);
		});

		Assert.IsTrue(Blueprint.TryLoad("TestFirst", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput(outputName, out var result));
		Assert.IsTrue(result.TryInterpretAs(out int first));
		Assert.AreEqual(first, list[0]);
	}
	
	[UnityTest]
	public IEnumerator TestFirstWithEmptyArray()
	{
		var list = new List<int>();
		const string inputName = "Array";
		const string outputName = "Result";
		
		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			_ = g.AddInputWithNode<List<int>>(inputName, UBFTypes.ArrayInt, list);
			var setResultNode = g.AddOutputWithNode(outputName, UBFTypes.Int);
			var firstNode = g.AddNode(new First<int>(UBFTypes.Int));
			g.ConnectEntry(setResultNode);
			g.PassInputToNode(inputName, firstNode, First<int>.In.Array);
			g.SetOutputFromNode(firstNode, First<int>.Out.Element, outputName);
		});
	
		Assert.IsTrue(Blueprint.TryLoad("TestFirstWithEmptyArray", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		
		// No idea why but you have to put this Assert before yielding on the task...
		LogAssert.Expect(LogType.Error, "[UBF][DLL][First] Array is empty");
		yield return task;
	}
	
	[UnityTest]
	public IEnumerator TestAtIndex()
	{
		var list = new List<int>
		{
			1,
			2,
			3,
			4,
		};
		const int index = 1;
		const string inputName = "Array";
		const string outputName = "Result";

		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			g.AddInputWithNode<List<int>>(inputName, UBFTypes.ArrayInt, list);
			var setResultNode = g.AddOutputWithNode(outputName, UBFTypes.Int);
			var atIndexNode = g.AddNode(new AtIndex<int>(UBFTypes.Int, index));
			g.ConnectEntry(setResultNode);
			g.PassInputToNode(inputName, atIndexNode, AtIndex<int>.In.Array);
			g.SetOutputFromNode(atIndexNode, AtIndex<int>.Out.Element, outputName);
		});
	
		Assert.IsTrue(Blueprint.TryLoad("TestAtIndex", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput(outputName, out var result));
		Assert.IsTrue(result.TryInterpretAs(out int atIndex));
		Assert.AreEqual(atIndex, list[index]);
	}
	
	[UnityTest]
	public IEnumerator TestAtIndexOutOfBounds()
	{
		var list = new List<int>
		{
			1,
			2,
			3,
		};
		const int index = 5;
		const string inputName = "Array";
		const string outputName = "Result";

		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			g.AddInputWithNode<List<int>>(inputName, UBFTypes.ArrayInt, list);
			var setResultNode = g.AddOutputWithNode(outputName, UBFTypes.Int);
			var atIndexNode = g.AddNode(new AtIndex<int>(UBFTypes.Int, index));
			g.ConnectEntry(setResultNode);
			g.PassInputToNode(inputName, atIndexNode, AtIndex<int>.In.Array);
			g.SetOutputFromNode(atIndexNode, AtIndex<int>.Out.Element, outputName);
		});
	
		Assert.IsTrue(Blueprint.TryLoad("TestAtIndexOutOfBounds", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		LogAssert.Expect(LogType.Error, "[UBF][DLL][AtIndex] Index out of bounds!");
		yield return task;
		
	}
	
	[UnityTest]
	public IEnumerator TestMakeArray()
	{
		const string input1Name = "Item1";
		const string input2Name = "Item2";
		const string inputValue1 = "Hello";
		const string inputValue2 = "World";
		const string outputName = "Result";

		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			g.AddInputWithNode<string>(input1Name, UBFTypes.String, inputValue1);
			g.AddInputWithNode<string>(input2Name, UBFTypes.String, inputValue2);
			var setResultNode = g.AddOutputWithNode(outputName, UBFTypes.String);
			var makeArrayNode = g.AddNode(new MakeArray<string>(UBFTypes.String, TestUtils.DefaultList<string>(2)));
			g.ConnectEntry(setResultNode);
			g.PassInputToNode(input1Name, makeArrayNode, "Element.1");
			g.PassInputToNode(input2Name, makeArrayNode, "Element.2");
			g.SetOutputFromNode(makeArrayNode, MakeArray<string>.Out.Array, outputName);
		});
	
		Assert.IsTrue(Blueprint.TryLoad("TestMakeArray", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput(outputName, out var result));
		Assert.IsTrue(result.TryReadArray(out List<string> array));
		CollectionAssert.Contains(array, inputValue1);
		CollectionAssert.Contains(array, inputValue2);
	}
	
	[UnityTest]
	public IEnumerator TestForeach()
	{
		var list = new List<string>
		{
			"Hello",
			"World",
		};
		const string inputName = "Array";
		
		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			g.AddInputWithNode<List<string>>(inputName, UBFTypes.ArrayString, list);
			var debugLogNode1 = g.AddNode(new DebugLog());
			var debugLogNode2 = g.AddNode(new DebugLog());
			var forEachNode = g.AddNode(new ForEach<string>(UBFTypes.String));
			var toStringNode = g.AddNode(new ToString<int>(UBFTypes.Int));
			g.ConnectEntry(forEachNode);
			g.PassInputToNode(inputName, forEachNode, ForEach<string>.In.Array);
			g.ConnectNodes(forEachNode, ForEach<string>.Out.Loop, debugLogNode1, DebugLog.In.Exec);
			g.ConnectNodes(forEachNode, ForEach<string>.Out.Element, debugLogNode1, DebugLog.In.Message);
			g.ConnectExecution(debugLogNode1, debugLogNode2);
			g.ConnectNodes(forEachNode, ForEach<string>.Out.Index, toStringNode, ToString<int>.In.Value);
			g.ConnectNodes(toStringNode, ToString<string>.Out.String, debugLogNode2, DebugLog.In.Message);
		});
	
		Assert.IsTrue(Blueprint.TryLoad("TestForeach", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		for (var i = 0; i < list.Count; i++)
		{
			LogAssert.Expect(LogType.Log, $"[UBF] {list[i]}");
			LogAssert.Expect(LogType.Log, $"[UBF] {i}");
		}
	}
	
	public class TestWaitForFrames : ACustomExecNode
	{
		public TestWaitForFrames(Context context) : base(context) { }
	
		protected override IEnumerator ExecuteAsync()
		{
			yield return null;
			yield return null;
			yield return null;
		}
	}
	
	public class WaitForFrames : Node
	{
		public struct In
		{
			public const string Exec = "Exec";
		}

		public struct Out
		{
			public const string Exec = "Exec";
		}
		
		public WaitForFrames()
		{
			Id = System.Guid.NewGuid().ToString();
			Type = "TestWaitForFrames";
			Inputs = new List<Pin>
			{
				new()
				{
					Id = "Exec",
					Type = "exec",
					Value = null,
				},
			};
			Outputs = new List<Pin>
			{
				new()
				{
					Id = "Exec",
					Type = "exec",
				},
			};
		}
	};
	
	[UnityTest]
	public IEnumerator TestForeachAsync()
	{
		var list = new List<string>
		{
			"Hello",
			"World",
		};
		const string inputName = "Array";
		
		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			g.AddInputWithNode<List<string>>(inputName, UBFTypes.ArrayString, list);
			var debugLogNode1 = g.AddNode(new DebugLog());
			var debugLogNode2 = g.AddNode(new DebugLog("Finished"));
			var forEachNode = g.AddNode(new ForEach<string>(UBFTypes.String));
			var waitForFramesNode = g.AddNode(new WaitForFrames());
			g.ConnectEntry(forEachNode);
			g.PassInputToNode(inputName, forEachNode, ForEach<string>.In.Array);
			g.ConnectNodes(forEachNode, ForEach<string>.Out.Loop, waitForFramesNode, WaitForFrames.In.Exec);
			g.ConnectExecution(waitForFramesNode, debugLogNode1);
			g.ConnectNodes(forEachNode, ForEach<string>.Out.Element, debugLogNode1, DebugLog.In.Message);
			g.ConnectExecution(forEachNode, debugLogNode2);
		});
	
		// Add TestWaitForFrames to registry so we can use it
		var task = TestEnv.Create(graph, (ref TestEnv e) =>
		{
			e.AddToRegistry<TestWaitForFrames>();
		});
		
		yield return task;
		
		foreach (var element in list)
		{
			LogAssert.Expect(LogType.Log, $"[UBF] {element}");
		}
		
		LogAssert.Expect(LogType.Log, "[UBF] Finished");
	}
}