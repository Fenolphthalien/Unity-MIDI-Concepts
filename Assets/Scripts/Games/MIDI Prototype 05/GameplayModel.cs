using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityMIDI;

namespace PrototypeFive
{
    public class GameplayModel : MonoBehaviour
    {
        [Header("Members")]
        public int points;
        public List<ITargetable> targetedObjects = new List<ITargetable>();
        public List<ITargetable> hitQueue = new List<ITargetable>();
        public List<ClayPigeon> pigeonList = new List<ClayPigeon>();

        public bool playEnabled 
        { 
            get 
            { 
                return m_playEnabled; 
            }
            set
            {
                m_playEnabled = value; 
                player.inputEnabled = value;
                midiInterprator.interprateEvents = value;
            } 
        }

        bool m_playEnabled;

        [Header("MIDI Object references")]
        public MIDIPlayer02 midiPlayer;
        public Metronome metronome;

        [Header("Gameplay Object references")]
        public AudioClip leadInClip;
        public MIDIInterprator midiInterprator;
        public PlayerAvatar player;

        [Header("Target Controls")]
        public SideTargetControl leftTargetControl;
        public CenterTargetControl centerTargetControl = null;
        public SideTargetControl rightTargetControl;
       
        [Header("Clay pigeon Launchers")]
        public ClayPigeonLauncher leftPigeonLauncher;
        public ClayPigeonLauncher rightPigeonLauncher;

        public void Initialise()
        {
            points = 0;
            playEnabled = false;
        }
    }
}