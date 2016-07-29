using UnityEngine;
using UnityMIDI;
using System.Collections;

public class UnityMIDIPreferences
{
    [SerializeField]
    private static Texture2D m_midiColourTexture, m_octaveColourTexture;

    public static Texture2D midiColourTexture { get { return m_midiColourTexture =  m_midiColourTexture != null ? m_midiColourTexture : Resources.Load<Texture2D>("/MIDIColourFull.png"); } }
    public static Texture2D octaveColourTexture { get { return m_octaveColourTexture = m_octaveColourTexture != null ? m_octaveColourTexture : Resources.Load<Texture2D>("/OctaveColours.png"); } }

    //Constants
    static Color kC = new Color(1, 0, 0),
    kDb = new Color(1, 88f / 255, 10f / 255),
    kD = new Color(1, 178f / 255, 20f / 255),
    kEb = new Color(247f / 255, 203f / 255, 10f / 255),
    kE = new Color(239f / 255, 230f / 255, 0),
    kF = new Color(255f / 255, 176f / 255, 20 / 255),
    kGb = new Color(229f / 255, 159f / 255f, 63 / 255),
    kG = new Color(48f / 255f, 77 / 255f, 206 / 255),
    kAb = new Color(184f / 255, 0, 229f / 255),
    kA = new Color(128f / 255, 0, 242f / 255),
    kBb = new Color(72f / 255, 0, 1),
    kB = new Color(1, 0, 203f / 255);


    private static Color colour01 = kC,
    colour02 = kDb,
    colour03 = kD,
    colour04 = kEb,
    colour05 = kE,
    colour06 = kF,
    colour07 = kGb,
    colour08 = kG,
    colour09 = kAb,
    colour10 = kA,
    colour11 = kBb,
    colour12 = kB;

    public static Color GetColor(Tone nc)
    {
		switch(nc){
		case Tone.C:
			return colour01;
		case Tone.Db:
			return colour02;
		case Tone.D:
			return colour03;
		case Tone.Eb:
			return colour04;
		case Tone.E:
			return colour05;
		case Tone.F:
			return colour06;
		case Tone.Gb:
			return colour07;
		case Tone.G:
			return colour08;
		case Tone.Ab:
			return colour09;
		case Tone.A:
			return colour10;
		case Tone.Bb:
			return colour11;
		case Tone.B:
			return colour12;
		default:
			return Color.black;
		}
	}

	public static Color GetColor(int i)
    {
		switch (i % 12) {
		case 0:
			return colour01;
		case 1:
			return colour02;
		case 2:
			return colour03;
		case 3:
			return colour04;
		case 4:
			return colour05;
		case 5:
			return colour06;
		case 6:
			return colour07;
		case 7:
			return colour08;
		case 8:
			return colour09;
		case 9:
			return colour10;
		case 10:
			return colour11;
		case 11:
			return colour12;
		default:
			return Color.black;
		}
	}

    public static Accidental GetAccidental() { return (Accidental)PlayerPrefs.GetInt("AccidentalPreference", 0); ; }

    static void SetShaderColours()
    {
        Shader.SetGlobalColor("_Color_C", colour01);
        Shader.SetGlobalColor("_Color_Db", colour02);
        Shader.SetGlobalColor("_Color_D", colour03);
        Shader.SetGlobalColor("_Color_Eb", colour04);
        Shader.SetGlobalColor("_Color_E", colour05);
        Shader.SetGlobalColor("_Color_F", colour06);
        Shader.SetGlobalColor("_Color_Fb", colour07);
        Shader.SetGlobalColor("_Color_G", colour08);
        Shader.SetGlobalColor("_Color_Ab", colour09);
        Shader.SetGlobalColor("_Color_A", colour10);
        Shader.SetGlobalColor("_Color_Bb", colour11);
        Shader.SetGlobalColor("_Color_B", colour12);
    }

    static void SetShaderTextures()
    {
        Shader.SetGlobalTexture("_MIDITex", m_midiColourTexture);
        Shader.SetGlobalTexture("_OctaveTex", m_octaveColourTexture);
    }

    public static void SetShaderVariables()
    {
        SetShaderColours();
        SetShaderTextures();
    }
}
