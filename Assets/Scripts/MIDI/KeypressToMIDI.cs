using UnityEngine;
using System.Collections;

namespace UnityMIDI
{
    public class KeypressToMIDI : MIDIDispatcher
    {
        const KeyCode
            key_C = KeyCode.A,
            key_CS = KeyCode.W,
            key_D = KeyCode.S,
            key_DS = KeyCode.E,
            key_E = KeyCode.D,
            key_F = KeyCode.F,
            key_FS = KeyCode.T,
            key_G = KeyCode.G,
            key_GS = KeyCode.Y,
            key_A = KeyCode.H,
            key_AS = KeyCode.U,
            key_B = KeyCode.J,
            key_C2 = KeyCode.K,
            key_CS2 = KeyCode.O,
            key_D2 = KeyCode.L,
            key_DS2 = KeyCode.P,
            key_E2 = KeyCode.Semicolon | KeyCode.Comma,
            key_Octave_Up = KeyCode.X,
            key_Octave_Down = KeyCode.Z,
            key_Velocity_Down = KeyCode.C,
            key_Velocity_Up = KeyCode.V;

        public int octave = 2;
        public int velocity = 100;
       
        void Update()
        {
            //Handle Octave input.
            if (IsKeyDown(key_Octave_Down))
                octave--;
            else if (IsKeyDown(key_Octave_Up))
                octave++;

            //Handle velocity input
            if (IsKeyDown(key_Velocity_Down))
            {
                int velocityDelta = velocity - 20;
                velocity = velocityDelta > 0 ? velocityDelta : 0;
            }
            else if (IsKeyDown(key_Velocity_Up))
            {
                int velocityDelta = velocity + 20;
                velocity = velocityDelta < 127 ? velocityDelta : 127;
            }
           
            if (IsKeyDown(key_C))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON,0,new KeyEvent(0,octave,velocity)));
            else if(IsKeyUp(key_C))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON,0,new KeyEvent(0,octave,0)));

            if (IsKeyDown(key_CS))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(1, octave, velocity)));
            else if (IsKeyUp(key_CS))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(1, octave, 0)));

            if (IsKeyDown(key_D))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(2, octave, velocity)));
            else if (IsKeyUp(key_D))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(2, octave, 0)));

            if (IsKeyDown(key_DS))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(3, octave, velocity)));
            else if (IsKeyUp(key_DS))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(3, octave, 0)));

            if (IsKeyDown(key_E))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(4, octave, velocity)));
            else if (IsKeyUp(key_E))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(4, octave, 0)));

            if (IsKeyDown(key_F))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(5, octave, velocity)));
            else if (IsKeyUp(key_F))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(5, octave, 0)));

            if (IsKeyDown(key_FS))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(6, octave, velocity)));
            else if (IsKeyUp(key_FS))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(6, octave, 0)));

            if (IsKeyDown(key_G))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(7, octave, velocity)));
            else if (IsKeyUp(key_G))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(7, octave, 0)));

            if (IsKeyDown(key_GS))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(8, octave, velocity)));
            else if (IsKeyUp(key_GS))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(8, octave, 0)));

            if (IsKeyDown(key_A))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(9, octave, velocity)));
            else if (IsKeyUp(key_A))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(9, octave, 0)));

            if (IsKeyDown(key_AS))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(10, octave, velocity)));
            else if (IsKeyUp(key_AS))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(10, octave, 0)));

            if (IsKeyDown(key_B))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(11, octave, velocity)));
            else if (IsKeyUp(key_B))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(11, octave, 0)));

            if (IsKeyDown(key_C2))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(0, octave+1, velocity)));
            else if (IsKeyUp(key_C2))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(0, octave+1, 0)));

            if (IsKeyDown(key_CS2))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(1, octave + 1, velocity)));
            else if (IsKeyUp(key_CS2))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(1, octave + 1, 0)));

            if (IsKeyDown(key_D2))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(2, octave + 1, velocity)));
            else if (IsKeyUp(key_D2))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(2, octave + 1, 0)));

            if (IsKeyDown(key_DS2))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(3, octave + 1, velocity)));
            else if (IsKeyUp(key_DS2))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(3, octave + 1, 0)));

            if (IsKeyDown(key_E2))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(4, octave + 1, velocity)));
            else if (IsKeyUp(key_E2))
                Dispatch(new MIDIMessage(MIDIEvent.NOTE_ON, 0, new KeyEvent(4, octave + 1, 0)));

        }

        bool IsKeyDown(KeyCode key){return Input.GetKeyDown(key);}

        bool IsKeyUp(KeyCode key){ return Input.GetKeyUp(key);}
    }
}
