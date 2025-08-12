// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System;
using System.Collections.Generic;
using Futureverse.UBF.Runtime.Utils;
using UnityEngine;

namespace Futureverse.UBF.Runtime.Builtin
{
	public class FindSceneNodes : ACustomNode
	{
		public FindSceneNodes(Context context) : base(context) { }

		protected override void ExecuteSync()
		{
			if (!TryRead<SceneNode>("Root", out var node))
			{
				UbfLogger.LogError("[FindSceneNodes] Could not find input \"Root\"");
				return;
			}

			if (!TryRead<string>("Filter", out var filter))
			{
				UbfLogger.LogError("[FindSceneNodes] Could not find input \"Filter\"");
				return;
			}

			var nodeArray = new List<SceneNode>();
			FindNodes(filter, node, foundNode => { nodeArray.Add(foundNode); });
			WriteOutput("Nodes", nodeArray);
		}

		private static void FindNodes(string filter, SceneNode root, Action<SceneNode> action)
		{
			/*
			foreach (Transform child in root)
			{
				// TODO: support more comprehensive filtering (perhaps a glob pattern?)
				if (child.name.StartsWith(filter))
				{
					action(child);
				}

				FindNodes(filter, child, action);
			}*/

			foreach (var child in root.Children)
			{
				if (child.TargetSceneObject.name.StartsWith(filter))
				{
					action(child);
				}
				
				FindNodes(filter, child, action);
			}
		}
	}
}