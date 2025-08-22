// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System.Collections;
using Futureverse.UBF.Runtime;
using Futureverse.UBF.Runtime.Execution;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// ReSharper disable InconsistentNaming

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
			Node = new Add<T>(type),
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
			Node = new Subtract<T>(type),
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
			Node = new Multiply<T>(type),
		};
	}

	private static IEnumerable MathTestCases()
	{
		yield return CreateAddTestCase(UBFTypes.Int, 12, 8, 20);
		yield return CreateAddTestCase(UBFTypes.Float, 123.45f, 0.6f, evaluator:(actual) => { Assert.AreEqual(actual, 124.05f, delta:0.01f); });
		yield return CreateSubtractTestCase(UBFTypes.Float, 1.25f, 2.25f, evaluator:(actual) => { Assert.AreEqual(actual, 1.25f - 2.25f, delta:0.01f); });
		yield return CreateSubtractTestCase(UBFTypes.Int, 123, 12, 111);
		yield return CreateMultiplyTestCase(UBFTypes.Int, 4, 10, 40);
		yield return CreateMultiplyTestCase(UBFTypes.Float, -1.5f, 3f, evaluator:(actual) => { Assert.AreEqual(actual, -4.5f, delta:0.01f); });
	}	
	
	[UnityTest]
	public IEnumerator TestMath<T>([ValueSource(nameof(MathTestCases))] MathTestCase<T> testCase)
	{
		const string value1 = "value1";
		const string value2 = "value2";
		const string outputName = "Result";
		
		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			g.AddInputWithNode<T>(value1, testCase.Type, testCase.Value1);
			g.AddInputWithNode<T>(value2, testCase.Type, testCase.Value2);
			var setOutputNode = g.AddOutputWithNode(outputName, testCase.Type);
			var node = g.AddNode(testCase.Node);
			g.ConnectEntry(setOutputNode);
			// Not using Add specifically, but using the static class to get the input/output names
			g.PassInputToNode(value1, node, Add<T>.In.A);
			g.PassInputToNode(value2, node, Add<T>.In.B);
			g.SetOutputFromNode(node, Add<T>.Out.Result, outputName);
		});

		Assert.IsTrue(Blueprint.TryLoad("TestMath", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput(outputName, out var result));
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
	}

	private static IncompatibleMathTestCase<T, U> CreateIncompatibleAddTestCase<T, U>(string type1, string type2, T value1, U value2)
	{
		return new IncompatibleMathTestCase<T, U>()
		{
			Type1 = type1,
			Type2 = type2,
			Value1 = value1,
			Value2 = value2,
			Node = new Add<T>(type1),
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
			Node = new Subtract<T>(type1),
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
			Node = new Multiply<T>(type1),
		};
	}
	
	private static IEnumerable IncompatibleMathTestCases()
	{
		yield return CreateIncompatibleAddTestCase(UBFTypes.Int, UBFTypes.String, 12, "Hello");
		yield return CreateIncompatibleSubtractTestCase(UBFTypes.Float, UBFTypes.Int, 1.25f, 2);
		yield return CreateIncompatibleMultiplyTestCase(UBFTypes.String, UBFTypes.String, "Hello", "World");
	}	
	
	[UnityTest]
	public IEnumerator TestAddIncompatibleTypes<T, U>([ValueSource(nameof(IncompatibleMathTestCases))] IncompatibleMathTestCase<T, U> testCase)
	{
		const string value1 = "value1";
		const string value2 = "value2";
		const string outputName = "Result";
		
		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			g.AddInputWithNode<T>(value1, testCase.Type1, testCase.Value1);
			g.AddInputWithNode<T>(value2, testCase.Type2, testCase.Value2);
			var setOutputNode = g.AddOutputWithNode(outputName, testCase.Type1);
			var node = g.AddNode(testCase.Node);
			g.ConnectEntry(setOutputNode);
			// Not using Add specifically, but using the static class to get the input/output names
			g.PassInputToNode(value1, node, Add<T>.In.A);
			g.PassInputToNode(value2, node, Add<T>.In.B);
			g.SetOutputFromNode(node, Add<T>.Out.Result, outputName);
		});

		Assert.IsTrue(Blueprint.TryLoad("TestIncompatibleMath", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		LogAssert.Expect(LogType.Error, $"[UBF][DLL][{testCase.Node.Type}] Input types cannot be operated on");
		yield return task;
	}
}