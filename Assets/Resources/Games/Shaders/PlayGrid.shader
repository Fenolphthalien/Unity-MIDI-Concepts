Shader "Prototyping/ScrollingMaskedColor" {
	Properties {
		_Color ("Color", Color) = (0,0,0,1)
		_Color1 ("Texture Color", Color) = (1,1,1,1)
		_MainTex ("Scrolling Texture(A)", 2D) = "white" {}
		_Scroll ("Scroll Direction (Normalised XY)", Vector) = (0.5,0.5,1,1)
		_Speed ("Scroll Speed", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass { Lighting Off}
		
		CGPROGRAM
		#pragma surface surf Standard nolightmap novertexlights noforwardadd
		
		#include "Assets/UnityMIDIShaderVariables.cginc"
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		uniform sampler2D _MainTex;

		struct Input 
		{
			float2 uv_MainTex;
		};

		uniform fixed4 _Color;
		uniform fixed4 _Color1;
		uniform fixed4 _Scroll;
		uniform fixed _Speed;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 cTex = tex2D (_MainTex, IN.uv_MainTex + (_Time.yy * _Scroll.xy * _Speed + _Pulse.xx * _Scroll.xy));
			//fixed4 cTex = tex2D (_MainTex, IN.uv_MainTex + ( _Pulse.xx));
			o.Albedo = lerp(_Color,_Color1,cTex.a);
			o.Alpha = 1;
		}
		ENDCG
	} 
	CustomEditor "ScrollingMaskedTextureEditor"
}
