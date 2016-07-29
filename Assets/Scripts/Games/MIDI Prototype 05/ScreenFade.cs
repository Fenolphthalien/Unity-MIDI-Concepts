using UnityEngine;
using System.Collections;

namespace PrototypeFive
{
    public class ScreenFade : MonoBehaviour
    {
        float fade = 0, maxAlpha, deltaMultiplier;
        Texture2D fadeTex;
        bool fadingOut;

        // Update is called once per frame
        void Update()
        {
            if (fadingOut)
            {
                fade -= Time.deltaTime * deltaMultiplier;
                fade = fade <= 0 ? 0 : fade;
            }
            else
            {
                fade += Time.deltaTime * deltaMultiplier;
                fade = fade > maxAlpha ? maxAlpha : fade;
            }
        }

        public void BeginFadeOut()
        {
            fadingOut = true;
        }

        void InitTexture(Color C)
        {
            if (fadeTex == null)
                fadeTex = new Texture2D(1, 1);
            fadeTex.SetPixel(1, 1, C);
            fadeTex.Apply();
        }

        void OnGUI()
        {
            Color c = Color.white * new Color(1, 1, 1, fade);
            GUI.color = c;
            Rect screen = new Rect(0, 0, Screen.width, Screen.height);
            GUI.DrawTexture(screen, fadeTex);
        }

        public void Clear()
        {
            Destroy(this.gameObject);
        }

        public static ScreenFade BeginFade(ScreenFade screenFade, float _maxAlpha, float overSeconds)
        {
            if (screenFade == null)
            {
                GameObject go = new GameObject("ScreenFade");
                screenFade = go.AddComponent<ScreenFade>();
            }

            screenFade.maxAlpha = _maxAlpha;
            screenFade.deltaMultiplier = _maxAlpha / overSeconds;
            screenFade.fadingOut = false;
            screenFade.InitTexture(Color.black);
           
            return screenFade;
        }
    }
}