// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System.Collections.Generic;
using Futureverse.UBF.Runtime;
using Futureverse.UBF.Runtime.Execution;
using Futureverse.UBF.Runtime.Resources;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnitTests.PlayModeTests.Utils
{
	public class TestEnvBuilder
	{
		private readonly Catalog _catalog;
		private readonly Registry _registry;
		private readonly Dictionary<string, object> _blueprintInputs = new();
		private readonly Dictionary<string, GameObject> _sceneObjects = new();
		private readonly string _blueprintJson;
		private readonly GameObject _rootObject;

		public TestEnvBuilder(string blueprintJson)
		{
			_blueprintJson = blueprintJson;
			_registry = Registry.DefaultRegistry;
			_catalog = new Catalog();
			_rootObject = new GameObject
			{
				name = "Root",
			};
			_sceneObjects.Add("Root", _rootObject);
		}

		public TestEnvBuilder AddTestResource(string resourceId, string resourceName)
		{
			var resourcePath =  $"{Application.dataPath}/Tests/PlayModeTests/Resources/{resourceName}";
			_catalog.AddResource(new ResourceData(resourceId, resourcePath));
			return this;
		}

		public TestEnvBuilder AddChildGameObjectToRoot(string gameObjectName = "NewObject")
		{
			var obj = new GameObject
			{
				name = gameObjectName,
				transform =
				{
					parent = _rootObject.transform,
				},
			};
			_sceneObjects.Add(gameObjectName, obj);
			return this;
		}

		public TestEnvBuilder SetChildObjectPosition(string gameObjectName, Vector3 position)
		{
			if (_sceneObjects.TryGetValue(gameObjectName, out var obj))
			{
				obj.transform.position = position;
			}
			return this;
		}
		
		public TestEnvBuilder SetChildObjectRotation(string gameObjectName, Vector3 rotation)
		{
			if (_sceneObjects.TryGetValue(gameObjectName, out var obj))
			{
				obj.transform.rotation = Quaternion.Euler(rotation);
			}
			return this;
		}

		public TestEnvBuilder RegisterAdditionalNode<T>() where T : ACustomNode
		{
			_registry.Register<T>();
			return this;
		}

		public TestEnvBuilder SpawnPrefabUnderRoot(string prefabName)
		{
			var gameObject = Resources.Load(prefabName);
			var obj = Object.Instantiate(gameObject, _rootObject.transform);
			obj.name = obj.name.Replace("(Clone)", "");
			return this;
		}

		public TestEnvBuilder AddBlueprintInput(string key, object input)
		{
			_blueprintInputs.Add(key, input);
			return this;
		}

		public TestEnvBuilder AddSceneComponent<T>(string gameObjectName) where T: Component
		{
			if (_sceneObjects.TryGetValue(gameObjectName, out var obj))
			{
				obj.AddComponent<T>();
			}
			return this;
		}

		public BlueprintExecutionTask Build()
		{
			Assert.IsTrue(Blueprint.TryLoad("TestSpawnMesh", _blueprintJson, out var blueprint, _registry));
			ArtifactProvider.Instance.RegisterCatalog(_catalog);
			foreach (var input in _blueprintInputs)
			{
				blueprint.RegisterVariable(input.Key, input.Value);
			}
			return new BlueprintExecutionTask(
				blueprint,
				new ExecutionConfig(_rootObject?.transform, null)
			);
		}
	}
}