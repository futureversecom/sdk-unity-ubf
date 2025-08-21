// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using Futureverse.UBF.Runtime.Utils;
using UnityEngine;

namespace Futureverse.UBF.Runtime.Builtin
{
	public class SetBlendShape : ACustomExecNode
	{
		public SetBlendShape(Context context) : base(context) { }

		protected override void ExecuteSync()
		{
			if (!TryRead<Renderer>("Renderer", out var targetRenderer) || targetRenderer == null)
			{
				UbfLogger.LogError("[SetBlendShape] Could not find input \"Renderer\"");
				return;
			}
			
			if (!TryRead<string>("BlendShapeID", out var blendShapeId))
			{
				UbfLogger.LogError("[SetBlendShape] Could not find input \"BlendShapeID\"");
				return;
			}
			
			if (!TryRead<float>("Value", out var blendShapeValue))
			{
				UbfLogger.LogError("[SetBlendShape] Could not find input \"Value\"");
				return;
			}

			var smr = targetRenderer as SkinnedMeshRenderer;
			if (smr == null)
			{
				UbfLogger.LogError("[SetBlendShape] No Skinned Mesh Renderer on target transform");
				return;
			}

			var index = smr.sharedMesh.GetBlendShapeIndex(blendShapeId);
			if (index == -1)
			{
				UbfLogger.LogWarn($"[SetBlendShape] Invalid blend shape ID: {blendShapeId}");
				return;
			}

			smr.SetBlendShapeWeight(index, blendShapeValue);
		}
	}
}