using UnityEngine;
using System.Collections;

public class ManipulateTimeScale : MonoBehaviour 
{
    float timeScale = 1;

    const int maxScale = 8;

    public bool muteWhenSpedUp;

    public AudioSource[] linkedAudioSources;

    void Update()
    {
        if (Input.GetKey(KeyCode.RightBracket))
        {
            timeScale *= 2;
            timeScale = timeScale < maxScale ? timeScale : maxScale;
            Time.timeScale = timeScale;
            SetPitch();
        }
        else if(Input.GetKey(KeyCode.LeftBracket))
        {
            timeScale *= 0.5f;
            timeScale = timeScale > 1 ? timeScale : 1;
            Time.timeScale = timeScale;
            SetPitch();
        }
        else if(Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            timeScale = 1;
            Time.timeScale = timeScale;
            SetPitch();
        }
    }

   void SetPitch()
   {
       if(linkedAudioSources == null || linkedAudioSources.Length == 0)
        return;
       foreach (AudioSource audio in linkedAudioSources)
       {
           audio.pitch = timeScale;
           audio.volume = muteWhenSpedUp && timeScale != 1 ? 0 : 1;
       }
   }
}
