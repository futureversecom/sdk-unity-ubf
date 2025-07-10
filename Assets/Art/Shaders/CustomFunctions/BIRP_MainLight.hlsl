void MainLight_float(float3 WorldPos,out float3 Direction,  out float3 Color, out float DistanceAtten, out float ShadowAtten)
{
#if SHADERGRAPH_PREVIEW
  Direction = float3(0.5, 0.5, 0);
   Color = 1;
   DistanceAtten = 1;
   ShadowAtten = 1;
#else
#if SHADOWS_SCREEN
   float4 clipPos = TransformWorldToHClip(WorldPos);
   float4 shadowCoord = ComputeScreenPos(clipPos);
#else
   float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
//this function doesn't quite work
   Light mainLight = GetMainLight(shadowCoord);
   //Direction = mainLight.position;
   //Color = _MainLightColor.rgb;
   //Direction = mainLight.direction;
//get xyz on lightPos?
   Direction = _WorldSpaceLightPos0;
   //Color = mainLight.color;
   //haven't figured out the color yet
      Color = 1;
   DistanceAtten = unity_ProbesOcclusion.x;
   ShadowAtten = mainLight.shadowAttenuation;
#endif
}