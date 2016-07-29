using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class KaraokeExtensions
{
    public static int CharactersUntilEscape(this string str, char escape, ref int offset)
    {
        int chars = 0;
        char current;
        if (str == null || offset >= str.Length)
        {
            return chars;
        }
        do
        {
            current = str[offset];
            if (current != escape)
            {
                chars++;
            }
            offset++; //Move past escape character when passed, otherwise increment for next loop.
        } while (current != escape && offset < str.Length);
        return chars;
    }
}
