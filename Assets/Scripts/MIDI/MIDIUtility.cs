using UnityEngine;
using System.Collections;
using System.IO;

namespace UnityMIDI
{
    public static class MIDITextureMasks
    {
        static Texture2D m_globalMIDIMask;
        static Texture2D globalMIDIMask { get { return m_globalMIDIMask; } set { Shader.SetGlobalTexture(globalMIDIMaskPath, value); m_globalMIDIMask = value; } }
       
        static Texture2D m_globalOctaveMask;
        static Texture2D globalOctaveMask { get { return m_globalOctaveMask; } set { Shader.SetGlobalTexture(globalOctaveMaskPath, value); m_globalOctaveMask = value; } }

        public static Texture2D midiMask{get{return m_globalMIDIMask;}}
        public static Texture2D octaveMask { get { return m_globalOctaveMask; } }

        public const string globalMIDIMaskPath = "_MIDIMask", globalOctaveMaskPath = "_OctaveMask";

        static MIDITextureMasks()
        {
            m_globalOctaveMask = null;
            m_globalMIDIMask = null;
        }

        public static Texture2D GenerateFullMIDIMask(bool makeGlobal = false)
        {
            Texture2D tex2d = new Texture2D(16, 16, TextureFormat.RGB24, false);
            tex2d.hideFlags = HideFlags.HideAndDontSave;
            tex2d.filterMode = FilterMode.Point;

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    tex2d.SetPixel(x, y, Color.black);
                }
            }
            tex2d.Apply();
            if (makeGlobal)
               globalMIDIMask = tex2d;
            return tex2d;
        }

        public static Texture2D GenerateOctaveMask(bool makeGlobal = false)
        {
            Texture2D tex2d = new Texture2D(4, 4, TextureFormat.RGB24, false);
            tex2d.hideFlags = HideFlags.HideAndDontSave;
            tex2d.filterMode = FilterMode.Point;

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    tex2d.SetPixel(x, y, Color.black);
                }
            }
            tex2d.Apply();
            if (makeGlobal)
                globalOctaveMask = tex2d;
            return tex2d;
        }

        public static void GetGlobalMasks(out Texture2D midiMask, out Texture2D octaveMask)
        {
            midiMask = m_globalMIDIMask;
            octaveMask = m_globalOctaveMask;
        }

        public static void GetMIDIMaskPixel(Tone note, int octave, out int x, out int y)
        {
            x = (int)note;
            y = (octave >= 9 ? 8 : octave) < 0 ? 0 : octave;
        }

        public static void GetMIDIMaskPixel(KeyEvent keyEvent, out int x, out int y)
        {
            x = (int)keyEvent.note;
            y = (keyEvent.octave >= 9 ? 8 : keyEvent.octave) < 0 ? 0 : keyEvent.octave;
        }

        public static void GetMIDIMaskPixel(MIDIMessage message, out int x, out int y)
        {
            x = (int)message.GetNote();
            y = (message.GetOctave() >= 9 ? 8 : message.GetOctave()) < 0 ? 0 : message.GetOctave();
        }

        public static void GetOctaveMaskPixel(Tone note, out int x, out int y)
        { 
            x = (int)note % 4;
            y = (int)note / 4;
        }

        public static void GetOctaveMaskPixel(KeyEvent keyEvent, out int x, out int y)
        {
            x = (int)keyEvent.note % 4;
            y = (int)keyEvent.note / 4;
        }

        public static void NullMIDITexture(Texture2D tex2d = null)
        { 
            if(tex2d != null && m_globalMIDIMask == tex2d)
            {
               globalMIDIMask = null;
               return;
            }
           globalMIDIMask = null;
        }

        public static void NullOctaveTexture(Texture2D tex2d = null)
        {
            if (tex2d != null && globalOctaveMask == tex2d)
            {
                globalOctaveMask = null;
                return;
            }
            globalOctaveMask = null;
        }

        public static void LogMIDITexture(string path = null, Texture2D tex2d = null)
        { 
            if(tex2d == null && globalMIDIMask != null)
            {
                LogTexture("GlobalMIDIMask", m_globalMIDIMask, path);
                return;
            }
            else if(tex2d != null)
            {
                LogTexture("GlobalMIDIMask", tex2d, path);
                return;
            }
        }
        
        public static void LogOctaveTexture(string path = null, Texture2D tex2d = null)
        {
            if (tex2d == null && globalOctaveMask != null)
            {
                LogTexture("GlobalOctaveMask", m_globalOctaveMask, path);
                return;
            }
            else if (tex2d != null)
            {
                LogTexture("GlobalOctaveMask", tex2d, path);
                return;
            }
        }

        public static void LogTexture(string name, Texture2D tex2d, string path = null)
        {
            byte[] png = tex2d.EncodeToPNG();
            string truePath = path != null ? path : Application.persistentDataPath +"/Log/";
            
            /*Check if destination exists.*/
            if(!System.IO.Directory.Exists(truePath.TrimEnd(new char[]{'/'})))
                System.IO.Directory.CreateDirectory(truePath.TrimEnd(new char[]{'/'}));

            using (BinaryWriter bw = new BinaryWriter(File.Create(truePath + name + DateString() + ".png")))
            {
                bw.Write(png);
            }
        }

        public static void LogTexture(string name, Texture2D tex2d, ref int logCount, string path = null)
        {
            byte[] png = tex2d.EncodeToPNG();
            string truePath = path != null ? path : Application.persistentDataPath + "/Log/";

            logCount++;

            /*Check if destination exists.*/
            if (!System.IO.Directory.Exists(truePath.TrimEnd(new char[] { '/' })))
                System.IO.Directory.CreateDirectory(truePath.TrimEnd(new char[] { '/' }));

            using (BinaryWriter bw = new BinaryWriter(File.Create(truePath + name + DateString() + "_" + logCount.ToString() + ".png")))
            {
                bw.Write(png);
            }
        }

        static string DateString()
        {
            return "_"+System.DateTime.Now.Ticks.ToString();
        }
    }
}
