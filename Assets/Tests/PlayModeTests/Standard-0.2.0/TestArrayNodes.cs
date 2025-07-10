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
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<int>("Array", "Array<int>", list)
			.AddOutputWithNode("Result", "int")
			.AddNode(TestGraphBuilder.First<int>("First", "int"))
			.ConnectEntry("Set_Result")
			.PassInputToNode("Array", "First", "Array")
			.SetOutputFromNode("First", "Element", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestFirst", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryInterpretAs(out int first));
		Assert.AreEqual(first, list[0]);
	}
	
	[UnityTest]
	public IEnumerator TestFirstWithEmptyArray()
	{
		var list = new List<int>();
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<int>("Array", "Array<int>", list)
			.AddOutputWithNode("Result", "int")
			.AddNode(TestGraphBuilder.First<int>("First", "int"))
			.ConnectEntry("Set_Result")
			.PassInputToNode("Array", "First", "Array")
			.SetOutputFromNode("First", "Element", "Result")
			.Build();

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
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<int>("Array", "Array<int>", list)
			.AddOutputWithNode("Result", "int")
			.AddNode(TestGraphBuilder.AtIndex<int>("AtIndex", "int", index))
			.ConnectEntry("Set_Result")
			.PassInputToNode("Array", "AtIndex", "Array")
			.SetOutputFromNode("AtIndex", "Element", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestAtIndex", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
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
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<int>("Array", "Array<int>", list)
			.AddOutputWithNode("Result", "int")
			.AddNode(TestGraphBuilder.AtIndex<int>("AtIndex", "int", index))
			.ConnectEntry("Set_Result")
			.PassInputToNode("Array", "AtIndex", "Array")
			.SetOutputFromNode("AtIndex", "Element", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestAtIndexOutOfBounds", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		LogAssert.Expect(LogType.Error, "[UBF][DLL][AtIndex] Index out of bounds!");
		yield return task;
		
	}
	
	[UnityTest]
	public IEnumerator TestMakeArray()
	{
		const string item1 = "Hello";
		const string item2 = "World";
		var graph = new TestGraphBuilder(BlueprintVersion.Version).AddInputWithNode<string>("Item1", "string", item1)
			.AddInputWithNode<string>("Item2", "string", item2)
			.AddOutputWithNode("Result", "Array<string>")
			.AddNode(TestGraphBuilder.MakeArray("MakeArray", "string", TestUtils.DefaultList<string>(2)))
			.ConnectEntry("Set_Result")
			.PassInputToNode("Item1", "MakeArray", "Item.1")
			.PassInputToNode("Item2", "MakeArray", "Item.2")
			.SetOutputFromNode("MakeArray", "Array", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestMakeArray", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryReadArray(out List<string> array));
		CollectionAssert.Contains(array, item1);
		CollectionAssert.Contains(array, item2);
	}
	
	[UnityTest]
	public IEnumerator TestForeach()
	{
		var list = new List<string>
		{
			"Hello",
			"World",
		};
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<string>("Array", "Array<string>", list)
			.AddNode(TestGraphBuilder.ForEach<string>("ForEach", "string"))
			.AddNode(TestGraphBuilder.DebugLog("DebugLog1"))
			.AddNode(TestGraphBuilder.DebugLog("DebugLog2"))
			.AddNode(TestGraphBuilder.ToString<int>("ToString", "int"))
			.ConnectEntry("ForEach")
			.PassInputToNode("Array", "ForEach", "Array")
			.ConnectNodes("ForEach", "Loop Body", "DebugLog1", "Exec")
			.ConnectNodes("ForEach", "Array Element", "DebugLog1", "Message")
			.ConnectExecution("DebugLog1", "DebugLog2")
			.ConnectNodes("ForEach", "Array Index", "ToString", "Value")
			.ConnectNodes("ToString", "String", "DebugLog2", "Message")
			.Build();

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

	private static Node TestWaitForFramesNode(string id)
		=> new()
		{
			Id = id,
			Type = "TestWaitForFrames",
			Inputs = new List<Pin>
			{
				new()
				{
					Id = "Exec",
					Type = "exec",
					Value = null,
				},
			},
			Outputs = new List<Pin>
			{
				new()
				{
					Id = "Exec",
					Type = "exec",
				},
			},
		};
	
	[UnityTest]
	public IEnumerator TestForeachAsync()
	{
		var list = new List<string>
		{
			"Hello",
			"World",
		};
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<string>("Array", "Array<string>", list)
			.AddNode(TestGraphBuilder.ForEach<string>("ForEach", "string"))
			.AddNode(TestGraphBuilder.DebugLog("DebugLog1"))
			.AddNode(TestGraphBuilder.DebugLog("DebugLog2", "Finished"))
			.AddNode(TestWaitForFramesNode("WaitForFrames"))
			.ConnectEntry("ForEach")
			.PassInputToNode("Array", "ForEach", "Array")
			.ConnectNodes("ForEach", "Loop Body", "WaitForFrames", "Exec")
			.ConnectExecution("WaitForFrames", "DebugLog1")
			.ConnectNodes("ForEach", "Array Element", "DebugLog1", "Message")
			.ConnectExecution("ForEach", "DebugLog2")
			.Build();

		// Add TestWaitForFrames to registry so we can use it
		var task = new TestEnvBuilder(graph)
			.RegisterAdditionalNode<TestWaitForFrames>()
			.Build();
		yield return task;
		
		foreach (var element in list)
		{
			LogAssert.Expect(LogType.Log, $"[UBF] {element}");
		}
		
		LogAssert.Expect(LogType.Log, "[UBF] Finished");
	}
}