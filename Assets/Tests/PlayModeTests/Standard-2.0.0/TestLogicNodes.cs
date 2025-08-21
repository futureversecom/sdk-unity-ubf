// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Futureverse.UBF.Runtime;
using Futureverse.UBF.Runtime.Execution;
using NUnit.Framework;
using UnitTests.PlayModeTests.Utils;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;
using Object = UnityEngine.Object;

public class TestLogicNodes
{
	[TearDown]
	public void TearDown()
	{
		var go = GameObject.Find("Root");
		if (go != null)
			Object.Destroy(go);
	}
	
	[UnityTest]
	public IEnumerator TestAnd()
	{
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddOutputWithNode("Result1", "boolean")
			.AddOutputWithNode("Result2", "boolean")
			.AddOutputWithNode("Result3", "boolean")
			.AddNode(TestGraphBuilder.And("And1", true, true))
			.AddNode(TestGraphBuilder.And("And2", true, false))
			.AddNode(TestGraphBuilder.And("And3", false, false))
			.ConnectEntry("Set_Result1")
			.ConnectExecution("Set_Result1", "Set_Result2")
			.ConnectExecution("Set_Result2", "Set_Result3")
			.SetOutputFromNode("And1", "Result", "Result1")
			.SetOutputFromNode("And2", "Result", "Result2")
			.SetOutputFromNode("And3", "Result", "Result3")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestAnd", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result1", out var result1));
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result2", out var result2));
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result3", out var result3));
		Assert.IsTrue(result1.TryInterpretAs(out bool bool1));
		Assert.IsTrue(result2.TryInterpretAs(out bool bool2));
		Assert.IsTrue(result3.TryInterpretAs(out bool bool3));
		Assert.IsTrue(bool1);
		Assert.IsFalse(bool2);
		Assert.IsFalse(bool3);
	}
	
	[UnityTest]
	public IEnumerator TestOr()
	{
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddOutputWithNode("Result1", "boolean")
			.AddOutputWithNode("Result2", "boolean")
			.AddOutputWithNode("Result3", "boolean")
			.AddNode(TestGraphBuilder.Or("Or1", true, true))
			.AddNode(TestGraphBuilder.Or("Or2", true, false))
			.AddNode(TestGraphBuilder.Or("Or3", false, false))
			.ConnectEntry("Set_Result1")
			.ConnectExecution("Set_Result1", "Set_Result2")
			.ConnectExecution("Set_Result2", "Set_Result3")
			.SetOutputFromNode("Or1", "Result", "Result1")
			.SetOutputFromNode("Or2", "Result", "Result2")
			.SetOutputFromNode("Or3", "Result", "Result3")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestOr", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result1", out var result1));
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result2", out var result2));
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result3", out var result3));
		Assert.IsTrue(result1.TryInterpretAs(out bool bool1));
		Assert.IsTrue(result2.TryInterpretAs(out bool bool2));
		Assert.IsTrue(result3.TryInterpretAs(out bool bool3));
		Assert.IsTrue(bool1);
		Assert.IsTrue(bool2);
		Assert.IsFalse(bool3);
	}
	
	[UnityTest]
	public IEnumerator TestXor()
	{
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddOutputWithNode("Result1", "boolean")
			.AddOutputWithNode("Result2", "boolean")
			.AddOutputWithNode("Result3", "boolean")
			.AddNode(TestGraphBuilder.Xor("Xor1", true, true))
			.AddNode(TestGraphBuilder.Xor("Xor2", true, false))
			.AddNode(TestGraphBuilder.Xor("Xor3", false, false))
			.ConnectEntry("Set_Result1")
			.ConnectExecution("Set_Result1", "Set_Result2")
			.ConnectExecution("Set_Result2", "Set_Result3")
			.SetOutputFromNode("Xor1", "Result", "Result1")
			.SetOutputFromNode("Xor2", "Result", "Result2")
			.SetOutputFromNode("Xor3", "Result", "Result3")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestXor", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result1", out var result1));
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result2", out var result2));
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result3", out var result3));
		Assert.IsTrue(result1.TryInterpretAs(out bool bool1));
		Assert.IsTrue(result2.TryInterpretAs(out bool bool2));
		Assert.IsTrue(result3.TryInterpretAs(out bool bool3));
		Assert.IsFalse(bool1);
		Assert.IsTrue(bool2);
		Assert.IsFalse(bool3);
	}
	
	[UnityTest]
	public IEnumerator TestNot()
	{
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddOutputWithNode("Result1", "boolean")
			.AddOutputWithNode("Result2", "boolean")
			.AddNode(TestGraphBuilder.Not("Not1",true))
			.AddNode(TestGraphBuilder.Not("Not2",false))
			.ConnectEntry("Set_Result1")
			.ConnectExecution("Set_Result1", "Set_Result2")
			.SetOutputFromNode("Not1", "Result", "Result1")
			.SetOutputFromNode("Not2", "Result", "Result2")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestNot", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result1", out var result1));
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result2", out var result2));
		Assert.IsTrue(result1.TryInterpretAs(out bool bool1));
		Assert.IsTrue(result2.TryInterpretAs(out bool bool2));
		Assert.IsFalse(bool1);
		Assert.IsTrue(bool2);
	}
	
	public class ComparisonTestCase<T> where T: IComparable<T>
	{
		public delegate bool CompareDelegate(T a, T b);
		
		public string Type;
		public T Value1;
		public T Value2;
		public Node Node;
		public string NodeName;
		public CompareDelegate Comparer;
	}

	private static ComparisonTestCase<T> CreateGreaterThanTestCase<T>(string type, T value1, T value2)
		where T : IComparable<T>
	{
		return new ComparisonTestCase<T>()
		{
			Type = type,
			Value1 = value1,
			Value2 = value2,
			Node = TestGraphBuilder.GreaterThan<T>("GreaterThan", type),
			NodeName = "GreaterThan",
			Comparer = (a, b) => a.CompareTo(b) > 0,
		};
	}

	private static ComparisonTestCase<T> CreateGreaterThanOrEqualsTestCase<T>(string type, T value1, T value2)
		where T : IComparable<T>
	{
		return new ComparisonTestCase<T>()
		{
			Type = type,
			Value1 = value1,
			Value2 = value2,
			Node = TestGraphBuilder.GreaterThanOrEquals<T>("GreaterThanOrEqual", type),
			NodeName = "GreaterThanOrEqual",
			Comparer = (a, b) => a.CompareTo(b) >= 0,
		};
	}

	private static ComparisonTestCase<T> CreateEqualsTestCase<T>(string type, T value1, T value2)
		where T : IComparable<T>
	{
		return new ComparisonTestCase<T>()
		{
			Type = type,
			Value1 = value1,
			Value2 = value2,
			Node = TestGraphBuilder.Equals<T>("Equals", type),
			NodeName = "Equals",
			Comparer = (a, b) => a.CompareTo(b) == 0,
		};
	}

	private static ComparisonTestCase<T> CreateNotEqualsTestCase<T>(string type, T value1, T value2)
		where T : IComparable<T>
	{
		return new ComparisonTestCase<T>()
		{
			Type = type,
			Value1 = value1,
			Value2 = value2,
			Node = TestGraphBuilder.NotEquals<T>("NotEquals", type),
			NodeName = "NotEquals",
			Comparer = (a, b) => a.CompareTo(b) != 0,
		};
	}

	private static ComparisonTestCase<T> CreateLessThanTestCase<T>(string type, T value1, T value2)
		where T : IComparable<T>
	{
		return new ComparisonTestCase<T>()
		{
			Type = type,
			Value1 = value1,
			Value2 = value2,
			Node = TestGraphBuilder.LessThan<T>("LessThan", type),
			NodeName = "LessThan",
			Comparer = (a, b) => a.CompareTo(b) < 0,
		};
	}

	private static ComparisonTestCase<T> CreateLessThanOrEqualsTestCase<T>(string type, T value1, T value2)
		where T : IComparable<T>
	{
		return new ComparisonTestCase<T>()
		{
			Type = type,
			Value1 = value1,
			Value2 = value2,
			Node = TestGraphBuilder.LessThanOrEquals<T>("LessThanOrEquals", type),
			NodeName = "LessThanOrEquals",
			Comparer = (a, b) => a.CompareTo(b) <= 0,
		};
	}
	
	private static IEnumerable ComparisonTestCases()
	{
		yield return CreateGreaterThanTestCase("int", 10, 5);
		yield return CreateGreaterThanTestCase("int", 5, 10);
		yield return CreateGreaterThanTestCase("float", 1.75f, 1.25f);
		yield return CreateGreaterThanTestCase("float", 1.5f, 1.50f);

		yield return CreateGreaterThanOrEqualsTestCase("int", 10, 10);
		yield return CreateGreaterThanOrEqualsTestCase("int", 20, 10);
		yield return CreateGreaterThanOrEqualsTestCase("float", 1.1f, 1.15);
		yield return CreateGreaterThanOrEqualsTestCase("float", 100.2f, 1.002f);

		yield return CreateEqualsTestCase("boolean", true, false);
		yield return CreateEqualsTestCase("boolean", true, true);
		yield return CreateEqualsTestCase("string", "Hello", "hello");
		yield return CreateEqualsTestCase("string", "World", "World");
		yield return CreateEqualsTestCase("int", 10, 10);
		yield return CreateEqualsTestCase("int", 1, 11);
		yield return CreateEqualsTestCase("float", 1f, 1.0f);
		yield return CreateEqualsTestCase("float", 5.5f, 5.05f);
		
		yield return CreateNotEqualsTestCase("string", "Hello", "hello");
		yield return CreateNotEqualsTestCase("string", "World", "World");
		yield return CreateNotEqualsTestCase("int", 10, 10);
		yield return CreateNotEqualsTestCase("int", 1, 11);
		yield return CreateNotEqualsTestCase("float", 1f, 1.0f);
		yield return CreateNotEqualsTestCase("float", 5.5f, 5.05f);
		
		yield return CreateLessThanTestCase("int", 10, 5);
		yield return CreateLessThanTestCase("int", 5, 10);
		yield return CreateLessThanTestCase("float", 1.75f, 1.25f);
		yield return CreateLessThanTestCase("float", 1.5f, 1.50f);

		yield return CreateLessThanOrEqualsTestCase("int", 10, 10);
		yield return CreateLessThanOrEqualsTestCase("int", 20, 10);
		yield return CreateLessThanOrEqualsTestCase("float", 1.1f, 1.15);
		yield return CreateLessThanOrEqualsTestCase("float", 100.2f, 1.002f);
	}
	
	[UnityTest]
	public IEnumerator TestComparisonNodes<T>([ValueSource(nameof(ComparisonTestCases))] ComparisonTestCase<T> testCase) where T: IComparable<T>
	{
		var expectedResult = testCase.Comparer(testCase.Value1, testCase.Value2);
		
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<int>("Value1", testCase.Type, testCase.Value1)
			.AddInputWithNode<int>("Value2", testCase.Type, testCase.Value2)
			.AddOutputWithNode("Result", "boolean")
			.AddNode(testCase.Node)
			.ConnectEntry("Set_Result")
			.PassInputToNode("Value1", testCase.NodeName, "A")
			.PassInputToNode("Value2", testCase.NodeName, "B")
			.SetOutputFromNode(testCase.NodeName, "Result", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestComparisonNode", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryInterpretAs(out bool resultBool));
		Assert.AreEqual(resultBool, expectedResult);
	}
	
	[UnityTest]
	public IEnumerator TestBranch()
	{
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddOutputWithNode("Result", "string", "Hello")
			.AddNode(TestGraphBuilder.Branch("Branch1", true))
			.AddNode(TestGraphBuilder.Branch("Branch2", false))
			.ConnectEntry("Branch1")
			.ConnectNodes("Branch1", "True", "Branch2", "Exec")
			.ConnectNodes("Branch2", "False", "Set_Result", "Exec")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestBranch", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryInterpretAs(out string resultString));
		Assert.AreEqual(resultString, "Hello");
	}
	
	[UnityTest]
	public IEnumerator TestPick()
	{
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddOutputWithNode("Result1", "string")
			.AddOutputWithNode("Result2", "int")
			.AddNode(TestGraphBuilder.Pick("Pick1", "string", "Hello", "World", true))
			.AddNode(TestGraphBuilder.Pick("Pick2", "int", 1, 2,false))
			.ConnectEntry("Set_Result1")
			.ConnectExecution("Set_Result1", "Set_Result2")
			.SetOutputFromNode("Pick1", "Result", "Result1")
			.SetOutputFromNode("Pick2", "Result", "Result2")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestPick", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result1", out var result1));
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result2", out var result2));
		Assert.IsTrue(result1.TryInterpretAs(out string resultString));
		Assert.IsTrue(result2.TryInterpretAs(out int resultInt));
		Assert.AreEqual(resultString, "Hello");
		Assert.AreEqual(resultInt, 2);
	}
	
	[UnityTest]
	public IEnumerator TestMapValue()
	{
		var dict = new Dictionary<int, string>
		{
			{
				4, "Four"
			},
			{
				5, "Five"
			},
			{
				6, "Six"
			},
		};

		const int input = 4;
		var expectedResult = dict[input];
		
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddOutputWithNode("Result", "string")
			.AddNode(TestGraphBuilder.MapValue("MapValue", "int", "string", input, "Default", dict.Keys.ToList(), dict.Values.ToList()))
			.ConnectEntry("Set_Result")
			.SetOutputFromNode("MapValue", "Result", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestMapValue", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryInterpretAs(out string resultString));
		Assert.AreEqual(resultString, expectedResult);
	}
	
	[UnityTest]
	public IEnumerator TestIsNullSceneNode()
	{
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddOutputWithNode("Result", "boolean")
			.AddNode(TestGraphBuilder.CreateSceneNode("CreateSceneNode", "Node"))
			.AddNode(TestGraphBuilder.IsNull<object>("IsNull", "SceneNode"))
			.ConnectEntry("CreateSceneNode")
			.ConnectExecution("CreateSceneNode", "Set_Result")
			.ConnectNodes("CreateSceneNode", "SceneNode", "IsNull", "Value")
			.SetOutputFromNode("IsNull", "IsNull", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestIsNull", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryInterpretAs(out bool resultBool));
		Assert.IsFalse(resultBool);
	}
	
	[UnityTest]
	public IEnumerator TestIsNullSceneNodeInput()
	{
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<object>("SceneNode", "SceneNode")
			.AddOutputWithNode("Result", "boolean")
			.AddNode(TestGraphBuilder.IsNull<object>("IsNull", "SceneNode"))
			.ConnectEntry("Set_Result")
			.PassInputToNode("SceneNode", "IsNull", "Value")
			.SetOutputFromNode("IsNull", "IsNull", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestIsNull", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryInterpretAs(out bool resultBool));
		Assert.IsTrue(resultBool);
	}
}