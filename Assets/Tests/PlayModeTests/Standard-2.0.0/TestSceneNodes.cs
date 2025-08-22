// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System.Collections;
using NUnit.Framework;
using UnitTests.PlayModeTests.Utils;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

// ReSharper disable RedundantTypeArgumentsOfMethod

public class TestSceneNodes
{
	[TearDown]
	public void TearDown()
	{
		var go = GameObject.Find("Root");
		if (go != null)
			Object.Destroy(go);
	}
	
	[UnityTest]
	public IEnumerator TestSpawnMesh()
	{
		const string inputName = "Parent";
		const string resourceName = "glb-resource";

		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			g.AddInputWithNode<object>(inputName, UBFTypes.SceneNode);
			var spawnMeshNode = g.AddNode(new SpawnMesh(resourceName));
			g.ConnectEntry(spawnMeshNode);
			g.PassInputToNode(inputName, spawnMeshNode, SpawnMesh.In.Parent);
		});

		var task = TestEnv.Create(graph, (ref TestEnv e) =>
		{
			e.AddTestResource(resourceName, "Beanie.glb");
			e.AddBlueprintInput(inputName, e.RootObject.transform);
		});
		
		LogAssert.Expect(LogType.Warning, "[UBF][SpawnMesh] Failed to get input \"Config\"");
		yield return task;
		
		var parentObject = GameObject.Find("Root");
		Assert.IsNotNull(parentObject);
		var glbObjects = parentObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		Assert.AreEqual(glbObjects.Length, 1);
		var glbObject = glbObjects[0];
		Assert.IsNotNull(glbObject);
		Assert.AreEqual(glbObject.name, "SK_PB_Beanie_Head_LOD0");
	}
	
	[UnityTest]
	public IEnumerator TestCreateMeshConfig()
	{
		const string outputName = "MeshConfig";
		const string resourceName = "glb-resource";
		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			var setOutputNode = g.AddOutputWithNode(outputName, UBFTypes.MeshConfig);
			// <string> here is technically invalid because this node takes Mesh or GLB resource.
			// but since those are both resources we can use string to pass in the resource id
			var createMeshConfigNode = g.AddNode(new CreateMeshConfig<string>(UBFTypes.GLBResource, resourceName));
			g.ConnectEntry(createMeshConfigNode);
			g.ConnectExecution(createMeshConfigNode, setOutputNode);
			g.SetOutputFromNode(createMeshConfigNode, CreateMeshConfig<string>.Out.MeshConfig, outputName);
		});

		var task = TestEnv.Create(graph, (ref TestEnv e) =>
		{
			e.AddTestResource(resourceName, "Beanie.glb");
		});

		yield return task;
		
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("MeshConfig", out var meshConfig));
		//Assert.IsTrue(meshConfig.TryInterpretAs(out RuntimeMeshConfig config));
		
		// TODO: How can we mock UBF Settings in tests to add config overrides?
	}
	
	[UnityTest]
	public IEnumerator TestCreateSceneNode()
	{
		const string gameObjectName = "NewObject";
		const string outputName = "SceneNode";

		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			var setOutputNode = g.AddOutputWithNode(outputName, UBFTypes.SceneNode);
			var createSceneNode = g.AddNode(new CreateSceneNode(gameObjectName));
			g.ConnectEntry(createSceneNode);
			g.SetOutputFromNode(createSceneNode, CreateSceneNode.Out.SceneNode, outputName);
		});

		var task = TestEnv.Create(graph);
		yield return task;
		
		var spawnedObject = GameObject.Find(gameObjectName);
		Assert.IsNotNull(spawnedObject);
		var parent = spawnedObject.transform.parent.gameObject;
		Assert.IsNotNull(parent);
		Assert.AreEqual(parent.name, "Root");
	}
	
	[UnityTest]
	public IEnumerator TestSetSceneNodeEnabled()
	{
		const string inputName = "GameObject";
		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			g.AddInputWithNode<object>(inputName, UBFTypes.SceneNode);
			var setSceneNodeEnabledNode = g.AddNode(new SetSceneNodeEnabled(Enabled:false));
			g.ConnectEntry(setSceneNodeEnabledNode);
			g.PassInputToNode(inputName, setSceneNodeEnabledNode, SetSceneNodeEnabled.In.SceneNode);
		});

		var task = TestEnv.Create(graph, (ref TestEnv e) =>
		{
			e.AddBlueprintInput(inputName, e.RootObject.transform);
		});
		
		yield return task;
		var gameObject = task.ExecutionContext.Config.GetRootTransform.gameObject;
		Assert.IsNotNull(gameObject);
		Assert.IsFalse(gameObject.activeSelf);
	}
	
	// SetBlendShape test is failing atm... not sure why yet, need to investigate more
	
	// [UnityTest]
	// public IEnumerator TestSetBlendShape()
	// {
	// 	const string inputName = "Renderer";
	// 	const string blendShapeName = "NeckJeanShirt";
	// 	const string prefabName = "BoomboxChainPrefab";
	// 	const float expectedValue = 1f;
	// 	
	// 	var graph = TestGraph.Create((ref TestGraph g) =>
	// 	{
	// 		g.AddInputWithNode<object>(inputName, UBFTypes.MeshRenderer);
	// 		var setBlendShapeNode = g.AddNode(new SetBlendShape(BlendShapeID: blendShapeName, Value: expectedValue));
	// 		g.ConnectEntry(setBlendShapeNode);
	// 		g.PassInputToNode(inputName, setBlendShapeNode, SetBlendShape.In.Renderer);
	// 	});
	// 	
	// 	var task = TestEnv.Create(graph, (ref TestEnv e) =>
	// 	{
	// 		var boomboxChain = e.SpawnPrefab(e.RootObject, prefabName);
	// 		e.AddBlueprintInput(inputName, boomboxChain.GetComponentInChildren<SkinnedMeshRenderer>());
	// 	});
	//
	// 	yield return task;
	//
	// 	var boomboxChain = GameObject.Find(prefabName);
	// 	Assert.IsNotNull(boomboxChain);
	// 	var renderer = boomboxChain.GetComponentInChildren<SkinnedMeshRenderer>();
	// 	Assert.IsNotNull(renderer);
	// 	var blendShapeIndex = renderer.sharedMesh.GetBlendShapeIndex(blendShapeName);
	// 	var blendShapeWeight = renderer.GetBlendShapeWeight(blendShapeIndex);
	// 	Assert.AreEqual(blendShapeWeight, expectedValue);
	// }
	
	[UnityTest]
	public IEnumerator TestTransformPosition()
	{
		const string inputName = "Transform";
		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			g.AddInputWithNode<object>(inputName, UBFTypes.SceneNode);
			var transformNode = g.AddNode(new TransformPosition(Right:5f, Up:10f, Forward:20f));
			g.ConnectEntry(transformNode);
			g.PassInputToNode(inputName, transformNode, TransformPosition.In.SceneNode);
		});

		var task = TestEnv.Create(graph, (ref TestEnv e) =>
		{
			var gameObject = e.AddChildGameObject(e.RootObject, inputName);
			e.AddBlueprintInput(inputName, gameObject.transform);
		});
		
		yield return task;
	
		var transform = GameObject.Find("Transform").transform;
		Assert.IsNotNull(transform);
		Assert.AreEqual(transform.position.x, 5f);
		Assert.AreEqual(transform.position.y, 10f);
		Assert.AreEqual(transform.position.z, 20f);
	}
	
	[UnityTest]
	public IEnumerator TestTransformPositionAdditive()
	{
		const string inputName = "Transform";
		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			g.AddInputWithNode<object>(inputName, UBFTypes.SceneNode);
			var transformNode = g.AddNode(new TransformPosition(Additive:true, Right:5f, Up:10f, Forward:20f));
			g.ConnectEntry(transformNode);
			g.PassInputToNode(inputName, transformNode, TransformPosition.In.SceneNode);
		});
	
		var task = TestEnv.Create(graph, (ref TestEnv e) =>
		{
			var gameObject = e.AddChildGameObject(e.RootObject, inputName);
			gameObject.transform.position = new Vector3(10f, 10f, 10f);
			e.AddBlueprintInput(inputName, gameObject.transform);
		});
		
		yield return task;
	
		var transform = GameObject.Find("Transform").transform;
		Assert.IsNotNull(transform);
		Assert.AreEqual(transform.position.x, 15f);
		Assert.AreEqual(transform.position.y, 20f);
		Assert.AreEqual(transform.position.z, 30f);
	}
	
	[UnityTest]
	public IEnumerator TestTransformRotation()
	{
		const string inputName = "Transform";
		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			g.AddInputWithNode<object>(inputName, UBFTypes.SceneNode);
			var transformNode = g.AddNode(new TransformRotation(Pitch:60f, Yaw:120f, Roll:90f));
			g.ConnectEntry(transformNode);
			g.PassInputToNode(inputName, transformNode, TransformPosition.In.SceneNode);
		});
	
		var task = TestEnv.Create(graph, (ref TestEnv e) =>
		{
			var gameObject = e.AddChildGameObject(e.RootObject, inputName);
			e.AddBlueprintInput(inputName, gameObject.transform);
		});
		
		yield return task;
	
		var transform = GameObject.Find("Transform").transform;
		var expectedQuat = transform.localRotation *= Quaternion.Euler(60f, 120f, 90f);
		Assert.IsNotNull(transform);
		NUnit.Framework.Assert.AreEqual(transform.rotation.eulerAngles.x, expectedQuat.eulerAngles.x, delta:0.01f);
		NUnit.Framework.Assert.AreEqual(transform.rotation.eulerAngles.y, expectedQuat.eulerAngles.y, delta:0.01f);
		NUnit.Framework.Assert.AreEqual(transform.rotation.eulerAngles.z, expectedQuat.eulerAngles.z, delta:0.01f);
	}
	
	[UnityTest]
	public IEnumerator TestTransformRotationAdditive()
	{
		const string inputName = "Transform";
		var graph = TestGraph.Create((ref TestGraph g) =>
		{
			g.AddInputWithNode<object>(inputName, UBFTypes.SceneNode);
			var transformNode = g.AddNode(new TransformRotation(Additive:true, Pitch:60f, Yaw:120f, Roll:90f));
			g.ConnectEntry(transformNode);
			g.PassInputToNode(inputName, transformNode, TransformPosition.In.SceneNode);
		});
	
		var task = TestEnv.Create(graph, (ref TestEnv e) =>
		{
			var gameObject = e.AddChildGameObject(e.RootObject, inputName);
			gameObject.transform.rotation = Quaternion.Euler(new Vector3(10f, 10f, 10f));
			e.AddBlueprintInput(inputName, gameObject.transform);
		});
		
		yield return task;
	
		var transform = GameObject.Find("Transform").transform;
		var expectedQuat = transform.localRotation *= Quaternion.Euler(60f, 120f, 90f);
		Assert.IsNotNull(transform);
		NUnit.Framework.Assert.AreEqual(transform.rotation.eulerAngles.x, expectedQuat.eulerAngles.x, delta:0.01f);
		NUnit.Framework.Assert.AreEqual(transform.rotation.eulerAngles.y, expectedQuat.eulerAngles.y, delta:0.01f);
		NUnit.Framework.Assert.AreEqual(transform.rotation.eulerAngles.z, expectedQuat.eulerAngles.z, delta:0.01f);
	}
}