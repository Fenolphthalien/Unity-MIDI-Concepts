using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TextMasker : MonoBehaviour
{
    public RectTransform mask;
    public Text maskText;

    public int maskFromCharacter = 0;

    public void Reset()
    {
        maskFromCharacter = 0;
    }

	void LateUpdate () 
    {
        CalucalateMaskWidth();
	}

    void CalucalateMaskWidth()
    {
        maskFromCharacter = maskFromCharacter > 0 ? maskFromCharacter : 0;
        if (mask == null || maskText == null)
            return;
       
        //Run the text generation process to get the correct width for the text.
        TextGenerator textGenerator = maskText.cachedTextGenerator;

        List<UICharInfo> characterInfo = new List<UICharInfo>();
        textGenerator.GetCharacters(characterInfo);

        float width = Mathf.Abs(GetCharacterCursorX(characterInfo, maskFromCharacter));
        mask.sizeDelta = new Vector2(width, 100);
    }

    public void IncrementMask()
    {
        maskFromCharacter++;
    }

    public void IncrementMask(int by)
    {
        maskFromCharacter += by;
    }

    public void DecrementMask()
    {
        maskFromCharacter--;
    }

    public void DecrementMask(int by)
    {
        maskFromCharacter -= by;
    }

    public void MaskFrom(int from)
    {
        maskFromCharacter = from;
    }

    float GetCharacterCursorX(List<UICharInfo> charInfo, int character)
    {
        if (charInfo == null || charInfo.Count == 0)
            return 0;
        int index = (character >= 0 ? character : 0) < charInfo.Count ? character : charInfo.Count-1;
        return charInfo[index].cursorPos.x;
    }

    Vector2 GetCharacterCursor(List<UICharInfo> charInfo, int character)
    {
        if (charInfo == null || charInfo.Count == 0)
            return Vector2.zero;
        int index = (character >= 0 ? character : 0) < charInfo.Count ? character : charInfo.Count-1;
        return charInfo[index].cursorPos;
    }
}
