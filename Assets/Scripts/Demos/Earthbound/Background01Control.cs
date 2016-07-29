using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityMIDI;

namespace MIDIDemonstrations.Earthbound
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Background01Control : MonoBehaviour, INoteOnHandler
    {
        public enum ERampResolution
        {
            LOW = 16,
            MEDIUM = 32,
            HIGH = 64,
            VERY_HIGH = 128
        };

        public Color kickColour = Color.red, snareColour = Color.cyan;

        public bool displayKick, displaySnare;

        [SerializeField]
        AnimationCurve m_pulseTailCurve = AnimationCurve.Linear(0, 1, 1, 0);

        [SerializeField]
        ERampResolution m_rampResolution = ERampResolution.MEDIUM;

        Queue<Color> m_kickRampQueue, m_snareRampQueue;

        Texture2D m_kickRamp, m_snareRamp;

        Material m_material;

        const string kKickPath = "_KickRamp", kSnarePath = "_SnareRamp";

        [SerializeField]
        int m_framerate = 60;

        float m_trueFrameRate, m_kickTime = 0, m_snareTime = 0;

        void Awake()
        {
            m_kickTime = 0;
            m_snareTime = 0;

            m_trueFrameRate = m_framerate > 0 ? 1f / m_framerate : 0.014f; //If frame rate is 0 or less. Assume a framerate of 60fps.

            m_kickRampQueue = new Queue<Color>();
            m_snareRampQueue = new Queue<Color>();
            InitialiseRamps();
        }

        void OnDestroy()
        {
            DestroyObject(m_kickRamp);
            DestroyObject(m_snareRamp);
            m_kickRamp = null;
            m_snareRamp = null;
        }

        void Update()
        {
            if (displayKick)
            {
                m_kickTime += Time.deltaTime;
                if (m_kickTime >= m_trueFrameRate)
                {
                    Tick(m_kickRamp, m_kickRampQueue, kKickPath, ResolutionToStep());
                    m_kickTime -= m_trueFrameRate;
                }
            }

            if (displaySnare)
            {
                m_snareTime += Time.deltaTime;
                if (m_snareTime >= m_trueFrameRate)
                {
                    Tick(m_snareRamp, m_snareRampQueue, kSnarePath, ResolutionToStep());
                    m_snareTime -= m_trueFrameRate;
                }
            }
        }

        void Tick(Texture2D ramp, Queue<Color> queue, string path, int steps = 1)
        {
            if (ramp == null)
                return;

            int i;
            Color[] colours = new Color[(int)m_rampResolution], existingColours = ramp.GetPixels(0, 0, (int)m_rampResolution, 1);
            for (i = steps; i < existingColours.Length - (steps-1); i++)
            {
                colours[i - steps] = existingColours[i];
            }

            Color nextColor;
            for (i = steps; i > 0; i--)
            {
                nextColor = queue.Count > 0 ? queue.Dequeue() : Color.black;
                colours[(int)m_rampResolution - i] = nextColor;
            }
            ramp.SetPixels(colours);
            ramp.Apply();
        }

        void QueuePulse(Queue<Color> queue, ref float m_time, Color pulseColour, int velocity = 127)
        {
            EmptyQueue(queue);
            float maxAlpha = (float)velocity / 127;

            float m_tailDivider = 1f / (int)m_rampResolution;
            for (int i = 0; i < (int)m_rampResolution; i++)
            {
                m_kickRampQueue.Enqueue(pulseColour * m_pulseTailCurve.Evaluate(m_tailDivider * i)*maxAlpha);
            }
            m_time = m_trueFrameRate;
        }

        void TrimQueue(Queue<Color> queue, int amount)
        {
            for (int i = 0; i < queue.Count; i++)
            {
                Color last = queue.Dequeue();
                if (i <= queue.Count - amount)
                    continue;
                queue.Enqueue(last);
            }
        }

        void EmptyQueue(Queue<Color> queue)
        {
            for (int i = 0; i < queue.Count; i++)
            {
                queue.Dequeue();
            }
        }

        void InitialiseRamps()
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr != null)
                m_material = mr.sharedMaterial; //Don't want to instance the material.

            CreateRamp(ref m_kickRamp);
            CreateRamp(ref m_snareRamp);

            if (m_material != null)
            {
                m_material.SetTexture(kKickPath, m_kickRamp);
                m_material.SetTexture(kSnarePath, m_snareRamp);
            }
        }

        void CreateRamp(ref Texture2D tex2d)
        {
            tex2d = new Texture2D((int)m_rampResolution, 1);
            tex2d.hideFlags = HideFlags.HideAndDontSave;
            tex2d.filterMode = FilterMode.Point;
            tex2d.wrapMode = TextureWrapMode.Clamp;

            Color[] rampFill = new Color[(int)m_rampResolution];
            for (int i = 0; i < rampFill.Length; i++)
            {
                rampFill[i] = Color.black;
            }
            tex2d.SetPixels(rampFill);
            tex2d.Apply();
        }

        int ResolutionToStep()
        {
            switch(m_rampResolution)
            {
                case ERampResolution.LOW:
                    return 1;
                case ERampResolution.MEDIUM:
                    return 2;
                case ERampResolution.HIGH:
                    return 4;
                case ERampResolution.VERY_HIGH:
                    return 8;
            }
            return 1; //Failsafe.
        }

        public void OnNoteOn(MIDIMessage message)
        {
            Tone noteMask;
            int octaveMask;
            DrumMasks.Kick(out noteMask, out octaveMask);

            //If kick is triggered
            if (displayKick && message.IsNoteOn() && message.GetNote() == noteMask && message.GetOctave() == octaveMask)
            {
                QueuePulse(m_kickRampQueue, ref m_kickTime, kickColour, message.GetVelocity());
                Tick(m_kickRamp, m_kickRampQueue, kKickPath);
            }

            DrumMasks.Snare(out noteMask, out octaveMask);

            //If snare is triggered
            if (displaySnare && message.IsNoteOn() && message.GetNote() == noteMask && message.GetOctave() == octaveMask)
            {
                QueuePulse(m_snareRampQueue, ref m_snareTime, snareColour, message.GetVelocity());
                Tick(m_snareRamp, m_snareRampQueue, kSnarePath);
            }
        }
    }
}
