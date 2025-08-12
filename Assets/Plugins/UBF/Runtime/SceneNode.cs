using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneNode
{
    public GameObject TargetSceneObject;
    public List<SceneComponent> Components = new();
    public List<SceneNode> Children = new();
}

public class SceneComponent
{
    public SceneNode Node;
}

public class MeshRendererSceneComponent : SceneComponent
{
    public Renderer TargetMeshRenderer;
    public bool skinned;
}

public class RigSceneComponent : SceneComponent
{
    
}
