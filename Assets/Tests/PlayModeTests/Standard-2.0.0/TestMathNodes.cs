// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System.Collections;
using Futureverse.UBF.Runtime;
using Futureverse.UBF.Runtime.Execution;
using NUnit.Framework;
using UnitTests.PlayModeTests.Utils;
using UnityEngine;
using UnityEngine.TestTools;

public class TestMathNodes
{
	[TearDown]
	public void TearDown()
	{
		var go = GameObject.Find("Root");
		if (go != null)
			Object.Destroy(go);
	}
	
	public class MathTestCase<T>
	{
		public delegate void Evaluate(T actual);
		
		public string Type;
		public T Value1;
		public T Value2;
		public Node Node;
		public string NodeName;
		public Evaluate Evaluator;
	}

	private static MathTestCase<T> CreateAddTestCase<T>(string type, T value1, T value2, T expectedResult = default, MathTestCase<T>.Evaluate evaluator = null)
	{
		return new MathTestCase<T>()
		{
			Type = type,
			Value1 = value1,
			Value2 = value2,
			Evaluator = evaluator ?? (actual => { Assert.AreEqual(actual, expectedResult);}),
			Node = TestGraphBuilder.Add<T>("Add", type),
			NodeName = "Add",
		};
	}
	
	private static MathTestCase<T> CreateSubtractTestCase<T>(string type, T value1, T value2, T expectedResult = default, MathTestCase<T>.Evaluate evaluator = null)
	{
		return new MathTestCase<T>()
		{
			Type = type,
			Value1 = value1,
			Value2 = value2,
			Evaluator = evaluator ?? (actual => { Assert.AreEqual(actual, expectedResult);}),
			Node = TestGraphBuilder.Subtract<T>("Subtract", type),
			NodeName = "Subtract",
		};
	}
	
	private static MathTestCase<T> CreateMultiplyTestCase<T>(string type, T value1, T value2, T expectedResult = default, MathTestCase<T>.Evaluate evaluator = null)
	{
		return new MathTestCase<T>()
		{
			Type = type,
			Value1 = value1,
			Value2 = value2,
			Evaluator = evaluator ?? (actual => { Assert.AreEqual(actual, expectedResult);}),
			Node = TestGraphBuilder.Multiply<T>("Multiply", type),
			NodeName = "Multiply",
		};
	}

	private static IEnumerable MathTestCases()
	{
		yield return CreateAddTestCase("int", 12, 8, 20);
		yield return CreateAddTestCase("float", 123.45f, 0.6f, evaluator:(actual) => { Assert.AreEqual(actual, 124.05f, delta:0.01f); });
		yield return CreateSubtractTestCase("float", 1.25f, 2.25f, evaluator:(actual) => { Assert.AreEqual(actual, 1.25f - 2.25f, delta:0.01f); });
		yield return CreateSubtractTestCase("int", 123, 12, 111);
		yield return CreateMultiplyTestCase("int", 4, 10, 40);
		yield return CreateMultiplyTestCase("float", -1.5f, 3f, evaluator:(actual) => { Assert.AreEqual(actual, -4.5f, delta:0.01f); });
	}	
	
	[UnityTest]
	public IEnumerator TestMath<T>([ValueSource(nameof(MathTestCases))] MathTestCase<T> testCase)
	{
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<T>("value1", testCase.Type, testCase.Value1)
			.AddInputWithNode<T>("value2", testCase.Type, testCase.Value2)
			.AddOutputWithNode("Result", testCase.Type)
			.AddNode(testCase.Node)
			.ConnectEntry("Set_Result")
			.PassInputToNode("value1", testCase.NodeName, "A")
			.PassInputToNode("value2", testCase.NodeName, "B")
			.SetOutputFromNode(testCase.NodeName, "Result", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestMath", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryInterpretAs(out T mathResult));
		testCase.Evaluator(mathResult);
	}
	
	public class IncompatibleMathTestCase<T, U>
	{
		public string Type1;
		public string Type2;
		public T Value1;
		public U Value2;
		public Node Node;
		public string NodeName;
	}

	private static IncompatibleMathTestCase<T, U> CreateIncompatibleAddTestCase<T, U>(string type1, string type2, T value1, U value2)
	{
		return new IncompatibleMathTestCase<T, U>()
		{
			Type1 = type1,
			Type2 = type2,
			Value1 = value1,
			Value2 = value2,
			Node = TestGraphBuilder.Add<T>("Add", type1),
			NodeName = "Add",
		};
	}
	
	private static IncompatibleMathTestCase<T, U> CreateIncompatibleSubtractTestCase<T, U>(string type1, string type2, T value1, U value2)
	{
		return new IncompatibleMathTestCase<T, U>()
		{
			Type1 = type1,
			Type2 = type2,
			Value1 = value1,
			Value2 = value2,
			Node = TestGraphBuilder.Subtract<T>("Subtract", type1),
			NodeName = "Subtract",
		};
	}
	
	private static IncompatibleMathTestCase<T, U> CreateIncompatibleMultiplyTestCase<T, U>(string type1, string type2, T value1, U value2)
	{
		return new IncompatibleMathTestCase<T, U>()
		{
			Type1 = type1,
			Type2 = type2,
			Value1 = value1,
			Value2 = value2,
			Node = TestGraphBuilder.Multiply<T>("Multiply", type1),
			NodeName = "Multiply",
		};
	}
	
	private static IEnumerable IncompatibleMathTestCases()
	{
		yield return CreateIncompatibleAddTestCase("int", "string", 12, "Hello");
		yield return CreateIncompatibleSubtractTestCase("float", "int", 1.25f, 2);
		yield return CreateIncompatibleMultiplyTestCase("string", "string", "Hello", "World");
	}	
	
	[UnityTest]
	public IEnumerator TestAddIncompatibleTypes<T, U>([ValueSource(nameof(IncompatibleMathTestCases))] IncompatibleMathTestCase<T, U> testCase)
	{
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<T>("value1", testCase.Type1, testCase.Value1)
			.AddInputWithNode<U>("value2", "string", testCase.Value2)
			.AddOutputWithNode("Result", testCase.Type1)
			.AddNode(testCase.Node)
			.ConnectEntry("Set_Result")
			.PassInputToNode("value1", testCase.NodeName, "A")
			.PassInputToNode("value2", testCase.NodeName, "B")
			.SetOutputFromNode(testCase.NodeName, "Result", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestIncompatibleMath", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		LogAssert.Expect(LogType.Error, $"[UBF][DLL][{testCase.NodeName}] Input types cannot be operated on");
		yield return task;
	}
}