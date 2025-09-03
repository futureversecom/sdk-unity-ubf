// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System.Collections.Generic;
using Futureverse.UBF.Runtime;
using Futureverse.UBF.Runtime.Execution;
using Futureverse.UBF.Runtime.Resources;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnitTests.PlayModeTests.Utils
{
	public class TestEnv
	{
		private readonly Catalog _catalog;
		private readonly Registry _registry;
		private readonly Dictionary<string, object> _blueprintInputs = new();
		private readonly string _blueprintJson;
		public readonly GameObject RootObject;
		public delegate void BuilderFunc(ref TestEnv builder);

		public static BlueprintExecutionTask Create(string blueprintJson, BuilderFunc build = null)
		{
			var testEnv = new TestEnv(blueprintJson);
			build?.Invoke(ref testEnv);
			return testEnv.Build();
		}

		private TestEnv(string blueprintJson)
		{
			_blueprintJson = blueprintJson;
			_registry = Registry.DefaultRegistry;
			_catalog = new Catalog();
			RootObject = new GameObject
			{
				name = "Root",
			};
		}

		public void AddTestResource(string resourceId, string resourceName)
		{
			var resourcePath =  $"{Application.dataPath}/Tests/PlayModeTests/Resources/{resourceName}";
			_catalog.AddResource(new ResourceData(resourceId, resourcePath));
		}

		public GameObject AddChildGameObject(GameObject parent, string gameObjectName = "NewObject")
		{
			return new GameObject
			{
				name = gameObjectName,
				transform =
				{
					parent = parent.transform,
				},
			};
		}

		public void AddToRegistry<T>() where T : ACustomNode
		{
			_registry.Register<T>();
		}

		public GameObject SpawnPrefab(GameObject parent, string prefabName)
		{
			var gameObject = Resources.Load<GameObject>(prefabName);
			var obj = Object.Instantiate(gameObject, parent.transform);
			obj.name = obj.name.Replace("(Clone)", "");
			return gameObject;
		}

		public void AddBlueprintInput(string key, object input)
		{
			_blueprintInputs.Add(key, input);
		}

		public SceneNode SceneNode(Transform t)
		{
			return new SceneNode
			{
				TargetSceneObject = t.gameObject,
				Name = t.gameObject.name
			};
		}

		private BlueprintExecutionTask Build()
		{
			Assert.IsTrue(Blueprint.TryLoad("TestSpawnMesh", _blueprintJson, out var blueprint, _registry));
			ArtifactProvider.Instance.RegisterCatalog(_catalog);
			foreach (var input in _blueprintInputs)
			{
				blueprint.RegisterVariable(input.Key, input.Value);
			}
			return new BlueprintExecutionTask(
				blueprint,
				new ExecutionConfig(RootObject?.transform, null)
			);
		}
	}
}