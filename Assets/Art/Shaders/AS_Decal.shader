// Made with Amplify Shader Editor v1.9.3.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Futureverse/Decal/Standard"
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
		_DecalMask("DecalMask", 2D) = "white" {}
		_TintBase("TintBase", Color) = (1,1,1,1)
		_TintA("TintA", Color) = (1,0,0,1)
		_TintB("TintB", Color) = (0,1,0,1)
		_TintC("TintC", Color) = (0,0,1,1)
		_DarkenBase("DarkenBase", Range( 0 , 1)) = 1
		_DarkenA("DarkenA", Range( 0 , 1)) = 0
		_DarkenB("DarkenB", Range( 0 , 1)) = 1
		_DarkenC("DarkenC", Range( 0 , 1)) = 1
		_RoughBase("RoughBase", Range( 0 , 1)) = 0.5
		_RoughA("RoughA", Range( 0 , 1)) = 0.5
		_RoughB("RoughB", Range( 0 , 1)) = 0.5
		_RoughC("RoughC", Range( 0 , 1)) = 0.5
		_Flakes("Flakes", 2D) = "white" {}
		_FlakesSize("FlakesSize", Float) = 1
		_FlakesBase("FlakesBase", Range( 0 , 1)) = 1
		_FlakesA("FlakesA", Range( 0 , 1)) = 1
		_FlakesB("FlakesB", Range( 0 , 1)) = 1
		_FlakesC("FlakesC", Range( 0 , 1)) = 1
		_MetalBase("MetalBase", Range( 0 , 1)) = 0
		_MetalA("MetalA", Range( 0 , 1)) = 0
		_MetalB("MetalB", Range( 0 , 1)) = 0
		_MetalC("MetalC", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _USENORMALMAP_ON
		#pragma shader_feature_local _ALBEDOINPUT_TEXTURE _ALBEDOINPUT_SOLIDCOLOR _ALBEDOINPUT_VERTEXCOLOR
		#pragma shader_feature_local _USEEMISSIONTEX_ON
		#pragma shader_feature_local _USEORM_ON
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _NormalTex;
		uniform float4 _NormalTex_ST;
		uniform float _NormalStrength;
		uniform float4 _TintBase;
		uniform float _DarkenBase;
		uniform float4 _TintA;
		uniform float _DarkenA;
		uniform sampler2D _DecalMask;
		uniform float4 _DecalMask_ST;
		uniform float4 _TintB;
		uniform float _DarkenB;
		uniform float4 _TintC;
		uniform float _DarkenC;
		uniform float4 _BaseTint;
		uniform sampler2D _DiffuseTex;
		uniform float4 _DiffuseTex_ST;
		uniform float4 _BaseColor;
		uniform float _EmissionBoost;
		uniform sampler2D _EmissionTex;
		uniform float4 _EmissionTex_ST;
		uniform float _UseEmissiveTint;
		uniform float4 _EmissiveTint;
		uniform float _MetalBase;
		uniform float _MetalA;
		uniform float _MetalB;
		uniform float _MetalC;
		uniform sampler2D _ORMTex;
		uniform float4 _ORMTex_ST;
		uniform float _Metallic;
		uniform float _RoughBase;
		uniform sampler2D _Flakes;
		uniform float _FlakesSize;
		uniform float _FlakesBase;
		uniform float _RoughA;
		uniform float _FlakesA;
		uniform float _RoughB;
		uniform float _FlakesB;
		uniform float _RoughC;
		uniform float _FlakesC;
		uniform float _Roughness;
		uniform float _Occlusion;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 temp_cast_0 = (1).xxxx;
			float2 uv_NormalTex = i.uv_texcoord * _NormalTex_ST.xy + _NormalTex_ST.zw;
			#ifdef _USENORMALMAP_ON
				float4 staticSwitch30_g65 = tex2D( _NormalTex, uv_NormalTex );
			#else
				float4 staticSwitch30_g65 = temp_cast_0;
			#endif
			float3 I_Normal43 = UnpackScaleNormal( staticSwitch30_g65, _NormalStrength );
			o.Normal = I_Normal43;
			float3 hsvTorgb14_g64 = RGBToHSV( _TintBase.rgb );
			float3 hsvTorgb17_g64 = HSVToRGB( float3(hsvTorgb14_g64.x,( hsvTorgb14_g64.y + 1.0 ),hsvTorgb14_g64.z) );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float fresnelNdotV9_g64 = dot( ase_normWorldNormal, ase_worldViewDir );
			float fresnelNode9_g64 = ( 0.0 + 1.0 * pow( max( 1.0 - fresnelNdotV9_g64 , 0.0001 ), 2.5 ) );
			float4 temp_cast_4 = (fresnelNode9_g64).xxxx;
			float Fresnel12_g64 = saturate( CalculateContrast(1.75,temp_cast_4) ).r;
			float4 lerpResult4_g64 = lerp( _TintBase , CalculateContrast(0.15,float4( hsvTorgb17_g64 , 0.0 )) , ( Fresnel12_g64 * _DarkenBase ));
			float4 ColorBase124_g64 = lerpResult4_g64;
			float3 hsvTorgb30_g64 = RGBToHSV( _TintA.rgb );
			float3 hsvTorgb33_g64 = HSVToRGB( float3(hsvTorgb30_g64.x,( hsvTorgb30_g64.y + 1.0 ),hsvTorgb30_g64.z) );
			float4 lerpResult38_g64 = lerp( _TintA , CalculateContrast(0.15,float4( hsvTorgb33_g64 , 0.0 )) , ( Fresnel12_g64 * _DarkenA ));
			float4 ColorA125_g64 = lerpResult38_g64;
			float2 uv_DecalMask = i.uv_texcoord * _DecalMask_ST.xy + _DecalMask_ST.zw;
			float4 tex2DNode131_g64 = tex2D( _DecalMask, uv_DecalMask );
			float MaskA132_g64 = tex2DNode131_g64.r;
			float4 lerpResult129_g64 = lerp( ColorBase124_g64 , ColorA125_g64 , MaskA132_g64);
			float3 hsvTorgb40_g64 = RGBToHSV( _TintB.rgb );
			float3 hsvTorgb43_g64 = HSVToRGB( float3(hsvTorgb40_g64.x,( hsvTorgb40_g64.y + 1.0 ),hsvTorgb40_g64.z) );
			float4 lerpResult48_g64 = lerp( _TintB , CalculateContrast(0.15,float4( hsvTorgb43_g64 , 0.0 )) , ( Fresnel12_g64 * _DarkenB ));
			float4 ColorB126_g64 = lerpResult48_g64;
			float MaskB133_g64 = tex2DNode131_g64.g;
			float4 lerpResult137_g64 = lerp( lerpResult129_g64 , ColorB126_g64 , MaskB133_g64);
			float3 hsvTorgb50_g64 = RGBToHSV( _TintC.rgb );
			float3 hsvTorgb53_g64 = HSVToRGB( float3(hsvTorgb50_g64.x,( hsvTorgb50_g64.y + 1.0 ),hsvTorgb50_g64.z) );
			float4 lerpResult58_g64 = lerp( _TintC , CalculateContrast(0.15,float4( hsvTorgb53_g64 , 0.0 )) , ( Fresnel12_g64 * _DarkenC ));
			float4 ColorC127_g64 = lerpResult58_g64;
			float MaskC134_g64 = tex2DNode131_g64.b;
			float4 lerpResult140_g64 = lerp( lerpResult137_g64 , ColorC127_g64 , MaskC134_g64);
			float2 uv_DiffuseTex = i.uv_texcoord * _DiffuseTex_ST.xy + _DiffuseTex_ST.zw;
			#if defined(_ALBEDOINPUT_TEXTURE)
				float4 staticSwitch1_g65 = tex2D( _DiffuseTex, uv_DiffuseTex );
			#elif defined(_ALBEDOINPUT_SOLIDCOLOR)
				float4 staticSwitch1_g65 = _BaseColor;
			#elif defined(_ALBEDOINPUT_VERTEXCOLOR)
				float4 staticSwitch1_g65 = i.vertexColor;
			#else
				float4 staticSwitch1_g65 = tex2D( _DiffuseTex, uv_DiffuseTex );
			#endif
			float4 break5_g65 = staticSwitch1_g65;
			float3 appendResult7_g65 = (float3(break5_g65.r , break5_g65.g , break5_g65.b));
			float4 I_Diffuse42 = ( _BaseTint * float4( appendResult7_g65 , 0.0 ) );
			float MaskX135_g64 = tex2DNode131_g64.a;
			float4 lerpResult143_g64 = lerp( lerpResult140_g64 , I_Diffuse42 , MaskX135_g64);
			float4 F_Diffuse52 = lerpResult143_g64;
			o.Albedo = F_Diffuse52.rgb;
			float2 uv_EmissionTex = i.uv_texcoord * _EmissionTex_ST.xy + _EmissionTex_ST.zw;
			#ifdef _USEEMISSIONTEX_ON
				float4 staticSwitch15_g65 = tex2D( _EmissionTex, uv_EmissionTex );
			#else
				float4 staticSwitch15_g65 = float4(0,0,0,1);
			#endif
			float4 temp_cast_14 = (0.0).xxxx;
			float4 I_Emission44 = ( ( _EmissionBoost * staticSwitch15_g65 ) + (( _UseEmissiveTint )?( ( (staticSwitch15_g65).w * _EmissiveTint ) ):( temp_cast_14 )) );
			o.Emission = I_Emission44.xyz;
			float lerpResult168_g64 = lerp( _MetalBase , _MetalA , MaskA132_g64);
			float lerpResult170_g64 = lerp( lerpResult168_g64 , _MetalB , MaskB133_g64);
			float lerpResult174_g64 = lerp( lerpResult170_g64 , _MetalC , MaskC134_g64);
			float2 uv_ORMTex = i.uv_texcoord * _ORMTex_ST.xy + _ORMTex_ST.zw;
			#ifdef _USEORM_ON
				float4 staticSwitch38_g65 = tex2D( _ORMTex, uv_ORMTex );
			#else
				float4 staticSwitch38_g65 = float4(1,0.5,0,0);
			#endif
			float4 break40_g65 = staticSwitch38_g65;
			float Metallic59_g65 = break40_g65.z;
			float I_Metallic45 = saturate( ( Metallic59_g65 + ( ( _Metallic * 2.0 ) - 1.0 ) ) );
			float lerpResult176_g64 = lerp( lerpResult174_g64 , I_Metallic45 , MaskX135_g64);
			float F_Metallic53 = lerpResult176_g64;
			o.Metallic = F_Metallic53;
			float2 temp_cast_18 = (_FlakesSize).xx;
			float2 uv_TexCoord62_g64 = i.uv_texcoord * temp_cast_18;
			float2 temp_cast_19 = (( _FlakesSize * 0.75 )).xx;
			float2 uv_TexCoord65_g64 = i.uv_texcoord * temp_cast_19;
			float Flakes86_g64 = saturate( pow( (0.45 + (( tex2D( _Flakes, uv_TexCoord62_g64 ).r * ( 1.0 - tex2D( _Flakes, uv_TexCoord65_g64 ).r ) ) - -1.0) * (0.8 - 0.45) / (1.0 - -1.0)) , abs( 2.48 ) ) );
			float lerpResult90_g64 = lerp( _RoughBase , Flakes86_g64 , ( _FlakesBase * ( 1.0 - Fresnel12_g64 ) ));
			float RoughBase98_g64 = lerpResult90_g64;
			float lerpResult99_g64 = lerp( _RoughA , Flakes86_g64 , ( _FlakesA * ( 1.0 - Fresnel12_g64 ) ));
			float RoughA106_g64 = lerpResult99_g64;
			float lerpResult151_g64 = lerp( RoughBase98_g64 , RoughA106_g64 , MaskA132_g64);
			float lerpResult107_g64 = lerp( _RoughB , Flakes86_g64 , ( _FlakesB * ( 1.0 - Fresnel12_g64 ) ));
			float RoughB114_g64 = lerpResult107_g64;
			float lerpResult152_g64 = lerp( lerpResult151_g64 , RoughB114_g64 , MaskB133_g64);
			float lerpResult115_g64 = lerp( _RoughC , Flakes86_g64 , ( _FlakesC * ( 1.0 - Fresnel12_g64 ) ));
			float RoughC122_g64 = lerpResult115_g64;
			float lerpResult155_g64 = lerp( lerpResult152_g64 , RoughC122_g64 , MaskC134_g64);
			float Roughness58_g65 = break40_g65.y;
			float I_Roughness46 = saturate( ( Roughness58_g65 + ( ( _Roughness * 2.0 ) - 1.0 ) ) );
			float lerpResult158_g64 = lerp( lerpResult155_g64 , I_Roughness46 , MaskX135_g64);
			float F_Roughness54 = lerpResult158_g64;
			o.Smoothness = ( 1.0 - F_Roughness54 );
			float Occlusion47_g65 = break40_g65.x;
			float I_Occlusion47 = saturate( ( Occlusion47_g65 + ( 1.0 - _Occlusion ) ) );
			o.Occlusion = I_Occlusion47;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.vertexColor = IN.color;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19302
Node;AmplifyShaderEditor.FunctionNode;102;-1945.574,51.10452;Inherit;False;ASF_MasterInputs;0;;65;3bfdaf0e9a0c6b348bfb29ea14ab3f9c;0;0;7;COLOR;0;FLOAT3;36;FLOAT4;28;FLOAT;71;FLOAT;64;FLOAT;46;FLOAT;27
Node;AmplifyShaderEditor.RegisterLocalVarNode;42;-1645.671,-85.11873;Inherit;False;I_Diffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-1646.833,128.3463;Inherit;False;I_Metallic;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;46;-1644.513,199.1147;Inherit;False;I_Roughness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;50;-1311.241,115.9296;Inherit;False;45;I_Metallic;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;51;-1308.921,186.698;Inherit;False;46;I_Roughness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;49;-1312.4,40.52061;Inherit;False;42;I_Diffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;101;-1086.284,91.75784;Inherit;False;ASF_Decal;21;;64;83824157a5c52ad40980bd94598e0b5c;0;3;145;COLOR;1,1,1,0;False;178;FLOAT;0;False;160;FLOAT;0.5;False;3;COLOR;0;FLOAT;177;FLOAT;162
Node;AmplifyShaderEditor.RegisterLocalVarNode;54;-811.4245,189.7434;Inherit;False;F_Roughness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;47;-1642.193,268.7227;Inherit;False;I_Occlusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;43;-1645.673,-13.19054;Inherit;False;I_Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-1645.673,56.41771;Inherit;False;I_Emission;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;53;-812.4097,115.8246;Inherit;False;F_Metallic;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;59;-450.3345,125.7097;Inherit;False;54;F_Roughness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;52;-813.6385,36.69074;Inherit;False;F_Diffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;48;-1642.193,339.4908;Inherit;False;I_Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;56;-419.934,-92.39018;Inherit;False;43;I_Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;57;-422.5342,-20.89014;Inherit;False;44;I_Emission;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-419.9339,51.90983;Inherit;False;53;F_Metallic;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;61;-417.334,276.8102;Inherit;False;48;I_Alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;60;-421.2339,201.1098;Inherit;False;47;I_Occlusion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;-419.2337,-167.1903;Inherit;False;52;F_Diffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;41;-159.1956,96.30266;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Futureverse/Decal/Standard;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;42;0;102;0
WireConnection;45;0;102;71
WireConnection;46;0;102;64
WireConnection;101;145;49;0
WireConnection;101;178;50;0
WireConnection;101;160;51;0
WireConnection;54;0;101;162
WireConnection;47;0;102;46
WireConnection;43;0;102;36
WireConnection;44;0;102;28
WireConnection;53;0;101;177
WireConnection;52;0;101;0
WireConnection;48;0;102;27
WireConnection;41;0;59;0
WireConnection;0;0;55;0
WireConnection;0;1;56;0
WireConnection;0;2;57;0
WireConnection;0;3;58;0
WireConnection;0;4;41;0
WireConnection;0;5;60;0
ASEEND*/
//CHKSM=14474E2AA9A68708C8628AA3F0AC8B62FC529EAB