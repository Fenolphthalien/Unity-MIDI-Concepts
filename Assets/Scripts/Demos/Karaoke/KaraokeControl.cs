using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityMIDI;

public class KaraokeControl : MonoBehaviour, INoteOnHandler, INoteOffHandler
{
    public TextAsset lyrics;
    public CanvasGroup lyricsGroup = null;

    TextAssetReader lyricReader;

    const char escapeChar = '|';

    int charsRead = 0, lineChars, iterator = 0;

    string lineWithEscapes;

    public TextMasker textMasker;
    public Text underText, overText;

    public bool startHidden;

    const int highlightNext = (int)Tone.C, toggleLyrics = (int)Tone.D, toggleCredits = (int)Tone.E;

    void Awake()
    {
        lyricReader = new TextAssetReader(lyrics);
        ReadNewLine();
        DisplayLyrics(!startHidden);
    }

    void ReadNewLine()
    {
        string str, fStr;
        bool fin;
        lyricReader.ReadLine(escapeChar,out str, out fStr, out fin);
        if (str == null)
            return;
        lineChars = str.Length;
        lineWithEscapes = fStr;
        underText.text = str;
        overText.text = str;
        charsRead = 0;
        iterator = 0;
        textMasker.MaskFrom(0);
    }

    void ReadChar()
    {
        textMasker.IncrementMask();
        charsRead++;
    }

    void ReadChars(int read)
    {
        textMasker.IncrementMask(read);
        charsRead += read;
    }

    void NewLineCheck()
    {
        if (charsRead >= lineChars)
        {
            ReadNewLine();
            Debug.Log("Line read");
        }
    }

    void DisplayLyrics(bool b)
    {
        if (lyricsGroup != null)
            lyricsGroup.alpha = b ? 1 : 0;
    }

    public void OnNoteOn(MIDIMessage midiMessage)
    {
        if (midiMessage.GetNote() == highlightNext)
        {
           // ReadChars(midiMessage.GetVelocity());
            NewLineCheck();
            int read = lineWithEscapes.CharactersUntilEscape(escapeChar, ref iterator);
            ReadChars(read);
        }
        else if ((int)midiMessage.GetNote() == toggleLyrics)
        {
            DisplayLyrics(true);
        }
    }

    public void OnNoteOff(MIDIMessage midiMessage)
    {
        if ((int)midiMessage.GetNote() == toggleLyrics)
        {
            DisplayLyrics(false);
        }
    }
}
