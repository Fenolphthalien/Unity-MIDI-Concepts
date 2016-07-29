
#ifndef UNITY_MIDI_SHADER_VARS
#define UNITY_MIDI_SHADER_VARS

//Uniform colors for notes - Set in preferences
uniform fixed4 _Color_C;
uniform fixed4 _Color_Db;
uniform fixed4 _Color_D;
uniform fixed4 _Color_Eb;
uniform fixed4 _Color_E;
uniform fixed4 _Color_F;
uniform fixed4 _Color_Gb;
uniform fixed4 _Color_G;
uniform fixed4 _Color_Ab;
uniform fixed4 _Color_A;
uniform fixed4 _Color_Bb;
uniform fixed4 _Color_B;

//Pulse, X = amount, Y = time, Z = Null, W = Null.
uniform fixed4 _Pulse;

//Tempo, X = BPM, Y = BPS;
uniform fixed2 _Tempo;

//Set In preferences
uniform sampler2D _OctaveTex;
uniform sampler2D _MIDITex;

//Set by developer.
uniform sampler2D _OctaveMask;
uniform sampler2D _MIDIMask;

//Functions
float4 SampleOctaveMap (float2 uv, bool useMask)
{
	float4 map = tex2D(_OctaveTex, uv);
	float4 mask = tex2D(_OctaveMask,uv);
	float4 col = useMask ? map * mask : map; 
	return col;
} 

float4 SampleMIDIMap (float2 uv, bool useMask)
{
	float4 map = tex2D(_MIDITex, uv);
	float4 mask = tex2D(_MIDIMask,uv);
	float4 col = useMask ? map * mask : map; 
	return col;
} 

#endif