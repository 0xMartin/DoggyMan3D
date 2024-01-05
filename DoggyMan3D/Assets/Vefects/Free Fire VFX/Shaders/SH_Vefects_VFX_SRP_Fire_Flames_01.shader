// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Vefects/SH_Vefects_VFX_SRP_Fire_Flames_01"
{
	Properties
	{
		_EmissiveIntensity("Emissive Intensity", Float) = 1
		_Noise_01_Texture("Noise_01_Texture", 2D) = "white" {}
		_Noise_02_Texture("Noise_02_Texture", 2D) = "white" {}
		_ColorGradientMask("Color Gradient Mask", 2D) = "white" {}
		_DistortionMaskTexture("Distortion Mask Texture", 2D) = "white" {}
		_Mask_Texture("Mask_Texture", 2D) = "white" {}
		_TextureSample2("Texture Sample 2", 2D) = "white" {}
		_ColorBottom("Color Bottom", Color) = (1,0.3666667,0,0)
		_ColorTop("Color Top", Color) = (1,0.6666667,0,0)
		_NoiseDistortion_Texture("NoiseDistortion_Texture", 2D) = "white" {}
		_Noise_01_Scale("Noise_01_Scale", Vector) = (0.8,0.8,0,0)
		_Noise_02_Scale("Noise_02_Scale", Vector) = (1,1,0,0)
		_NoiseDistortion_Scale("NoiseDistortion_Scale", Vector) = (1,1,0,0)
		_Noise_01_Speed("Noise_01_Speed", Vector) = (0.5,0.5,0,0)
		_Noise_02_Speed("Noise_02_Speed", Vector) = (-0.2,0.4,0,0)
		_Mask_Scale("Mask_Scale", Vector) = (1,1,0,0)
		_Mask_Offset("Mask_Offset", Vector) = (0,0,0,0)
		_NoiseDistortion_Speed("NoiseDistortion_Speed", Vector) = (-0.3,-0.3,0,0)
		_Mask_Multiply("Mask_Multiply", Float) = 1
		_Noises_Multiply("Noises_Multiply", Float) = 5
		_Mask_Power("Mask_Power", Float) = 1
		_Noises_Power("Noises_Power", Float) = 1
		_DistortionAmount("Distortion Amount", Float) = 1
		_DistortionMaskIntensity("Distortion Mask Intensity", Float) = 1
		_NoisesOpacityBoost("Noises Opacity Boost", Float) = 1
		_DepthFade("Depth Fade", Float) = 1
		_WindSpeed("Wind Speed", Float) = 1
		_Dissolve("Dissolve", Float) = 0
		[Space(13)][Header(AR)][Space(13)]_Cull1("Cull", Float) = 2
		_Src1("Src", Float) = 5
		_Dst1("Dst", Float) = 10
		_ZWrite1("ZWrite", Float) = 0
		_ZTest1("ZTest", Float) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
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
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float _Cull1;
		uniform float _Src1;
		uniform float _Dst1;
		uniform float _ZTest1;
		uniform float _ZWrite1;
		uniform float4 _ColorBottom;
		uniform float4 _ColorTop;
		uniform sampler2D _ColorGradientMask;
		uniform float4 _ColorGradientMask_ST;
		uniform sampler2D _Mask_Texture;
		uniform sampler2D _DistortionMaskTexture;
		uniform sampler2D _NoiseDistortion_Texture;
		uniform float _WindSpeed;
		uniform float2 _NoiseDistortion_Speed;
		uniform float2 _NoiseDistortion_Scale;
		uniform float _DistortionAmount;
		uniform float _DistortionMaskIntensity;
		uniform float2 _Mask_Scale;
		uniform float2 _Mask_Offset;
		uniform float _Mask_Power;
		uniform float _Mask_Multiply;
		uniform sampler2D _Noise_01_Texture;
		uniform float2 _Noise_01_Speed;
		uniform float2 _Noise_01_Scale;
		uniform sampler2D _Noise_02_Texture;
		uniform float2 _Noise_02_Speed;
		uniform float2 _Noise_02_Scale;
		uniform float _Noises_Power;
		uniform float _Noises_Multiply;
		uniform float _NoisesOpacityBoost;
		uniform sampler2D _TextureSample2;
		uniform float _EmissiveIntensity;
		uniform float _Dissolve;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _DepthFade;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_ColorGradientMask = i.uv_texcoord * _ColorGradientMask_ST.xy + _ColorGradientMask_ST.zw;
			float4 lerpResult152 = lerp( _ColorBottom , _ColorTop , tex2D( _ColorGradientMask, uv_ColorGradientMask ).r);
			float2 uv_TexCoord92 = i.uv_texcoord + float2( 0,0.4 );
			float windSpeed60 = ( _WindSpeed * _Time.y );
			float2 panner65 = ( windSpeed60 * _NoiseDistortion_Speed + ( i.uv_texcoord * _NoiseDistortion_Scale ));
			float Distortion81 = ( ( tex2D( _NoiseDistortion_Texture, panner65 ).r * 0.1 ) * _DistortionAmount );
			float2 uv_TexCoord106 = i.uv_texcoord * _Mask_Scale + _Mask_Offset;
			float2 panner85 = ( windSpeed60 * _Noise_01_Speed + ( i.uv_texcoord * _Noise_01_Scale ));
			float2 panner83 = ( windSpeed60 * _Noise_02_Speed + ( i.uv_texcoord * _Noise_02_Scale ));
			float noises109 = saturate( ( pow( ( tex2D( _Noise_01_Texture, ( panner85 + Distortion81 ) ).r * tex2D( _Noise_02_Texture, ( panner83 + Distortion81 ) ).r ) , _Noises_Power ) * _Noises_Multiply ) );
			float2 uv_TexCoord116 = i.uv_texcoord + float2( 0,0.4 );
			float temp_output_133_0 = ( saturate( ( pow( tex2D( _Mask_Texture, ( ( tex2D( _DistortionMaskTexture, uv_TexCoord92 ).r * ( Distortion81 * _DistortionMaskIntensity ) ) + uv_TexCoord106 ) ).r , _Mask_Power ) * _Mask_Multiply ) ) - ( ( noises109 * _NoisesOpacityBoost ) * tex2D( _TextureSample2, uv_TexCoord116 ).r ) );
			o.Emission = ( ( ( i.vertexColor * lerpResult152 ) * temp_output_133_0 ) * _EmissiveIntensity ).rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth131 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth131 = abs( ( screenDepth131 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFade ) );
			o.Alpha = saturate( ( ( ( temp_output_133_0 - _Dissolve ) * saturate( distanceDepth131 ) ) * i.vertexColor.a ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;2;640,0;Inherit;False;1238;166;Auto Register Variables;5;7;6;5;4;3;Lush was here! <3;0.4872068,0.2971698,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;7;688,48;Inherit;False;Property;_Cull1;Cull;30;0;Create;True;0;0;0;True;3;Space(13);Header(AR);Space(13);False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;944,48;Inherit;False;Property;_Src1;Src;31;0;Create;True;0;0;0;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;1200,48;Inherit;False;Property;_Dst1;Dst;32;0;Create;True;0;0;0;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;1712,48;Inherit;False;Property;_ZTest1;ZTest;34;0;Create;True;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;1456,48;Inherit;False;Property;_ZWrite1;ZWrite;33;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Vefects/SH_Vefects_VFX_SRP_Fire_Flames_01;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;True;_ZWrite1;0;True;_ZTest1;False;0;False;;0;False;;False;0;Custom;0.5;True;False;0;True;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;True;_Src1;10;True;_Dst1;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;1;-1;-1;-1;0;False;0;0;True;_Cull1;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;54;-10322.84,-1815.987;Inherit;False;786;417;Register Wind Speed;4;60;58;56;55;;0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-10272.84,-1765.987;Inherit;False;Property;_WindSpeed;Wind Speed;28;0;Create;True;0;0;0;False;0;False;1;-2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;56;-10272.84,-1509.987;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;57;-7103.57,-2202.365;Inherit;False;2502.5;663.612;Heat Haze;12;81;71;70;69;67;66;65;64;63;62;61;59;;0,0,0,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-10016.84,-1765.987;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;59;-6905.494,-1920.254;Inherit;False;Property;_NoiseDistortion_Scale;NoiseDistortion_Scale;14;0;Create;True;0;0;0;False;0;False;1,1;2,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;60;-9760.843,-1765.987;Inherit;False;windSpeed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;61;-6905.494,-2048.254;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;62;-6009.494,-1792.254;Inherit;False;60;windSpeed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;63;-6393.494,-1920.254;Inherit;False;Property;_NoiseDistortion_Speed;NoiseDistortion_Speed;19;0;Create;True;0;0;0;False;0;False;-0.3,-0.3;0.3,0.6;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-6649.494,-2048.254;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;65;-6009.494,-2048.254;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-5369.494,-1920.254;Inherit;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;67;-5753.494,-2048.254;Inherit;True;Property;_NoiseDistortion_Texture;NoiseDistortion_Texture;10;0;Create;True;0;0;0;False;0;False;-1;None;78f9606de07f24a4f9ff540cc0087f36;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;68;-10318.37,-1275.525;Inherit;False;2997.113;1074.221;Noises;25;109;108;105;102;101;97;93;91;89;88;87;86;85;84;83;82;80;79;78;77;76;75;74;73;72;;0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;69;-5113.495,-1920.254;Inherit;False;Property;_DistortionAmount;Distortion Amount;24;0;Create;True;0;0;0;False;0;False;1;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-5369.494,-2048.254;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-5113.495,-2048.254;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;72;-10233.49,-640.2536;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;73;-10233.49,-512.2535;Inherit;False;Property;_Noise_02_Scale;Noise_02_Scale;12;0;Create;True;0;0;0;False;0;False;1,1;2,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;74;-10233.49,-1152.254;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;75;-10233.49,-1024.254;Inherit;False;Property;_Noise_01_Scale;Noise_01_Scale;11;0;Create;True;0;0;0;False;0;False;0.8,0.8;2,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-9977.491,-640.2536;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;77;-9593.491,-384.2537;Inherit;False;60;windSpeed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;78;-9849.491,-512.2535;Inherit;False;Property;_Noise_02_Speed;Noise_02_Speed;16;0;Create;True;0;0;0;False;0;False;-0.2,0.4;0.25,0.25;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-9977.491,-1152.254;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;80;-9849.491,-1024.254;Inherit;False;Property;_Noise_01_Speed;Noise_01_Speed;15;0;Create;True;0;0;0;False;0;False;0.5,0.5;0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;81;-4857.497,-2048.254;Inherit;False;Distortion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;82;-9593.491,-896.2538;Inherit;False;60;windSpeed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;83;-9593.491,-640.2536;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;84;-9337.491,-384.2537;Inherit;False;81;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;85;-9593.491,-1152.254;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;86;-9337.491,-896.2538;Inherit;False;81;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;87;-9337.491,-640.2536;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;88;-9337.491,-1152.254;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;89;-9081.491,-640.2536;Inherit;True;Property;_Noise_02_Texture;Noise_02_Texture;3;0;Create;True;0;0;0;False;0;False;-1;None;4c7b88daf72dcdb4d85c35517cee3224;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;90;-6955.494,-1074.254;Inherit;False;980;550;Distortion Mask;6;104;100;99;95;94;92;;0,0,0,1;0;0
Node;AmplifyShaderEditor.SamplerNode;91;-9081.491,-1152.254;Inherit;True;Property;_Noise_01_Texture;Noise_01_Texture;2;0;Create;True;0;0;0;False;0;False;-1;None;5301f8ff7e2edea4c86dfaf8247a1ec5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;92;-6905.494,-1024.254;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0.4;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;-8697.491,-1152.254;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-6905.494,-640.2536;Inherit;False;Property;_DistortionMaskIntensity;Distortion Mask Intensity;25;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;95;-6905.494,-768.2538;Inherit;False;81;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;96;-6955.494,77.74625;Inherit;False;2391;470;Flame Mask;10;124;119;114;113;112;111;107;106;103;98;;0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-8441.492,-1024.254;Inherit;False;Property;_Noises_Power;Noises_Power;23;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;98;-6905.494,127.7462;Inherit;False;Property;_Mask_Scale;Mask_Scale;17;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;99;-6521.494,-1024.254;Inherit;True;Property;_DistortionMaskTexture;Distortion Mask Texture;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-6393.494,-768.2538;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;101;-8185.494,-1024.254;Inherit;False;Property;_Noises_Multiply;Noises_Multiply;21;0;Create;True;0;0;0;False;0;False;5;2.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;102;-8441.492,-1152.254;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;103;-6905.494,383.7461;Inherit;False;Property;_Mask_Offset;Mask_Offset;18;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;-6137.494,-1024.254;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;-8185.494,-1152.254;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;106;-6521.494,127.7462;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0.28,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;107;-6009.494,127.7462;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;108;-7929.494,-1152.254;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;109;-7673.494,-1152.254;Inherit;False;noises;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;110;-5803.494,717.746;Inherit;False;2717;536;Erosion;9;136;133;132;123;121;120;117;116;115;;0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-5369.494,255.7462;Inherit;False;Property;_Mask_Power;Mask_Power;22;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;112;-5753.494,127.7462;Inherit;True;Property;_Mask_Texture;Mask_Texture;6;0;Create;True;0;0;0;False;0;False;-1;None;48dc7bc18268b924c8348d314decc5da;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;113;-5369.494,127.7462;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;114;-4985.497,255.7462;Inherit;False;Property;_Mask_Multiply;Mask_Multiply;20;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;115;-5753.494,895.7463;Inherit;False;Property;_NoisesOpacityBoost;Noises Opacity Boost;26;0;Create;True;0;0;0;False;0;False;1;8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;116;-5753.494,1023.746;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0.4;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;117;-5753.494,767.746;Inherit;False;109;noises;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;118;-3115.493,1741.745;Inherit;False;1170;806;Camera Offset;9;144;139;138;135;130;129;128;126;125;;0,0,0,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;-4985.497,127.7462;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;-5241.494,767.746;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;121;-5241.494,1023.746;Inherit;True;Property;_TextureSample2;Texture Sample 2;7;0;Create;True;0;0;0;False;0;False;-1;None;7eb80d217035c754e91933ce42a82aec;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;122;-2603.493,701.746;Inherit;False;596;694;Depth Fade;4;142;137;131;127;;0,0,0,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;123;-4729.496,767.746;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;124;-4729.496,127.7462;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;125;-3065.493,2431.746;Inherit;False;Property;_CameraOffset;CameraOffset;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;126;-3065.493,2303.746;Inherit;False;3;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;127;-2553.493,1279.746;Inherit;False;Property;_DepthFade;Depth Fade;27;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;128;-3065.493,2047.746;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;129;-3065.493,1791.745;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;130;-2809.493,2301.014;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DepthFade;131;-2553.493,1151.746;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;132;-3577.493,895.7463;Inherit;False;Property;_Dissolve;Dissolve;29;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;133;-4217.496,767.746;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;134;-3883.493,-946.254;Inherit;False;2580;1190;Color;9;153;152;151;150;149;148;146;145;141;;0,0,0,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;135;-2553.493,2303.746;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.01,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;136;-3321.493,767.746;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;137;-2297.493,1151.746;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;138;-2681.493,1791.745;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;139;-2425.493,1791.745;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;140;-1579.491,717.746;Inherit;False;471;185;Particle System Opacity;2;154;143;;0,0,0,1;0;0
Node;AmplifyShaderEditor.VertexColorNode;141;-3321.493,-896.2538;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;142;-2169.493,751.746;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-1529.491,767.746;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;144;-2169.493,1791.745;Inherit;False;CameraOffset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;145;-3833.493,-0.2538238;Inherit;True;Property;_ColorGradientMask;Color Gradient Mask;4;0;Create;True;0;0;0;False;0;False;-1;None;b1f212ebadb408243930c2abe294793c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;146;-3833.493,-512.2535;Inherit;False;Property;_ColorBottom;Color Bottom;8;0;Create;True;0;0;0;False;0;False;1,0.3666667,0,0;1,0.5,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-2937.493,-896.2538;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;149;-2234.69,-135.9528;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;-1529.491,-0.2538238;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;151;-3833.493,-256.2537;Inherit;False;Property;_ColorTop;Color Top;9;0;Create;True;0;0;0;False;0;False;1,0.6666667,0,0;1,0.3333333,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;152;-3449.493,-512.2535;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;154;-1273.491,767.746;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;147;-1792.017,1789.716;Inherit;False;144;CameraOffset;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;153;-1529.491,127.7462;Inherit;False;Property;_EmissiveIntensity;Emissive Intensity;0;0;Create;True;0;0;0;False;0;False;1;123;0;0;0;1;FLOAT;0
WireConnection;0;2;150;0
WireConnection;0;9;154;0
WireConnection;58;0;55;0
WireConnection;58;1;56;0
WireConnection;60;0;58;0
WireConnection;64;0;61;0
WireConnection;64;1;59;0
WireConnection;65;0;64;0
WireConnection;65;2;63;0
WireConnection;65;1;62;0
WireConnection;67;1;65;0
WireConnection;70;0;67;1
WireConnection;70;1;66;0
WireConnection;71;0;70;0
WireConnection;71;1;69;0
WireConnection;76;0;72;0
WireConnection;76;1;73;0
WireConnection;79;0;74;0
WireConnection;79;1;75;0
WireConnection;81;0;71;0
WireConnection;83;0;76;0
WireConnection;83;2;78;0
WireConnection;83;1;77;0
WireConnection;85;0;79;0
WireConnection;85;2;80;0
WireConnection;85;1;82;0
WireConnection;87;0;83;0
WireConnection;87;1;84;0
WireConnection;88;0;85;0
WireConnection;88;1;86;0
WireConnection;89;1;87;0
WireConnection;91;1;88;0
WireConnection;93;0;91;1
WireConnection;93;1;89;1
WireConnection;99;1;92;0
WireConnection;100;0;95;0
WireConnection;100;1;94;0
WireConnection;102;0;93;0
WireConnection;102;1;97;0
WireConnection;104;0;99;1
WireConnection;104;1;100;0
WireConnection;105;0;102;0
WireConnection;105;1;101;0
WireConnection;106;0;98;0
WireConnection;106;1;103;0
WireConnection;107;0;104;0
WireConnection;107;1;106;0
WireConnection;108;0;105;0
WireConnection;109;0;108;0
WireConnection;112;1;107;0
WireConnection;113;0;112;1
WireConnection;113;1;111;0
WireConnection;119;0;113;0
WireConnection;119;1;114;0
WireConnection;120;0;117;0
WireConnection;120;1;115;0
WireConnection;121;1;116;0
WireConnection;123;0;120;0
WireConnection;123;1;121;1
WireConnection;124;0;119;0
WireConnection;130;0;126;0
WireConnection;130;1;125;0
WireConnection;131;0;127;0
WireConnection;133;0;124;0
WireConnection;133;1;123;0
WireConnection;135;0;130;0
WireConnection;136;0;133;0
WireConnection;136;1;132;0
WireConnection;137;0;131;0
WireConnection;138;0;129;0
WireConnection;138;1;128;0
WireConnection;139;0;138;0
WireConnection;139;1;135;0
WireConnection;142;0;136;0
WireConnection;142;1;137;0
WireConnection;143;0;142;0
WireConnection;143;1;141;4
WireConnection;144;0;139;0
WireConnection;148;0;141;0
WireConnection;148;1;152;0
WireConnection;149;0;148;0
WireConnection;149;1;133;0
WireConnection;150;0;149;0
WireConnection;150;1;153;0
WireConnection;152;0;146;0
WireConnection;152;1;151;0
WireConnection;152;2;145;1
WireConnection;154;0;143;0
ASEEND*/
//CHKSM=B335322E2305A701FE7733BF48F1A3A79A8B4646