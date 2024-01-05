// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Vefects/SH_Vefects_VFX_SRP_Particles_Erosion_01"
{
	Properties
	{
		_Noise_01_Texture("Noise_01_Texture", 2D) = "white" {}
		_Noise_02_Texture("Noise_02_Texture", 2D) = "white" {}
		_MaskTexture("Mask Texture", 2D) = "white" {}
		_MaskMoveTexture("Mask Move Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,0)
		_NoiseDistortionTexture("Noise Distortion Texture", 2D) = "white" {}
		_Noise01Scale("Noise 01 Scale", Vector) = (0.8,0.8,0,0)
		_Noise02Scale("Noise 02 Scale", Vector) = (1,1,0,0)
		_NoiseDistortionScale("Noise Distortion Scale", Vector) = (1,1,0,0)
		_Noise01Speed("Noise 01 Speed", Vector) = (0.5,0.5,0,0)
		_Noise02Speed("Noise 02 Speed", Vector) = (-0.2,0.4,0,0)
		_MaskMoveScale("Mask Move Scale", Vector) = (1,1,0,0)
		_MaskScale("Mask Scale", Vector) = (1,1,0,0)
		_MaskOffset("Mask Offset", Vector) = (0,0,0,0)
		_NoiseDistortionSpeed("Noise Distortion Speed", Vector) = (0.2,0.25,0,0)
		_MaskMultiply("Mask Multiply", Float) = 1
		_MaskMoveMultiply("Mask Move Multiply", Float) = 1
		_NoisesMultiply("Noises Multiply", Float) = 1
		_MaskPower("Mask Power", Float) = 1
		_NoisesPower("Noises Power", Float) = 1
		_MaskMovePower("Mask Move Power", Float) = 1
		_Distortion("Distortion", Float) = 1
		_DistortionIntensity("Distortion Intensity", Float) = 0
		_OpacityBoost("Opacity Boost", Float) = 5
		_EmissionIntensity("Emission Intensity", Float) = 1
		_WindSpeed("Wind Speed", Float) = 1
		_MaskSpeed("Mask Speed", Float) = 0
		_Dissolve("Dissolve", Float) = 0
		[Space(13)][Header(AR)][Space(13)]_Cull1("Cull", Float) = 2
		_Src1("Src", Float) = 5
		_Dst1("Dst", Float) = 10
		_ZWrite1("ZWrite", Float) = 0
		_ZTest1("ZTest", Float) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull [_Cull1]
		ZWrite [_ZWrite1]
		ZTest [_ZTest1]
		Blend [_Src1] [_Dst1]
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float4 uv2_texcoord2;
		};

		uniform float _Cull1;
		uniform float _Src1;
		uniform float _Dst1;
		uniform float _ZTest1;
		uniform float _ZWrite1;
		uniform float4 _Color;
		uniform sampler2D _MaskTexture;
		uniform sampler2D _NoiseDistortionTexture;
		uniform float _WindSpeed;
		uniform float2 _NoiseDistortionSpeed;
		uniform float2 _NoiseDistortionScale;
		uniform float _Distortion;
		uniform float _DistortionIntensity;
		uniform float _MaskSpeed;
		uniform float2 _MaskScale;
		uniform float2 _MaskOffset;
		uniform float _MaskPower;
		uniform float _MaskMultiply;
		uniform sampler2D _MaskMoveTexture;
		uniform float2 _MaskMoveScale;
		uniform float _MaskMovePower;
		uniform float _MaskMoveMultiply;
		uniform sampler2D _Noise_01_Texture;
		uniform float2 _Noise01Speed;
		uniform float2 _Noise01Scale;
		uniform sampler2D _Noise_02_Texture;
		uniform float2 _Noise02Speed;
		uniform float2 _Noise02Scale;
		uniform float _NoisesPower;
		uniform float _NoisesMultiply;
		uniform float _EmissionIntensity;
		uniform float _OpacityBoost;
		uniform float _Dissolve;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float windSpeed13 = ( _WindSpeed * _Time.y );
			float2 uv_TexCoord15 = i.uv_texcoord * _NoiseDistortionScale;
			float2 panner18 = ( windSpeed13 * _NoiseDistortionSpeed + uv_TexCoord15);
			float Distortion31 = ( ( tex2D( _NoiseDistortionTexture, panner18 ).r * 0.1 ) * _Distortion );
			float2 appendResult44 = (float2(_MaskSpeed , 0.0));
			float2 uv_TexCoord39 = i.uv_texcoord * _MaskScale + _MaskOffset;
			float2 panner49 = ( windSpeed13 * appendResult44 + uv_TexCoord39);
			float2 uv_TexCoord50 = i.uv_texcoord * _MaskMoveScale;
			float2 appendResult52 = (float2(i.uv2_texcoord2.z , i.uv2_texcoord2.w));
			float2 uv_TexCoord28 = i.uv_texcoord * _Noise01Scale;
			float2 panner45 = ( windSpeed13 * _Noise01Speed + uv_TexCoord28);
			float2 uv_TexCoord35 = i.uv_texcoord * _Noise02Scale;
			float2 panner43 = ( windSpeed13 * _Noise02Speed + uv_TexCoord35);
			float Noises77 = saturate( ( pow( ( tex2D( _Noise_01_Texture, ( Distortion31 + panner45 ) ).r * tex2D( _Noise_02_Texture, ( Distortion31 + panner43 ) ).r ) , _NoisesPower ) * _NoisesMultiply ) );
			float temp_output_82_0 = ( saturate( ( saturate( ( pow( tex2D( _MaskTexture, ( ( Distortion31 * _DistortionIntensity ) + panner49 ) ).r , _MaskPower ) * _MaskMultiply ) ) * saturate( ( pow( tex2D( _MaskMoveTexture, ( uv_TexCoord50 + appendResult52 ) ).r , _MaskMovePower ) * _MaskMoveMultiply ) ) ) ) * Noises77 );
			o.Emission = ( ( ( _Color * i.vertexColor ) * temp_output_82_0 ) * _EmissionIntensity ).rgb;
			float temp_output_90_0 = ( saturate( ( temp_output_82_0 * _OpacityBoost ) ) - ( i.uv2_texcoord2.x + _Dissolve ) );
			o.Alpha = saturate( ( i.vertexColor.a * temp_output_90_0 ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;2;640,0;Inherit;False;1238;166;Auto Register Variables;5;7;6;5;4;3;Lush was here! <3;0.4872068,0.2971698,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;7;688,48;Inherit;False;Property;_Cull1;Cull;31;0;Create;True;0;0;0;True;3;Space(13);Header(AR);Space(13);False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;944,48;Inherit;False;Property;_Src1;Src;32;0;Create;True;0;0;0;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;1200,48;Inherit;False;Property;_Dst1;Dst;33;0;Create;True;0;0;0;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;1712,48;Inherit;False;Property;_ZTest1;ZTest;35;0;Create;True;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;1456,48;Inherit;False;Property;_ZWrite1;ZWrite;34;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Vefects/SH_Vefects_VFX_SRP_Particles_Erosion_01;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;True;_ZWrite1;0;True;_ZTest1;False;0;False;;0;False;;False;0;Custom;0.5;True;False;0;True;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;True;_Src1;10;True;_Dst1;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;True;_Cull1;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;8;-5456.596,-2091.256;Inherit;False;786;289;Wind Speed;4;13;11;10;9;;0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-5406.596,-2041.255;Inherit;False;Property;_WindSpeed;Wind Speed;28;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;10;-5406.596,-1913.257;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-5150.596,-2041.255;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;12;-5442.63,-1450.251;Inherit;False;2184.752;611.1584;Distortion;11;31;26;22;21;20;19;18;17;16;15;14;;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-4894.594,-2041.255;Inherit;False;windSpeed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;14;-5405.821,-1350.724;Inherit;False;Property;_NoiseDistortionScale;Noise Distortion Scale;10;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;15;-5151.36,-1371.416;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;16;-5130.83,-1090.506;Inherit;False;13;windSpeed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;17;-5183.386,-1214.482;Inherit;False;Property;_NoiseDistortionSpeed;Noise Distortion Speed;16;0;Create;True;0;0;0;False;0;False;0.2,0.25;0.2,0.25;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;18;-4898.217,-1370.466;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;19;-4712.369,-1400.251;Inherit;True;Property;_NoiseDistortionTexture;Noise Distortion Texture;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;20;-4423.413,-1240.914;Inherit;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-4222.655,-1233.838;Inherit;False;Property;_Distortion;Distortion;23;0;Create;True;0;0;0;False;0;False;1;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-4280.223,-1367.796;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;23;-8592.217,-1481.475;Inherit;False;2458.886;1220.625;Noises;23;77;73;70;68;66;62;60;57;54;53;51;47;45;43;38;35;34;32;29;28;27;25;24;;0,0,0,1;0;0
Node;AmplifyShaderEditor.Vector2Node;24;-8530.731,-1335.278;Inherit;False;Property;_Noise01Scale;Noise 01 Scale;7;0;Create;True;0;0;0;False;0;False;0.8,0.8;2,2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;25;-8373.678,-696.809;Inherit;False;Property;_Noise02Scale;Noise 02 Scale;8;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-4047.655,-1368.499;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-8074.121,-370.9482;Inherit;False;13;windSpeed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;28;-8320.631,-1354.248;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;29;-8299.617,-985.87;Inherit;False;13;windSpeed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;30;-7077.145,1098.682;Inherit;False;Property;_MaskOffset;Mask Offset;15;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-3803.642,-1374.766;Inherit;False;Distortion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;32;-8128.916,-537.4708;Inherit;False;Property;_Noise02Speed;Noise 02 Speed;12;0;Create;True;0;0;0;False;0;False;-0.2,0.4;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;33;-7077.145,970.6823;Inherit;False;Property;_MaskScale;Mask Scale;14;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;34;-8302.655,-1165.84;Inherit;False;Property;_Noise01Speed;Noise 01 Speed;11;0;Create;True;0;0;0;False;0;False;0.5,0.5;0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;35;-8158.98,-715.7943;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;36;-6821.145,1098.682;Inherit;False;Property;_MaskSpeed;Mask Speed;29;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;37;-7077.145,1482.682;Inherit;False;Property;_MaskMoveScale;Mask Move Scale;13;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;38;-7883.157,-803.1503;Inherit;False;31;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;39;-6821.145,970.6823;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0.28,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;40;-7077.145,586.6821;Inherit;False;Property;_DistortionIntensity;Distortion Intensity;24;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;41;-7077.145,458.682;Inherit;False;31;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;42;-6437.145,1098.682;Inherit;False;13;windSpeed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;43;-7900.272,-711.9257;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;44;-6821.145,1226.682;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;45;-8070.227,-1306.917;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;46;-7077.145,1738.682;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;47;-7786.916,-1429.177;Inherit;False;31;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-6693.145,458.682;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;49;-6437.145,970.6823;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;50;-6821.145,1482.682;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-7587.17,-1333.907;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;52;-6821.145,1738.682;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;53;-7695.131,-740.3961;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;54;-7432.92,-1164.766;Inherit;True;Property;_Noise_01_Texture;Noise_01_Texture;1;0;Create;True;0;0;0;False;0;False;-1;None;f1b6b10620016be45b38159cac55e09b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;55;-6181.145,1482.682;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;56;-6181.145,970.6823;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;57;-7468.396,-772.3779;Inherit;True;Property;_Noise_02_Texture;Noise_02_Texture;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;58;-6053.145,970.6823;Inherit;True;Property;_MaskTexture;Mask Texture;3;0;Create;True;0;0;0;False;0;False;-1;None;cd946dcaa5face64fb135a09e293d277;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;59;-6053.145,1482.682;Inherit;True;Property;_MaskMoveTexture;Mask Move Texture;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-7097.63,-941.9957;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-5669.145,1610.682;Inherit;False;Property;_MaskMovePower;Mask Move Power;22;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-7083.905,-723.2778;Inherit;False;Property;_NoisesPower;Noises Power;21;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-5669.145,1098.682;Inherit;False;Property;_MaskPower;Mask Power;20;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;64;-5669.145,970.6823;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-5413.145,1098.682;Inherit;False;Property;_MaskMultiply;Mask Multiply;17;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-6866.75,-714.9881;Inherit;False;Property;_NoisesMultiply;Noises Multiply;19;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-5413.145,1610.682;Inherit;False;Property;_MaskMoveMultiply;Mask Move Multiply;18;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;68;-6870.528,-940.0291;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;69;-5669.145,1482.682;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-6696.058,-945.4979;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-5413.145,1482.682;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-5413.145,970.6823;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;73;-6522.298,-943.2118;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;74;-5157.145,970.6823;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;75;-5157.145,1482.682;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-4901.143,1226.682;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;77;-6371.447,-950.3036;Inherit;False;Noises;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;79;-4773.143,1226.682;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;80;-3125.14,842.6824;Inherit;False;Property;_OpacityBoost;Opacity Boost;25;0;Create;True;0;0;0;False;0;False;5;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;81;-1879.138,536.6821;Inherit;False;855;550;Depth Fade;5;97;93;91;88;84;;0,0,0,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;-4133.14,330.682;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-2469.14,1098.682;Inherit;False;Property;_Dissolve;Dissolve;30;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;84;-1829.138,970.6823;Inherit;False;Property;_DepthFade;Depth Fade;26;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-3109.14,586.6821;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;86;-2725.14,970.6823;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;87;-2469.14,970.6823;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;88;-1829.138,842.6824;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;89;-2853.14,586.6821;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;90;-2341.139,586.6821;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;91;-1573.138,842.6824;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;92;-1495.138,1560.682;Inherit;False;980;849;Camera Offset;8;107;103;101;100;99;96;95;94;;0,0,0,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;-1445.138,586.6821;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;94;-1445.138,2250.683;Inherit;False;3;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;95;-1445.138,2122.683;Inherit;False;Property;_CameraOffset;Camera Offset;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;96;-1061.139,2122.683;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;97;-1189.139,586.6821;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;98;-2213.139,-693.3179;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;99;-1445.138,1610.682;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;100;-1445.138,1866.682;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;101;-677.1395,2122.683;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-933.1397,74.68204;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;103;-1061.139,1610.682;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;104;-933.1397,-181.3179;Inherit;False;Property;_EmissionIntensity;Emission Intensity;27;0;Create;True;0;0;0;False;0;False;1;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;-1701.138,-949.3179;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;106;-2213.139,-949.3179;Inherit;False;Property;_Color;Color;5;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-677.1395,1610.682;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;-933.1397,-309.3178;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;-1189.139,-949.3179;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;108;-675.3646,74.68204;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;78;-4133.14,202.682;Inherit;False;77;Noises;1;0;OBJECT;;False;1;FLOAT;0
WireConnection;0;2;109;0
WireConnection;0;9;108;0
WireConnection;11;0;9;0
WireConnection;11;1;10;0
WireConnection;13;0;11;0
WireConnection;15;0;14;0
WireConnection;18;0;15;0
WireConnection;18;2;17;0
WireConnection;18;1;16;0
WireConnection;19;1;18;0
WireConnection;22;0;19;1
WireConnection;22;1;20;0
WireConnection;26;0;22;0
WireConnection;26;1;21;0
WireConnection;28;0;24;0
WireConnection;31;0;26;0
WireConnection;35;0;25;0
WireConnection;39;0;33;0
WireConnection;39;1;30;0
WireConnection;43;0;35;0
WireConnection;43;2;32;0
WireConnection;43;1;27;0
WireConnection;44;0;36;0
WireConnection;45;0;28;0
WireConnection;45;2;34;0
WireConnection;45;1;29;0
WireConnection;48;0;41;0
WireConnection;48;1;40;0
WireConnection;49;0;39;0
WireConnection;49;2;44;0
WireConnection;49;1;42;0
WireConnection;50;0;37;0
WireConnection;51;0;47;0
WireConnection;51;1;45;0
WireConnection;52;0;46;3
WireConnection;52;1;46;4
WireConnection;53;0;38;0
WireConnection;53;1;43;0
WireConnection;54;1;51;0
WireConnection;55;0;50;0
WireConnection;55;1;52;0
WireConnection;56;0;48;0
WireConnection;56;1;49;0
WireConnection;57;1;53;0
WireConnection;58;1;56;0
WireConnection;59;1;55;0
WireConnection;60;0;54;1
WireConnection;60;1;57;1
WireConnection;64;0;58;1
WireConnection;64;1;63;0
WireConnection;68;0;60;0
WireConnection;68;1;62;0
WireConnection;69;0;59;1
WireConnection;69;1;61;0
WireConnection;70;0;68;0
WireConnection;70;1;66;0
WireConnection;71;0;69;0
WireConnection;71;1;67;0
WireConnection;72;0;64;0
WireConnection;72;1;65;0
WireConnection;73;0;70;0
WireConnection;74;0;72;0
WireConnection;75;0;71;0
WireConnection;76;0;74;0
WireConnection;76;1;75;0
WireConnection;77;0;73;0
WireConnection;79;0;76;0
WireConnection;82;0;79;0
WireConnection;82;1;78;0
WireConnection;85;0;82;0
WireConnection;85;1;80;0
WireConnection;87;0;86;1
WireConnection;87;1;83;0
WireConnection;88;0;84;0
WireConnection;89;0;85;0
WireConnection;90;0;89;0
WireConnection;90;1;87;0
WireConnection;91;0;88;0
WireConnection;93;0;90;0
WireConnection;93;1;91;0
WireConnection;96;0;95;0
WireConnection;96;1;94;2
WireConnection;97;0;93;0
WireConnection;101;0;96;0
WireConnection;102;0;98;4
WireConnection;102;1;90;0
WireConnection;103;0;99;0
WireConnection;103;1;100;0
WireConnection;105;0;106;0
WireConnection;105;1;98;0
WireConnection;107;0;103;0
WireConnection;107;1;101;0
WireConnection;109;0;110;0
WireConnection;109;1;104;0
WireConnection;110;0;105;0
WireConnection;110;1;82;0
WireConnection;108;0;102;0
ASEEND*/
//CHKSM=198B67EBB9EEF95084D190899521331B66DDFCF1