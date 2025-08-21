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

public class TestStringNodes
{
	[TearDown]
	public void TearDown()
	{
		var go = GameObject.Find("Root");
		if (go != null)
			Object.Destroy(go);
	}
	
	[UnityTest]
	public IEnumerator TestAppend()
	{
		const string string1 = "Hello, ";
		const string string2 = "World!";
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<string>("String1", "string", string1)
			.AddInputWithNode<string>("String2", "string", string2)
			.AddOutputWithNode("Result", "string")
			.AddNode(TestGraphBuilder.Append("Append"))
			.ConnectEntry("Set_Result")
			.PassInputToNode("String1", "Append", "A")
			.PassInputToNode("String2", "Append", "B")
			.SetOutputFromNode("Append", "Result", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestAppend", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryInterpretAs(out string appendedString));
		Assert.AreEqual(appendedString, $"{string1}{string2}");
	}
	
	[UnityTest]
	public IEnumerator TestContains()
	{
		const string testString = "Hello, World!";
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<string>("TestString", "string", testString)
			.AddOutputWithNode("Result1", "boolean")
			.AddOutputWithNode("Result2", "boolean")
			.AddNode(TestGraphBuilder.Contains("Contains1", Substring:"world", IgnoreCase:true))
			.AddNode(TestGraphBuilder.Contains("Contains2", Substring:"world", IgnoreCase:false))
			.ConnectEntry("Set_Result1")
			.ConnectExecution("Set_Result1", "Set_Result2")
			.PassInputToNode("TestString", "Contains1", "String")
			.PassInputToNode("TestString", "Contains2", "String")
			.SetOutputFromNode("Contains1", "Contains", "Result1")
			.SetOutputFromNode("Contains2", "Contains", "Result2")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestContains", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result1", out var result1));
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result2", out var result2));
		Assert.IsTrue(result1.TryInterpretAs(out bool contains1));
		Assert.IsTrue(result2.TryInterpretAs(out bool contains2));
		Assert.IsTrue(contains1);
		Assert.IsFalse(contains2);
	}
	
	[UnityTest]
	public IEnumerator TestFormatString()
	{
		const string formatString = "{1}, {2}!";
		const string string1 = "Hello";
		const string string2 = "World";
		var expectedString = formatString.Replace("{1}", string1).Replace("{2}", string2);
		
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<string>("fString", "string", formatString)
			.AddInputWithNode<string>("String1", "string", string1)
			.AddInputWithNode<string>("String2", "string", string2)
			.AddOutputWithNode("Result", "string")
			.AddNode(TestGraphBuilder.FormatString("FormatString", Item:TestUtils.DefaultList<string>(2)))
			.ConnectEntry("Set_Result")
			.PassInputToNode("fString", "FormatString", "FormatString")
			.PassInputToNode("String1", "FormatString", "Item.1")
			.PassInputToNode("String2", "FormatString", "Item.2")
			.SetOutputFromNode("FormatString", "Result", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestFormatString", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryInterpretAs(out string formattedString));
		Assert.AreEqual(formattedString, expectedString);
	}
	
	[UnityTest]
	public IEnumerator TestFormatStringBadFormat()
	{
		const string formatString = "{1}, {2}!";
		const string string1 = "Hello";
		var expectedString = formatString.Replace("{1}", string1).Replace("{2}", "<missing>");
		
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<string>("fString", "string", formatString)
			.AddInputWithNode<string>("String1", "string", string1)
			.AddOutputWithNode("Result", "string")
			.AddNode(TestGraphBuilder.FormatString("FormatString", Item:TestUtils.DefaultList<string>(2)))
			.ConnectEntry("Set_Result")
			.PassInputToNode("fString", "FormatString", "FormatString")
			.PassInputToNode("String1", "FormatString", "Item.1")
			.SetOutputFromNode("FormatString", "Result", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestFormatStringBadFormat", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryInterpretAs(out string formattedString));
		Assert.AreEqual(formattedString, expectedString);
	}
	
	[UnityTest]
	public IEnumerator TestReplace()
	{
		const string testString = "Hello, World!";
		var expectedString = testString.Replace("Hello", "Goodbye");
		
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<string>("testString", "string", testString)
			.AddOutputWithNode("Result", "string")
			.AddNode(TestGraphBuilder.Replace("Replace1", ToReplace:"Hello", Replacement:"Goodbye"))
			.AddNode(TestGraphBuilder.Replace("Replace2", ToReplace:"Foo", Replacement:"Bar"))
			.ConnectEntry("Set_Result")
			.PassInputToNode("testString", "Replace1", "String")
			.ConnectNodes("Replace1", "Result", "Replace2", "String")
			.SetOutputFromNode("Replace2", "Result", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestReplace", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryInterpretAs(out string formattedString));
		Assert.AreEqual(formattedString, expectedString);
	}	
	
	[UnityTest]
	public IEnumerator TestSplit()
	{
		const string testString = "One,Two,Three";
		var items = testString.Split(",");
		
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<string>("testString", "string", testString)
			.AddOutputWithNode("Result", "Array<string>")
			.AddNode(TestGraphBuilder.Split("Split", Separator:","))
			.ConnectEntry("Set_Result")
			.PassInputToNode("testString", "Split", "String")
			.SetOutputFromNode("Split", "Parts", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestSplit", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryReadArray<string>(out var resultArray));
		CollectionAssert.AreEquivalent(resultArray, items);
	}
	
	[UnityTest]
	public IEnumerator TestSplitNoSeparator()
	{
		const string testString = "One,Two,Three";
		var items = testString.Split(".");
		
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<string>("testString", "string", testString)
			.AddOutputWithNode("Result", "Array<string>")
			.AddNode(TestGraphBuilder.Split("Split", Separator:"."))
			.ConnectEntry("Set_Result")
			.PassInputToNode("testString", "Split", "String")
			.SetOutputFromNode("Split", "Parts", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestSplit", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryReadArray<string>(out var resultArray));
		CollectionAssert.AreEquivalent(resultArray, items);
	}	
	
	[UnityTest]
	public IEnumerator TestToLower()
	{
		const string testString = "HELLO, WORLD!";
		var expectedString = testString.ToLower();
		
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<string>("testString", "string", testString)
			.AddOutputWithNode("Result", "string")
			.AddNode(TestGraphBuilder.ToLower("ToLower"))
			.ConnectEntry("Set_Result")
			.PassInputToNode("testString", "ToLower", "String")
			.SetOutputFromNode("ToLower", "Result", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestToLower", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryInterpretAs(out string resultString));
		Assert.AreEqual(resultString, expectedString);
	}	
	
	[UnityTest]
	public IEnumerator TestToUpper()
	{
		const string testString = "Hello, World!";
		var expectedString = testString.ToUpper();
		
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<string>("testString", "string", testString)
			.AddOutputWithNode("Result", "string")
			.AddNode(TestGraphBuilder.ToUpper("ToUpper"))
			.ConnectEntry("Set_Result")
			.PassInputToNode("testString", "ToUpper", "String")
			.SetOutputFromNode("ToUpper", "Result", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestToUpper", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryInterpretAs(out string resultString));
		Assert.AreEqual(resultString, expectedString);
	}

	public struct ToStringTestCase<T>
	{
		public delegate string StringifierDelegate(T value);
		
		public string Type;
		public T Value;
		public string ExpectedValue;
	}

	private static IEnumerable ToStringTestCases()
	{
		yield return new ToStringTestCase<int> { Type = "int", Value = 3, ExpectedValue = "3" };
		yield return new ToStringTestCase<float> { Type = "float", Value = 3.3f,ExpectedValue = "3.3" };
		yield return new ToStringTestCase<bool> { Type = "boolean", Value = false, ExpectedValue = "false"};
		yield return new ToStringTestCase<List<int>> { Type = "Array<int>", Value = new List<int> { 1, 2, 3 }, ExpectedValue = "[\"1\",\"2\",\"3\"]"};
		yield return new ToStringTestCase<Material> { Type = "Material", Value = null, ExpectedValue = "null" };
	}
	
	[UnityTest]
	public IEnumerator TestToString<T>([ValueSource(nameof(ToStringTestCases))] ToStringTestCase<T> testCase)
	{
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<int>("value", testCase.Type, testCase.Value)
			.AddOutputWithNode("Result", "string")
			.AddNode(TestGraphBuilder.ToString<T>("ToString", testCase.Type))
			.ConnectEntry("Set_Result")
			.PassInputToNode("value", "ToString", "Value")
			.SetOutputFromNode("ToString", "String", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestToString", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryInterpretAs(out string resultString));
		Assert.AreEqual(resultString, testCase.ExpectedValue);
	}
	
	[UnityTest]
	public IEnumerator TestToStringForeign()
	{
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddOutputWithNode("Result", "string")
			.AddNode(TestGraphBuilder.CreateSceneNode("CreateSceneNode", "Parent"))
			.AddNode(TestGraphBuilder.ToString<Transform>("ToString", "SceneNode"))
			.ConnectEntry("CreateSceneNode")
			.ConnectExecution("CreateSceneNode", "Set_Result")
			.ConnectNodes("CreateSceneNode", "SceneNode", "ToString", "Value")
			.ConnectNodes("ToString", "String", "Set_Result", "Value")
			.SetOutputFromNode("ToString", "String", "Result")
			.Build();

		Assert.IsTrue(Blueprint.TryLoad("TestToString", graph, out var blueprint));
		var task = new BlueprintExecutionTask(blueprint, new ExecutionConfig(null, null));
		yield return task;
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Result", out var result));
		Assert.IsTrue(result.TryInterpretAs(out string resultString));
		NUnit.Framework.Assert.That(resultString, Does.Match("<Foreign \\d+>"));
	}	
}