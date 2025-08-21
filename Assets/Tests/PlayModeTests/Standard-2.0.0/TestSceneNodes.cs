// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System.Collections;
using NUnit.Framework;
using UnitTests.PlayModeTests.Utils;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

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
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<object>("Parent", "SceneNode")
			.AddNode(TestGraphBuilder.SpawnMesh("SpawnMesh", "glb-resource"))
			.ConnectEntry("SpawnMesh")
			.PassInputToNode("Parent", "SpawnMesh", "Parent")
			.Build();

		var task = new TestEnvBuilder(graph)
			.AddTestResource("glb-resource", "Beanie.glb")
			.AddBlueprintInput("Parent", GameObject.Find("Root").transform)
			.Build();
		
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
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddOutputWithNode("MeshConfig", "MeshConfig")
			// String here is technically invalid because this node takes Mesh or GLB resource.
			// but since those are both resources we can use string to pass in the resource id
			.AddNode(TestGraphBuilder.CreateMeshConfig<string>("CreateMeshConfig", "Resource<GLB>", "glb-resource"))
			.ConnectEntry("CreateMeshConfig")
			.ConnectExecution("CreateMeshConfig", "Set_MeshConfig")
			.SetOutputFromNode("CreateMeshConfig", "MeshConfig", "MeshConfig")
			.Build();

		var task = new TestEnvBuilder(graph)
			.AddTestResource("glb-resource", "Beanie.glb")
			.Build();
		yield return task;
		
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("MeshConfig", out var meshConfig));
		//Assert.IsTrue(meshConfig.TryInterpretAs(out RuntimeMeshConfig config));
		
		// TODO: How can we mock UBF Settings in tests to add config overrides?
	}
	
	[UnityTest]
	public IEnumerator TestCreateSceneNode()
	{
		const string gameObjectName = "NewObject";
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddOutputWithNode("SceneNode", "SceneNode")
			.AddNode(TestGraphBuilder.CreateSceneNode("CreateSceneNode", gameObjectName))
			.ConnectEntry("CreateSceneNode")
			.SetOutputFromNode("CreateSceneNode", "Node", "SceneNode")
			.Build();

		var task = new TestEnvBuilder(graph)
			.Build();
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
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<object>("GameObject", "SceneNode")
			.AddNode(TestGraphBuilder.SetSceneNodeEnabled("SetSceneNodeEnabled", Enabled:false))
			.ConnectEntry("SetSceneNodeEnabled")
			.PassInputToNode("GameObject", "SetSceneNodeEnabled", "SceneNode")
			.Build();

		var task = new TestEnvBuilder(graph)
			.AddBlueprintInput("GameObject", GameObject.Find("Root").transform)
			.Build();
		
		yield return task;
		var gameObject = task.ExecutionContext.Config.GetRootTransform.gameObject;
		Assert.IsNotNull(gameObject);
		Assert.IsFalse(gameObject.activeSelf);
	}

	[UnityTest]
	public IEnumerator TestSetBlendShape()
	{
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<object>("Renderer", "MeshRenderer")
			.AddNode(TestGraphBuilder.SetBlendShape("SetBlendShape", BlendShapeID:"NeckJeanShirt", Value:1f))
			.ConnectEntry("SetBlendShape")
			.PassInputToNode("Renderer", "SetBlendShape", "Renderer")
			.Build();

		var task = new TestEnvBuilder(graph)
			.SpawnPrefabUnderRoot("BoomboxChain")
			.AddBlueprintInput(
				"Renderer",
				GameObject.Find("BoomboxChain")
					.GetComponentInChildren<SkinnedMeshRenderer>()
			)
			.Build();

		yield return task;

		var boomboxChain = GameObject.Find("BoomboxChain");
		Assert.IsNotNull(boomboxChain);
		var renderer = boomboxChain.GetComponentInChildren<SkinnedMeshRenderer>();
		Assert.IsNotNull(renderer);
		var blendShapeWeight = renderer.GetBlendShapeWeight(renderer.sharedMesh.GetBlendShapeIndex("NeckJeanShirt"));
		Assert.AreEqual(blendShapeWeight, 1f);
	}
	
	[UnityTest]
	public IEnumerator TestTransformPosition()
	{
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<object>("Transform", "SceneNode")
			.AddNode(TestGraphBuilder.TransformPosition("TransformPosition", Right:5F, Up:10f, Forward:20f))
			.ConnectEntry("TransformPosition")
			.PassInputToNode("Transform", "TransformPosition", "SceneNode")
			.Build();

		var task = new TestEnvBuilder(graph)
			.AddChildGameObjectToRoot("Transform")
			.AddBlueprintInput("Transform", GameObject.Find("Transform").transform)
			.Build();
		
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
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<object>("Transform", "SceneNode")
			.AddNode(TestGraphBuilder.TransformPosition("TransformPosition", Additive:true, Right:5F, Up:10f, Forward:20f))
			.ConnectEntry("TransformPosition")
			.PassInputToNode("Transform", "TransformPosition", "SceneNode")
			.Build();

		var task = new TestEnvBuilder(graph)
			.AddChildGameObjectToRoot("Transform")
			.SetChildObjectPosition("Transform", new Vector3(10f, 10f, 10f))
			.AddBlueprintInput("Transform", GameObject.Find("Transform").transform)
			.Build();
		
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
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<object>("Transform", "SceneNode")
			.AddNode(TestGraphBuilder.TransformRotation("TransformRotation", Pitch:60f, Yaw:120f, Roll:90f))
			.ConnectEntry("TransformRotation")
			.PassInputToNode("Transform", "TransformRotation", "SceneNode")
			.Build();

		var task = new TestEnvBuilder(graph)
			.AddChildGameObjectToRoot("Transform")
			.AddBlueprintInput("Transform", GameObject.Find("Transform").transform)
			.Build();
		
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
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<object>("Transform", "SceneNode")
			.AddNode(TestGraphBuilder.TransformRotation("TransformRotation", Additive:true, Pitch:60f, Yaw:120f, Roll:90f))
			.ConnectEntry("TransformRotation")
			.PassInputToNode("Transform", "TransformRotation", "SceneNode")
			.Build();

		var task = new TestEnvBuilder(graph)
			.AddChildGameObjectToRoot("Transform")
			.SetChildObjectRotation("Transform", new Vector3(10f, 10f, 10f))
			.AddBlueprintInput("Transform", GameObject.Find("Transform").transform)
			.Build();
		
		yield return task;

		var transform = GameObject.Find("Transform").transform;
		var expectedQuat = transform.localRotation *= Quaternion.Euler(60f, 120f, 90f);
		Assert.IsNotNull(transform);
		NUnit.Framework.Assert.AreEqual(transform.rotation.eulerAngles.x, expectedQuat.eulerAngles.x, delta:0.01f);
		NUnit.Framework.Assert.AreEqual(transform.rotation.eulerAngles.y, expectedQuat.eulerAngles.y, delta:0.01f);
		NUnit.Framework.Assert.AreEqual(transform.rotation.eulerAngles.z, expectedQuat.eulerAngles.z, delta:0.01f);
	}
}