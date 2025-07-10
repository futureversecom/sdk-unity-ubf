// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using Futureverse.UBF.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestGraphs
{
	[Test]
	public void LoadEmptyGraph()
	{
		Assert.IsFalse(Blueprint.TryLoad("LoadEmptyGraph", "", out _));
		LogAssert.Expect(LogType.Error, "[UBF][DLL] Failed to load graph: EOF while parsing a value at line 1 column 0");
	}
	
	[Test]
	public void LoadUnsupportedGraph()
	{
		const string graphJson = "{\"version\":\"0.0.0\",\"nodes\":[{\"id\":\"e\",\"type\":\"Entry\",\"inputs\":[],\"outputs\":[{\"id\":\"Exec\",\"type\":\"exec\",\"value\":null}]}],\"connections\":[],\"bindings\":[],\"fns\":{}}";
		Assert.IsFalse(Blueprint.TryLoad("LoadUnsupportedGraph", graphJson, out _));
		LogAssert.Expect(LogType.Error, "[UBF] Cannot load blueprint with unsupported standard version (0.0.0)");
	}
}