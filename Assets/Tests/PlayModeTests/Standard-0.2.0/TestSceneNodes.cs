// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System.Collections;
using System.Collections.Generic;
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
			.AddNode(TestGraphBuilder.CreateMeshConfig("CreateMeshConfig", "glb-resource"))
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
			.PassInputToNode("GameObject", "SetSceneNodeEnabled", "Node")
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
	public IEnumerator TestFindRenderer()
	{
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<object>("Renderers", "Array<MeshRenderer>")
			.AddOutputWithNode("Renderer", "MeshRenderer")
			.AddNode(TestGraphBuilder.FindRenderer("FindRenderer", Name:"Ren1"))
			.ConnectEntry("Set_Renderer")
			.PassInputToNode("Renderers", "FindRenderer", "Array")
			.SetOutputFromNode("FindRenderer", "Renderer", "Renderer")
			.Build();

		var task = new TestEnvBuilder(graph)
			.AddChildGameObjectToRoot("Ren1")
			.AddSceneComponent<SkinnedMeshRenderer>("Ren1")
			.AddChildGameObjectToRoot("Ren2")
			.AddSceneComponent<SkinnedMeshRenderer>("Ren2")
			.AddBlueprintInput("Renderers", new List<SkinnedMeshRenderer>
			{
				GameObject.Find("Ren1").GetComponent<SkinnedMeshRenderer>(),
				GameObject.Find("Ren2").GetComponent<SkinnedMeshRenderer>(),
			})
			.Build();
		
		yield return task;

		var ren1 = GameObject.Find("Ren1")
			.GetComponent<SkinnedMeshRenderer>();
		Assert.IsNotNull(ren1);
		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Renderer", out var result));
		Assert.IsTrue(result.TryInterpretAs(out SkinnedMeshRenderer renderer));
		Assert.AreEqual(ren1, renderer);
	}
	
	[UnityTest]
	public IEnumerator TestFindSceneNodes()
	{
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<object>("GameObject", "SceneNode")
			.AddOutputWithNode("Array", "Array<SceneNode>")
			.AddNode(TestGraphBuilder.FindSceneNodes("FindSceneNodes", Filter:"Hel"))
			.ConnectEntry("Set_Array")
			.PassInputToNode("GameObject", "FindSceneNodes", "Root")
			.SetOutputFromNode("FindSceneNodes", "Nodes", "Array")
			.Build();

		var task = new TestEnvBuilder(graph)
			.AddChildGameObjectToRoot("Hello")
			.AddChildGameObjectToRoot("Helix")
			.AddBlueprintInput("GameObject", GameObject.Find("Root").transform)
			.Build();
		
		yield return task;
		
		var helloGameObject = GameObject.Find("Hello");
		Assert.IsNotNull(helloGameObject);
		var helixGameObject = GameObject.Find("Helix");
		Assert.IsNotNull(helixGameObject);

		Assert.IsTrue(task.ExecutionContext.TryReadOutput("Array", out var result));
		Assert.IsTrue(result.TryReadArray(out List<Transform> transforms));
		CollectionAssert.Contains(transforms, helloGameObject.transform);
		CollectionAssert.Contains(transforms, helixGameObject.transform);
	}

	[UnityTest]
	public IEnumerator TestSetBlendShape()
	{
		var graph = new TestGraphBuilder(BlueprintVersion.Version)
			.AddInputWithNode<object>("Renderer", "MeshRenderer")
			.AddNode(TestGraphBuilder.SetBlendShape("SetBlendShape", ID:"NeckJeanShirt", Value:1f))
			.ConnectEntry("SetBlendShape")
			.PassInputToNode("Renderer", "SetBlendShape", "Target")
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
			.PassInputToNode("Transform", "TransformPosition", "Transform Object")
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
			.AddNode(TestGraphBuilder.TransformPosition("TransformPosition", IsAdditive:true, Right:5F, Up:10f, Forward:20f))
			.ConnectEntry("TransformPosition")
			.PassInputToNode("Transform", "TransformPosition", "Transform Object")
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
			.PassInputToNode("Transform", "TransformRotation", "Transform Object")
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
			.AddNode(TestGraphBuilder.TransformRotation("TransformRotation", IsAdditive:true, Pitch:60f, Yaw:120f, Roll:90f))
			.ConnectEntry("TransformRotation")
			.PassInputToNode("Transform", "TransformRotation", "Transform Object")
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