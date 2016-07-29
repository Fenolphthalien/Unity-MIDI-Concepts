Shader "MIDI/UnlitVertexColour"
 {
	Properties 
	{
		_Tint("Tint", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Pass
		{
			Lighting Off
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Assets/UnityMIDIShaderVariables.cginc"
			
			#pragma target 3.0
		
			uniform fixed4 _Tint;

			 // vertex input: position, color
			struct appdata 
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};

			struct V2F
			{
				float4 pos : SV_POSITION;
				float4 colour : COLOR;
			};

			V2F vert(appdata v)
			{
				V2F o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.colour = v.color * _Tint;
				return o;
			}

			fixed4 frag (V2F v) : SV_Target
			{
				return v.colour;
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
}
