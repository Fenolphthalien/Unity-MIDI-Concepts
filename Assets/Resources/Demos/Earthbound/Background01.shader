Shader "Earthbound/Background01" 
{
	Properties 
	{
		_MainTex ("Main Ramp(RGB)", 2D) = "white" {}
		_KickRamp ("Kick Ramp (RGB)", 2D) = "black" {}
		_SnareRamp ("Snare Ramp (RGB)", 2D) = "black" {}
		_TimeScale ("Time Scale",float) = 1
		_Tint ("Tint", Color) = (1,1,1,1)
	}

	SubShader 
	{
		Tags 
		{ 
			"RenderType"="Opaque" 
		}
		LOD 200
		Pass
		{
			Lighting Off
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Assets/UnityMIDIShaderVariables.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _KickRamp;
			uniform sampler2D _SnareRamp;
			uniform fixed _TimeScale;
			uniform fixed4 _Tint;

			 // vertex input: position, vertex Colour, first UV, second UV
			struct appdata 
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float4 texcoord0 : TEXCOORD0;
				float texcoord1 : TEXCOORD1;
			};

			//Vertex to Fragment shader: position, vertex colour, first UV, second UV
			struct V2F 
			{
				float4 pos : SV_POSITION;
				float4 uv0 : TEXCOORD0;
				float4 uv1: TEXCOORD1;
				fixed4 colour : COLOR;
			};

			V2F vert(appdata v)
			{
				V2F o;
				o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
				o.uv0 = v.texcoord0;
				o.uv1 = v.texcoord1;
				o.colour = v.color;
				return o;
			}

			fixed4 frag (V2F v) : SV_Target
			{
				fixed4 c = tex2D (_MainTex, v.uv1 + _Time.x * _TimeScale) * _Tint * v.colour;
				return c + tex2D(_KickRamp,v.uv1) + tex2D(_SnareRamp,v.uv1);
			}

			ENDCG
		}
	} 
}