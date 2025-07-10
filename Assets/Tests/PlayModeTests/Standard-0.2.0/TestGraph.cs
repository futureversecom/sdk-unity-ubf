// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using Futureverse.UBF.Runtime;
using Futureverse.UBF.Runtime.Execution;
using NUnit.Framework;
using UnitTests.PlayModeTests.Utils;
using UnityEngine;
using UnityEngine.TestTools;

public class TestGraph
{
	[Test]
	public void LoadGraphNoEntryNode()
	{
		var graphJson = TestGraphBuilder.EmptyGraph(BlueprintVersion.Version);
		Assert.IsTrue(Blueprint.TryLoad("LoadGraphNoEntryNode", graphJson, out var blueprint));
		_ = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		LogAssert.Expect(LogType.Error, "[UBF][DLL] Could not execute graph - no Entry node found");
	}

	[Test]
	public void LoadBasicGraph()
	{
		var graphJson = new TestGraphBuilder(BlueprintVersion.Version)
			.AddNode(TestGraphBuilder.DebugLog("dbl", "Hello, World!"))
			.ConnectEntry("dbl")
			.Build();
		Assert.IsTrue(Blueprint.TryLoad("LoadBasicGraph", graphJson, out _));
	}
}