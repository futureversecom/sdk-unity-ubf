// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using Futureverse.UBF.Runtime.Utils;
using GLTFast;
using Plugins.UBF.Runtime.Utils;
using UnityEngine;

namespace Futureverse.UBF.Runtime.Builtin
{
	public class SpawnModel : ACustomExecNode
	{
		protected readonly List<MeshRendererSceneComponent> Renderers = new();
		protected readonly List<SceneNode> Transforms = new();
		protected readonly List<MeshRendererSceneComponent> SkinnedMeshRenderers = new();
		
		public SpawnModel(Context context) : base(context) { }

		protected override IEnumerator ExecuteAsync()
		{
			if (!TryReadResourceId("Resource", out var resourceId) || !resourceId.IsValid)
			{
				UbfLogger.LogError("[SpawnModel] Could not find input \"Resource\"");
				yield break;
			}

			if (!TryRead<SceneNode>("Parent", out var parent))
			{
				UbfLogger.LogError("[SpawnModel] Could not find input \"Parent\"");
				yield break;
			}

			if (!TryRead<RuntimeMeshConfig>("Config", out var runtimeConfig))
			{
				UbfLogger.LogWarn("[SpawnModel] Failed to get input \"Config\"");
			}

			GltfImport gltfResource = null;
			var routine = CoroutineHost.Instance.StartCoroutine(
				NodeContext.ExecutionContext.Config.GetMeshInstance(
					resourceId,
					(resource, _) =>
					{
						gltfResource = resource;
					}
				)
			);
			if (routine != null)
			{
				yield return routine;
			}

			if (gltfResource == null)
			{
				UbfLogger.LogError($"[SpawnModel] Could not load GLB resource with Id \"{resourceId.Value}\"");
				yield break;
			}
			
			var instantiator = new GameObjectInstantiator(gltfResource, parent.TargetSceneObject.transform);
			instantiator.MeshAdded += MeshAddedCallback;

			var instantiateRoutine = CoroutineHost.Instance.StartCoroutine(
				new WaitForTask(gltfResource.InstantiateMainSceneAsync(instantiator))
			);
			if (instantiateRoutine != null)
			{
				yield return instantiateRoutine;
			}
			
			var glbReference = parent.TargetSceneObject.AddComponent<GLBReference>();
			glbReference.GLTFImport = gltfResource;
			
			// Extra yield here as we can't be sure that the mesh will be instantiated fully after the above task finishes
			yield return null;
			
			ApplyRuntimeConfig(runtimeConfig);

			foreach (var node in Transforms)
			{
				parent.Children.Add(node);
			}
			WriteOutput("Renderers", Renderers);
			WriteOutput("Scene Nodes", Transforms);
		}
		
		protected virtual void MeshAddedCallback(
			GameObject gameObject,
			uint nodeIndex,
			string meshName,
			MeshResult meshResult,
			uint[] joints,
			uint? rootJoint,
			float[] morphTargetWeights,
			int meshNumeration)
		{
			var node = new SceneNode()
			{
				TargetSceneObject = gameObject
			};
			Transforms.Add(node);
			var renderer = gameObject.GetComponent<Renderer>();
			if (renderer == null)
			{
				return;
			}

			var renderComponent = new MeshRendererSceneComponent()
			{
				Node = node,
				TargetMeshRenderer = renderer,
				skinned = (renderer is SkinnedMeshRenderer)
			};
			Renderers.Add(renderComponent);
			if (renderComponent.skinned)
			{
				SkinnedMeshRenderers.Add(renderComponent);
			}
			node.Components.Add(renderComponent);
		}

		protected void ApplyRuntimeConfig(RuntimeMeshConfig runtimeConfig)
		{
			if (runtimeConfig == null || runtimeConfig.AnimationObject == null || SkinnedMeshRenderers.Count <= 0)
			{
				return;
			}

			foreach (var renderer in SkinnedMeshRenderers)
			{
				UbfLogger.LogInfo(
					$"[{GetType().Name}] Retargeting \"{renderer.TargetMeshRenderer.name}\" with spawned config \"{runtimeConfig.Config.name}\""
				);
				var runtimeSMR = runtimeConfig.AnimationObject.GetComponentInChildren<SkinnedMeshRenderer>();
				if (runtimeSMR != null)
				{
					RigUtils.RetargetRig(runtimeSMR, renderer.TargetMeshRenderer as SkinnedMeshRenderer); // Assume if it lives in SkinnedMeshRenderers that it fits the type
				}
				else
				{
					RigUtils.RetargetRig(runtimeConfig.AnimationObject.transform, renderer.TargetMeshRenderer as SkinnedMeshRenderer);
				}
			}
		}
	}
}