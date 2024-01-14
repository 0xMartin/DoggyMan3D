// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "OnTheTopShader"
{
	Properties
	{
		_EdgeLength ( "Edge length", Range( 2, 50 ) ) = 50
		_Base_Albedo("Base_Albedo", 2D) = "white" {}
		_Base_NormalMap("Base_NormalMap", 2D) = "bump" {}
		_Base_SpecGloss("Base_SpecGloss", 2D) = "black" {}
		_OnTop_Tint("OnTop_Tint", Color) = (1,1,1,0)
		_OnTop_Emissiveness("OnTop_Emissiveness", Float) = 0
		_OnTop_Albdeo("OnTop_Albdeo", 2D) = "white" {}
		_OnTop_NormalMap("OnTop_NormalMap", 2D) = "bump" {}
		_OnTop_SpecCol("OnTop_SpecCol", 2D) = "black" {}
		_OnTop_SpecAmount("OnTop_SpecAmount", Range( 0 , 1)) = 0
		_OnTop_Glossyness("OnTop_Glossyness", Range( 0 , 1)) = 0
		_OnTop_Amount("OnTop_Amount", Range( 0 , 2)) = 1
		_OnTop_Tiling("OnTop_Tiling", Float) = 0.2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "Tessellation.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
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
		};

		uniform sampler2D _Base_NormalMap;
		uniform float4 _Base_NormalMap_ST;
		uniform sampler2D _OnTop_NormalMap;
		uniform float _OnTop_Tiling;
		uniform float _OnTop_Amount;
		uniform sampler2D _Base_Albedo;
		uniform float4 _Base_Albedo_ST;
		uniform float4 _OnTop_Tint;
		uniform sampler2D _OnTop_Albdeo;
		uniform float _OnTop_Emissiveness;
		uniform sampler2D _Base_SpecGloss;
		uniform float4 _Base_SpecGloss_ST;
		uniform sampler2D _OnTop_SpecCol;
		uniform float _OnTop_SpecAmount;
		uniform float _OnTop_Glossyness;
		uniform float _EdgeLength;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
		}

		void vertexDataFunc( inout appdata_full v )
		{
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_Base_NormalMap = i.uv_texcoord * _Base_NormalMap_ST.xy + _Base_NormalMap_ST.zw;
			float3 ase_worldPos = i.worldPos;
			float4 appendResult13 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 temp_output_14_0 = ( appendResult13 * _OnTop_Tiling );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float2 uv_Base_Albedo = i.uv_texcoord * _Base_Albedo_ST.xy + _Base_Albedo_ST.zw;
			float4 tex2DNode1 = tex2D( _Base_Albedo, uv_Base_Albedo );
			float clampResult49 = clamp( ( ( ( tex2DNode1.r + tex2DNode1.g + tex2DNode1.b ) / 3.0 ) + 0.5 ) , 0.0 , 1.0 );
			float clampResult20 = clamp( ( ase_worldNormal.y * _OnTop_Amount * clampResult49 ) , 0.0 , 1.0 );
			float3 lerpResult29 = lerp( UnpackNormal( tex2D( _Base_NormalMap, uv_Base_NormalMap ) ) , UnpackNormal( tex2D( _OnTop_NormalMap, temp_output_14_0.xy ) ) , clampResult20);
			o.Normal = lerpResult29;
			float4 lerpResult17 = lerp( tex2DNode1 , ( _OnTop_Tint * tex2D( _OnTop_Albdeo, temp_output_14_0.xy ) ) , clampResult20);
			o.Albedo = lerpResult17.rgb;
			o.Emission = ( lerpResult17 * clampResult20 * _OnTop_Emissiveness ).rgb;
			float2 uv_Base_SpecGloss = i.uv_texcoord * _Base_SpecGloss_ST.xy + _Base_SpecGloss_ST.zw;
			float4 tex2DNode3 = tex2D( _Base_SpecGloss, uv_Base_SpecGloss );
			float4 tex2DNode37 = tex2D( _OnTop_SpecCol, temp_output_14_0.xy );
			float4 lerpResult30 = lerp( tex2DNode3 , ( tex2DNode37 * _OnTop_SpecAmount ) , clampResult20);
			o.Specular = lerpResult30.rgb;
			float lerpResult34 = lerp( tex2DNode3.a , ( tex2DNode37.a * _OnTop_Glossyness ) , clampResult20);
			o.Smoothness = lerpResult34;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardSpecular keepalpha fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v );
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
				SurfaceOutputStandardSpecular o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardSpecular, o )
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
Version=15401
683;278;1096;704;1432.475;762.2641;3.283547;True;False
Node;AmplifyShaderEditor.SamplerNode;1;-517.4953,-169.7303;Float;True;Property;_Base_Albedo;Base_Albedo;5;0;Create;True;0;0;False;0;None;673319f47d7ac264db2a2163cfbcd79a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;51;-141.8823,47.27582;Float;False;Constant;_Float2;Float 2;11;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;46;-123.9439,-89.16405;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-143.207,128.0823;Float;False;Constant;_Float3;Float 3;11;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;50;30.32826,-48.10236;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;11;-1445.33,363.8387;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;52;47.54931,59.19806;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-1163.946,614.5886;Float;False;Property;_OnTop_Tiling;OnTop_Tiling;16;0;Create;True;0;0;False;0;0.2;0.15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;13;-1059.118,356.1941;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-90.99118,565.5573;Float;False;Property;_OnTop_Amount;OnTop_Amount;15;0;Create;True;0;0;False;0;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;23;-63.90396,345.6478;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-787.6019,590.0034;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ClampOpNode;49;170.7467,87.01685;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;223.564,433.8817;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;54;-639.886,425.5852;Float;False;Property;_OnTop_Tint;OnTop_Tint;8;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-500.9354,556.5016;Float;True;Property;_OnTop_Albdeo;OnTop_Albdeo;10;0;Create;True;0;0;False;0;None;149d1a4219083e14dacfe4fc99363d0e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;20;588.5486,611.0365;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-484.9903,1500.204;Float;False;Property;_OnTop_SpecAmount;OnTop_SpecAmount;13;0;Create;True;0;0;False;0;0;0.55;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-151.0692,1477.244;Float;False;Property;_OnTop_Glossyness;OnTop_Glossyness;14;0;Create;True;0;0;False;0;0;0.97;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-210.4199,466.7413;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;37;-837.9261,961.7571;Float;True;Property;_OnTop_SpecCol;OnTop_SpecCol;12;0;Create;True;0;0;False;0;None;15e2d54eab956f747add8cb34500a4b5;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;17;632.1578,-288.1697;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-78.644,1125.676;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;2;-512.0527,31.63861;Float;True;Property;_Base_NormalMap;Base_NormalMap;6;0;Create;True;0;0;False;0;None;b4b00fa2da00540458ce4e17425ade49;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;35;1121.602,43.82435;Float;False;Property;_OnTop_Emissiveness;OnTop_Emissiveness;9;0;Create;True;0;0;False;0;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-496.1175,779.2854;Float;True;Property;_OnTop_NormalMap;OnTop_NormalMap;11;0;Create;True;0;0;False;0;None;a976f69ac07481f44aa65191fe82c8ba;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;244.1228,1284.991;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-506.61,237.0898;Float;True;Property;_Base_SpecGloss;Base_SpecGloss;7;0;Create;True;0;0;False;0;None;bba048fbbbcbf1d42b1b64331166f119;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;1440.006,-15.18852;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;34;1033.489,467.1191;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;29;678.3016,-160.4414;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;30;932.064,-55.98749;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldNormalVector;21;-1402.451,124.2282;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1711.851,-175.0823;Float;False;True;6;Float;ASEMaterialInspector;0;0;StandardSpecular;OnTheTopShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;50;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;0;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;46;0;1;1
WireConnection;46;1;1;2
WireConnection;46;2;1;3
WireConnection;50;0;46;0
WireConnection;50;1;51;0
WireConnection;52;0;50;0
WireConnection;52;1;53;0
WireConnection;13;0;11;1
WireConnection;13;1;11;3
WireConnection;14;0;13;0
WireConnection;14;1;15;0
WireConnection;49;0;52;0
WireConnection;18;0;23;2
WireConnection;18;1;19;0
WireConnection;18;2;49;0
WireConnection;4;1;14;0
WireConnection;20;0;18;0
WireConnection;55;0;54;0
WireConnection;55;1;4;0
WireConnection;37;1;14;0
WireConnection;17;0;1;0
WireConnection;17;1;55;0
WireConnection;17;2;20;0
WireConnection;39;0;37;0
WireConnection;39;1;32;0
WireConnection;5;1;14;0
WireConnection;40;0;37;4
WireConnection;40;1;9;0
WireConnection;36;0;17;0
WireConnection;36;1;20;0
WireConnection;36;2;35;0
WireConnection;34;0;3;4
WireConnection;34;1;40;0
WireConnection;34;2;20;0
WireConnection;29;0;2;0
WireConnection;29;1;5;0
WireConnection;29;2;20;0
WireConnection;30;0;3;0
WireConnection;30;1;39;0
WireConnection;30;2;20;0
WireConnection;0;0;17;0
WireConnection;0;1;29;0
WireConnection;0;2;36;0
WireConnection;0;3;30;0
WireConnection;0;4;34;0
ASEEND*/
//CHKSM=6E46018FA829AAE2CB6F4A9E0CA65C05BA38DDA6