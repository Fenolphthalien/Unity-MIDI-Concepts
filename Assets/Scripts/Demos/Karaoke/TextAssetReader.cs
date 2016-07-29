using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextAssetReader 
{
    public readonly TextAsset textAsset;

    int index = 0, characters;
    string text;

    public TextAssetReader(TextAsset tAsset)
    {
        textAsset = tAsset;
        characters = tAsset.text.Length - 1;
        text = tAsset.text;
    }

    public void Reset()
    {
        index = 0;
    }

    public void ReadLine(out string read, out bool finished)
    {
        if(index > characters)
        {
            read = null;
            finished = true;
            return;
        }
        string buffer = null;
        char currentChar = text[index];
        while(currentChar != '\n' && characters >= index) //Break on new line
        {
            buffer += currentChar;
            index++;
            if (index > characters)
                break;
            currentChar = text[index];
        }
        index++; //Move past the new line character;
        finished = index > characters;
        read = buffer;
    }

    public void ReadLine(char ignore,out string read,out string fullRead, out bool finished)
    {
        if (index > characters)
        {
            read = null;
            fullRead = null;
            finished = true;
            return;
        }
        string buffer = null, fullBuffer = null;
        char currentChar = text[index];
        while (currentChar != '\n' && characters >= index) //Break on new line
        {
            if(currentChar != ignore)
                buffer += currentChar;
            fullBuffer += currentChar;
            index++;
            if (index > characters)
                break;
            currentChar = text[index];
        }
        index++; //Move past the new line character;
        finished = index > characters;
        read = buffer;
        fullRead = fullBuffer;
    }

    public void NumberOfWordsInTextWithSpaces(ref List<int> wordCharacterCount, ref int words)
    {
        if(wordCharacterCount != null)
            wordCharacterCount.Clear();

        wordCharacterCount = new List<int>();
        int chars = 0;
        char c;
        bool space = false, lastSpace;
        
        for(int i = 0; i < text.Length; i++)
        {
            lastSpace = space;
            c = text[i];
            space = c == ' ' || c == '\n';
            if (!space && lastSpace)
            {
                Debug.Log(string.Format("Starting from: {0} after {1} characters.", text[i], chars));
                wordCharacterCount.Add(chars);
                chars = 0;
                i--;
                continue;
            }
            chars++;
        }
        words = wordCharacterCount.Count;
    }
}
