Shader "nOSu/UnlitVertexPulse"
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_ModelPulse ("Pulse", float) = 1
		_MainTex ("Texture (RGBA)", 2D) = "white" {}
	}
	SubShader 
	{
		Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
		LOD 200
		Pass
		{
			Lighting Off
			
			/** https://en.wikibooks.org/wiki/Cg_Programming/Unity/Transparency **/
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Assets/UnityMIDIShaderVariables.cginc"
			
			#pragma target 3.0

			uniform sampler2D _MainTex;
			uniform fixed4 _Color;
			uniform fixed _ModelPulse;
			
			struct V2F 
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			V2F vert(appdata_base v)
			{
				V2F o;
				float4 vpulse = v.vertex * (_ModelPulse * _Pulse.x + 1);
				o.pos = mul(UNITY_MATRIX_MVP, float4(vpulse.x,v.vertex.y,vpulse.z,v.vertex.w));
				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag (V2F v) : SV_Target
			{
				fixed4 c = tex2D (_MainTex, v.uv) * _Color;
				return c;
			}
			
			ENDCG
		} 
	}
}
