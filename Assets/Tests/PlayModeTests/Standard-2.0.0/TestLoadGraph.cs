// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using Futureverse.UBF.Runtime;
using Futureverse.UBF.Runtime.Execution;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestLoadGraph
{
	[Test]
	public void LoadGraphNoEntryNode()
	{
		var graph = TestGraph.EmptyGraph(BlueprintVersion.Version);
		Assert.IsTrue(Blueprint.TryLoad("LoadGraphNoEntryNode", graph, out var blueprint));
		_ = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		LogAssert.Expect(LogType.Error, "[UBF][DLL] Could not execute graph - no Entry node found");
	}

	[Test]
	public void LoadBasicGraph()
	{
		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			var debugLog = g.AddNode(new DebugLog("Hello, World!"));
			g.ConnectEntry(debugLog);
		});
		
		Assert.IsTrue(Blueprint.TryLoad("LoadBasicGraph", graph, out _));
	}
}