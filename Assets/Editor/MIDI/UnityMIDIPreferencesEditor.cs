using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityMIDI;
using System.Collections;
using System.IO;

public class UnityMIDIPreferencesEditor
{
	
	private static bool prefsLoaded = false;

	private static string sourceFolderPath;
	public static string SourceFolderPath
    {
		get
        {
			return sourceFolderPath;
		}
	}

	//Constants
	static Color kC = new Color(1,0,0),
	kDb = new Color(1,88f / 255,10f / 255),
	kD = new Color(1,178f / 255,20f / 255),
	kEb = new Color(247f / 255,203f / 255,10f / 255),
	kE = new Color(239f / 255,230f / 255,0),
	kF = new Color(255f / 255,176f / 255,20 / 255),
	kGb = new Color(229f / 255,159f / 255f,63 / 255),
	kG = new Color(48f / 255f,77 / 255f,206 / 255),
	kAb = new Color(184f / 255,0,229f / 255),
	kA  = new Color(128f / 255,0,242f / 255),
	kBb = new Color(72f / 255,0,1),
	kB  = new Color(1,0,203f / 255);

	// The Preferences
	private static Accidental displayAccidental;

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
    
    [SerializeField]
    private static Texture2D m_midiColourTexture, m_octaveColourTexture;
   
    public static Texture2D midiColourTexture { get { return m_midiColourTexture != null ? m_midiColourTexture : AssetDatabase.LoadAssetAtPath<Texture2D>("/Resources/MIDIColourFull.png"); } }
    public static Texture2D octaveColourTexture { get { return m_octaveColourTexture != null ? m_octaveColourTexture : AssetDatabase.LoadAssetAtPath<Texture2D>("/Resources/OctaveColours.png"); } }

	[PreferenceItem("MIDI")]
	private static void CustomPreferencesGUI()
    {
		if (!prefsLoaded)
        {

            displayAccidental = (Accidental)PlayerPrefs.GetInt("AccidentalPreference", 0);
            sourceFolderPath = PlayerPrefs.GetString("SourceStringPref", "");
            SetShaderColours();
			prefsLoaded = true;
		}
		
		EditorGUILayout.LabelField("Version: IN DEVELOPMENT");
        EditorGUILayout.Separator();

		displayAccidental = (Accidental)EditorGUILayout.EnumPopup ("Display accidentals as:", displayAccidental);
		sourceFolderPath = EditorGUILayout.TextField ("Path to source MIDIs:", sourceFolderPath);

        EditorGUILayout.Separator();
        #region MIDIColours

        GUILayout.BeginHorizontal();
        GUILayout.Label("C:");
        GUILayout.Label(Tone.Db.NoteToString() + ":");
        GUILayout.Label("D:");
        GUILayout.Label(Tone.Eb.NoteToString() + ":");
        GUILayout.Label("E:");
        GUILayout.Label("F:");
        GUILayout.EndHorizontal();
       
        GUILayout.BeginHorizontal();
		colour01 = EditorGUILayout.ColorField (colour01);
		colour02 = EditorGUILayout.ColorField (colour02);
		colour03 = EditorGUILayout.ColorField (colour03);
		colour04 = EditorGUILayout.ColorField (colour04);
		colour05 = EditorGUILayout.ColorField (colour05);
		colour06 = EditorGUILayout.ColorField (colour06);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label(Tone.Gb.NoteToString() + ":");
        GUILayout.Label("G:");
        GUILayout.Label(Tone.Ab.NoteToString() + ":");
        GUILayout.Label("A:");
        GUILayout.Label(Tone.Bb.NoteToString() + ":");
        GUILayout.Label("B:");
        GUILayout.EndHorizontal();
       
        GUILayout.BeginHorizontal();
        colour07 = EditorGUILayout.ColorField(colour07);
		colour08 = EditorGUILayout.ColorField (colour08);
		colour09 = EditorGUILayout.ColorField (colour09);
		colour10 = EditorGUILayout.ColorField (colour10);
		colour11 = EditorGUILayout.ColorField (colour11);
		colour12 = EditorGUILayout.ColorField (colour12);
        GUILayout.EndHorizontal();

        SetShaderColours();
        #endregion
        EditorGUILayout.Separator();
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset Colours", new GUILayoutOption[] {GUILayout.Width(90)}))
        {
            ResetColours();
        }
        if (GUILayout.Button("Save MIDI Texture", new GUILayoutOption[] { GUILayout.Width(122) }))
        {
            GenerateFullMIDITexture();
        }
        if (GUILayout.Button("Save Octave Texture", new GUILayoutOption[] { GUILayout.Width(134) }))
        {
            GenerateOctaveTexture();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("MIDI Texture");
        GUILayout.Label("Octave Texture");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        m_midiColourTexture = (Texture2D)EditorGUILayout.ObjectField(m_midiColourTexture, typeof(Texture2D),null);
        m_octaveColourTexture = (Texture2D)EditorGUILayout.ObjectField(m_octaveColourTexture, typeof(Texture2D), null);
        SetShaderTextures();
        GUILayout.EndHorizontal();

		if (GUI.changed)
        {
			PlayerPrefs.SetInt("AccidentalPreference", (int)displayAccidental);
            PlayerPrefs.SetString("SourceStringPref", sourceFolderPath);
            PlayerPrefs.Save();
		}
	}

	public static Color GetColor(Tone nc){
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

	public static Color GetColor(int i){
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

	public static Accidental GetAccidental(){return displayAccidental;}

	static void ResetColours()
    {
		colour01 = kC;
		colour02 = kDb;
		colour03 = kD;
		colour04 = kEb;
		colour05 = kE;
		colour06 = kF; 
		colour07 = kGb;
		colour08 = kG;
		colour09 = kAb;
		colour10 = kA;
		colour11 = kBb;
		colour12 = kB;
        SetShaderColours();
	}

    static void GenerateFullMIDITexture()
    {
        Texture2D tex2d = new Texture2D(16, 16, TextureFormat.RGB24,false);
        tex2d.hideFlags = HideFlags.HideAndDontSave;
        tex2d.filterMode = FilterMode.Point;

        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 12; x++)
            {
                if (y * 12 + x < 108) //88 Piano(A0 to C8) keys plus C0 to Ab0(9) play Db8 to B8(11) = 88 + 9(97) + 11(108)
                {
                    tex2d.SetPixel(x, y, GetColor(x).TintByOctave(y));
                    continue;
                }
                break;
            }
        }

        tex2d.Apply();

        if (m_midiColourTexture == null)
        {
            byte[] png = tex2d.EncodeToPNG();
            Object.DestroyImmediate(tex2d);
            using (BinaryWriter bw = new BinaryWriter(File.Open(Application.dataPath + "/Resources/MIDIColoursFull.png", FileMode.Create)))
            {
                bw.Write(png);
            }
            m_midiColourTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Resources/MIDIColoursFull.png");
            if(m_midiColourTexture != null)
             m_midiColourTexture.filterMode = FilterMode.Point;
        }
        else
        {
            byte[] png = tex2d.EncodeToPNG();
            string path = AssetDatabase.GetAssetPath(m_midiColourTexture);
            Object.DestroyImmediate(tex2d);
            using (BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                bw.Write(png);
            }
        }
        AssetDatabase.SaveAssets();
        m_midiColourTexture = tex2d;
        SetShaderTextures();
    }
    
    static void GenerateOctaveTexture()
    {
        Texture2D tex2d = new Texture2D(4, 4, TextureFormat.RGB24, false);
        tex2d.hideFlags = HideFlags.HideAndDontSave;
        tex2d.filterMode = FilterMode.Point;

        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (y * 4 + x < 12)
                {
                    tex2d.SetPixel(x, y, GetColor(y * 4 + x));
                    continue;
                }
                break;
            }
        }

        tex2d.Apply();

        if (m_octaveColourTexture == null)
        {
            byte[] png = tex2d.EncodeToPNG();
            Object.DestroyImmediate(tex2d);
            using (BinaryWriter bw = new BinaryWriter(File.Open(Application.dataPath + "/Resources/OctaveColours.png", FileMode.Create)))
            {
                bw.Write(png);
            }
            m_octaveColourTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("/Resources/OctaveColours.png");
            if(m_octaveColourTexture != null)
             m_octaveColourTexture.filterMode = FilterMode.Point;
        }
        else
        {
            byte[] png = tex2d.EncodeToPNG();
            string path = AssetDatabase.GetAssetPath(m_octaveColourTexture);
            Object.DestroyImmediate(tex2d);
            using (BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                bw.Write(png);
            }
        }
        AssetDatabase.SaveAssets();
        m_octaveColourTexture = tex2d;
        SetShaderTextures();
    }

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
