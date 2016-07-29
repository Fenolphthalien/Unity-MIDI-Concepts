using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace UnityMIDI
{
    [RequireComponent(typeof(AudioSource))]
    public class LeadIn : MonoBehaviour
    {
        AudioClip sound;
        float length;
        TimeSignature timeSig;
        UnityAction[] OnFinish;

        int bpm, beats;
        float bps, beatSeconds,m_elapsed, totalElapsed;

        public float elapsed { get { return m_elapsed; } }

        float barLength { get { return beatSeconds * timeSig.beatsPerBar; } }

        void SetTempo(int _bpm)
        {
            bpm = _bpm;
            bps = _bpm / 60;
            beatSeconds = 1f / bps;
        }

        void CalculateStartingElapsed()
        {
            m_elapsed = barLength - length;
            Debug.Log(m_elapsed);
            length += -m_elapsed;
        }

        void Update()
        {
            m_elapsed += Time.deltaTime;
            totalElapsed += Time.deltaTime;
            if (m_elapsed < 0)
                return;
            if (m_elapsed >= beatSeconds)
            {
                Tick();
            }
        }

        void OnDestroy()
        {
            if (OnFinish != null)
            {
                for(int i =0; i < OnFinish.Length; i++)
                    OnFinish[i] = null;
            }
            OnFinish = null;
        }

        void Tick()
        {
            beats++;
            m_elapsed -= beatSeconds;
            GetComponent<AudioSource>().PlayOneShot(sound);
            
            if (beats <= timeSig.beatsPerBar)
                return;
            if (OnFinish != null)
            {
                foreach (UnityAction action in OnFinish)
                {
                    action.Invoke();
                }
            }
            Destroy(gameObject);
        }

        public float ScaledElapsed()
        {
            //Debug.Log(string.Format("Total Elapsed{0} : Length {1}", totalElapsed, length));
            return totalElapsed / length;
        }

        public static LeadIn PlayLeadIn(AudioClip _sound, float length,int bpm, TimeSignature timeSig, params UnityAction[] OnFinish)
        {
            GameObject go = new GameObject();
            go.name = "Lead in";
            go.AddComponent<AudioSource>();
            LeadIn leadIn = go.AddComponent<LeadIn>();
           
            leadIn.sound = _sound;
            leadIn.length = length;
            leadIn.timeSig = timeSig;
            leadIn.OnFinish = OnFinish;

            leadIn.SetTempo(bpm);
            leadIn.CalculateStartingElapsed();

            return leadIn;
        }

    }
}
