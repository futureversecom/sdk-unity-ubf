// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Futureverse.UBF.Runtime.Utils;
using UnityEngine;

namespace Futureverse.UBF.Runtime.Builtin
{
    public class GetChildSceneNodes : ACustomExecNode
    {
        public GetChildSceneNodes(Context context) : base(context) { }

        protected override void ExecuteSync()
        {
            if (!TryRead<SceneNode>("SceneNode", out var node))
            {
                UbfLogger.LogError("[GetChildSceneNodes] Could not find input \"SceneNode\"");
                return;
            }
            
            if (!TryRead<bool>("Recursive", out var recursive))
            {
                UbfLogger.LogError("[GetChildSceneNodes] Could not find input \"Recursive\"");
                return;
            }

            if (recursive)
            {
                var children = new List<SceneNode>();
                GetChildrenRecursive(node, children);
                WriteOutput("Children", children);
            }
            else
            {
                WriteOutput("Children", node.Children);
            }
        }

        private void GetChildrenRecursive(SceneNode current, List<SceneNode> children)
        {
            foreach (var child in current.Children)
            {
                children.Add(child);
                GetChildrenRecursive(child, children);
            }
        }
    }
}