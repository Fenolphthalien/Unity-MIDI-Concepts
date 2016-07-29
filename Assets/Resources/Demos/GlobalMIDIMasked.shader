Shader "MIDI/GlobalMIDIMasked" 
{
	Properties 
	{
		_MainTex ("Diffuse(RGB)", 2D) = "white" {}
		_MIDIColours ("MIDI Colours(RGB)", 2D) = "white" {}
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
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Assets/UnityMIDIShaderVariables.cginc"

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0
			
			uniform sampler2D _MainTex;
			uniform sampler2D _MIDIColours;

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
				return tex2D(_MIDIColours, v.uv1) * tex2D(_MainTex,v.uv0) * tex2D(_MIDIMask,v.uv1);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
