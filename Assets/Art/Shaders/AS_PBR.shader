// Made with Amplify Shader Editor v1.9.3.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Futureverse/PBR/Standard"
{
	Properties
	{
		[KeywordEnum(Texture,SolidColor,VertexColor)] _AlbedoInput("AlbedoInput", Float) = 0
		_DiffuseTex("DiffuseTex", 2D) = "white" {}
		_BaseTint("BaseTint", Color) = (1,1,1,1)
		_BaseColor("BaseColor", Color) = (0.8018868,0.8018868,0.8018868,1)
		[Toggle(_USEEMISSIONTEX_ON)] _UseEmissionTex("UseEmissionTex", Float) = 0
		_EmissionTex("EmissionTex", 2D) = "white" {}
		_EmissionBoost("EmissionBoost", Float) = 1
		[Toggle]_UseEmissiveTint("UseEmissiveTint", Float) = 0
		[HDR]_EmissiveTint("EmissiveTint", Color) = (1,1,1,1)
		[Toggle(_USENORMALMAP_ON)] _UseNormalMap("UseNormalMap", Float) = 0
		_NormalTex("NormalTex", 2D) = "bump" {}
		_NormalStrength("NormalStrength", Float) = 0
		[Toggle(_USEORM_ON)] _UseORM("UseORM", Float) = 0
		_ORMTex("ORMTex", 2D) = "white" {}
		_Occlusion("Occlusion", Range( 0 , 1)) = 0
		_Roughness("Roughness", Range( 0 , 1)) = 0.5
		_Metallic("Metallic", Range( 0 , 1)) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		ZWrite On
		ZTest LEqual
		Offset  0 , 0
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _USENORMALMAP_ON
		#pragma shader_feature_local _ALBEDOINPUT_TEXTURE _ALBEDOINPUT_SOLIDCOLOR _ALBEDOINPUT_VERTEXCOLOR
		#pragma shader_feature_local _USEEMISSIONTEX_ON
		#pragma shader_feature_local _USEORM_ON
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _NormalTex;
		uniform float4 _NormalTex_ST;
		uniform float _NormalStrength;
		uniform float4 _BaseTint;
		uniform sampler2D _DiffuseTex;
		uniform float4 _DiffuseTex_ST;
		uniform float4 _BaseColor;
		uniform float _EmissionBoost;
		uniform sampler2D _EmissionTex;
		uniform float4 _EmissionTex_ST;
		uniform float _UseEmissiveTint;
		uniform float4 _EmissiveTint;
		uniform sampler2D _ORMTex;
		uniform float4 _ORMTex_ST;
		uniform float _Metallic;
		uniform float _Roughness;
		uniform float _Occlusion;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 temp_cast_0 = (1).xxxx;
			float2 uv_NormalTex = i.uv_texcoord * _NormalTex_ST.xy + _NormalTex_ST.zw;
			#ifdef _USENORMALMAP_ON
				float4 staticSwitch30_g26 = tex2D( _NormalTex, uv_NormalTex );
			#else
				float4 staticSwitch30_g26 = temp_cast_0;
			#endif
			o.Normal = UnpackScaleNormal( staticSwitch30_g26, _NormalStrength );
			float2 uv_DiffuseTex = i.uv_texcoord * _DiffuseTex_ST.xy + _DiffuseTex_ST.zw;
			#if defined(_ALBEDOINPUT_TEXTURE)
				float4 staticSwitch1_g26 = tex2D( _DiffuseTex, uv_DiffuseTex );
			#elif defined(_ALBEDOINPUT_SOLIDCOLOR)
				float4 staticSwitch1_g26 = _BaseColor;
			#elif defined(_ALBEDOINPUT_VERTEXCOLOR)
				float4 staticSwitch1_g26 = i.vertexColor;
			#else
				float4 staticSwitch1_g26 = tex2D( _DiffuseTex, uv_DiffuseTex );
			#endif
			float4 break5_g26 = staticSwitch1_g26;
			float3 appendResult7_g26 = (float3(break5_g26.r , break5_g26.g , break5_g26.b));
			o.Albedo = ( _BaseTint * float4( appendResult7_g26 , 0.0 ) ).rgb;
			float2 uv_EmissionTex = i.uv_texcoord * _EmissionTex_ST.xy + _EmissionTex_ST.zw;
			#ifdef _USEEMISSIONTEX_ON
				float4 staticSwitch15_g26 = tex2D( _EmissionTex, uv_EmissionTex );
			#else
				float4 staticSwitch15_g26 = float4(0,0,0,1);
			#endif
			float4 temp_cast_5 = (0.0).xxxx;
			o.Emission = ( ( _EmissionBoost * staticSwitch15_g26 ) + (( _UseEmissiveTint )?( ( (staticSwitch15_g26).w * _EmissiveTint ) ):( temp_cast_5 )) ).xyz;
			float2 uv_ORMTex = i.uv_texcoord * _ORMTex_ST.xy + _ORMTex_ST.zw;
			#ifdef _USEORM_ON
				float4 staticSwitch38_g26 = tex2D( _ORMTex, uv_ORMTex );
			#else
				float4 staticSwitch38_g26 = float4(1,0.5,0,0);
			#endif
			float4 break40_g26 = staticSwitch38_g26;
			float Metallic59_g26 = break40_g26.z;
			o.Metallic = saturate( ( Metallic59_g26 + ( ( _Metallic * 2.0 ) - 1.0 ) ) );
			float Roughness58_g26 = break40_g26.y;
			o.Smoothness = ( 1.0 - saturate( ( Roughness58_g26 + ( ( _Roughness * 2.0 ) - 1.0 ) ) ) );
			float Occlusion47_g26 = break40_g26.x;
			o.Occlusion = saturate( ( Occlusion47_g26 + ( 1.0 - _Occlusion ) ) );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19302
Node;AmplifyShaderEditor.FunctionNode;27;-937,2.5;Inherit;False;ASF_MasterInputs;0;;26;3bfdaf0e9a0c6b348bfb29ea14ab3f9c;0;0;7;COLOR;0;FLOAT3;36;FLOAT4;28;FLOAT;71;FLOAT;64;FLOAT;46;FLOAT;27
Node;AmplifyShaderEditor.OneMinusNode;12;-216,96.5;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Futureverse/PBR/Standard;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;1;False;;3;False;;True;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;True;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;27;64
WireConnection;0;0;27;0
WireConnection;0;1;27;36
WireConnection;0;2;27;28
WireConnection;0;3;27;71
WireConnection;0;4;12;0
WireConnection;0;5;27;46
WireConnection;0;9;27;27
ASEEND*/
//CHKSM=31A89C9368CE734957E2D4A5EF0AFE917EBAB74C