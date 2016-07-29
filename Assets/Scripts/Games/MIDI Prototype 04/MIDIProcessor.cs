using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityMIDI;

namespace PrototypeFour
{
    namespace PreGame
    {
        public class MIDIProcessor : IMIDIProcessor
        {
            public MIDIProcessor() { }

            public ProcessedMIDI ProcessMIDI(MIDI midi, int track)
            {
                //Error checking.
                if (midi == null)
                    return null;
                bool trackOnly = track > -1;

                if (!trackOnly)
                {
                    UnityEngine.Debug.LogError("This class is meant to process a single track only. Please set track to 0 or greater");
                    return null;
                }
                
                //Copy neccesary data out of the MIDI object.
                string name = midi.name;
                int trueTrack = midi.GetFormat() == 1 && track == 0 ? track + 1 : track, noteIndex = 0; //Make sure that the meta track is not being accessed in a format 01 file.
                float seconds = midi.GetTrack(trueTrack).seconds;
                ulong tempo = midi.BPM;
                TimeSignature timeSignature = ConvertToDuple(midi.timeSignature);

                //Initialise Local variables.
                uint bars = 0, noteRange = 0;
                MIDITrack midiTrack = midi.GetTrack(trueTrack);
                ReadOnlyCollection<MIDIMessage> messages = midiTrack.messages;
                NoteSpawnInfo currentSpawnInfo = null;
                List<NoteSpawnInfo> spawnList = new List<NoteSpawnInfo>();
                float lastBarPosition = 0;

                MIDIEvent runningStatus = MIDIEvent.UNKNOWN;

                MIDIMessage message = messages[0];
                float barPosition = 0, length = 0;
                int sourceNote = 0, downRange = 0, upRange = 0;
                bool sourceNoteSet = false;

                //Are we in duple time - ALways 1 until I find out whats causing the bug with Tuple time.
                int lengthMultiplier = timeSignature.metre == Metre.DUPLE ? 1 : 2;

                //Note -  this loop skips the last message in a track. As this is nearly always a Note Off message, it is unnecasary infomation.
                for (int i = 1; i < messages.Count; i++)
                {
                    MIDIMessage current = message; //Set the last message as current working message.
                    message = messages[i];
                    
                    //Calculate the time that working message takes in quarter notes.
                    length = midi.GetLengthRelativeToQuarterNote(message) * lengthMultiplier; // returns 1 if quarter note.
                    length /= timeSignature.beatsPerBar; //Quarter relative to 4/4 is 0.25 of a bar.
                    
                    //Check if the current message is note On
                    if (current.IsNoteOn())
                    {
                        //The first Note On event in the track marks the source note. The track range is determined from this note. 
                        if (!sourceNoteSet)
                        {
                            sourceNote = current.keyEvent.ToInt();
                            sourceNoteSet = true;
                        }
                        
                        //The first note has been identified and we can start calculating the tracks note range.
                        else
                        {
                            int difference = 0;
                            //Is the current Note On message lower than the source note?
                            if (sourceNote > current.keyEvent.ToInt())
                            {
                                // Replace the downRange if the difference is greater.
                                difference = sourceNote - current.keyEvent.ToInt();
                                downRange = difference > downRange ? difference : downRange;
                            }
                            //Or is the current Note On message higher than the source note?
                            else if (sourceNote < current.keyEvent.ToInt())
                            {
                                // Replace the up range if the difference is greater.
                                difference = current.keyEvent.ToInt() - sourceNote;
                                upRange = difference > upRange ? difference : upRange;
                            }
                            noteIndex = current.keyEvent.ToInt() - sourceNote;
                        }
                        //Are we checking for chords?
                        bool bHasBarPositionChanged = lastBarPosition != barPosition;
                        bool bReferencingCurrentSpawnInfo = currentSpawnInfo != null;
                        bool bNoteOnRunningStatus = runningStatus == MIDIEvent.NOTE_ON;
                        if (!bHasBarPositionChanged && bNoteOnRunningStatus && bReferencingCurrentSpawnInfo) //Chord check.
                        {
                            currentSpawnInfo.AddNoteToRange(noteIndex);
                        }
                        //If not
                        else
                        {
                            //Build the spawn infomation and add it to the list.
                            currentSpawnInfo = new NoteSpawnInfo(barPosition, noteIndex, length);
                            spawnList.Add(currentSpawnInfo);
                        }
                        runningStatus = MIDIEvent.NOTE_ON;
                    }
                    //A different message has occured in the track - running status is reset.
                    else
                    {
                        runningStatus = MIDIEvent.UNKNOWN;
                    }
                    lastBarPosition = barPosition;
                    barPosition += length; //Move the bar position the number of beats in lenght.
                }

                //Set the total number of bars.
                bars = (uint)Mathf.CeilToInt(barPosition);
                noteRange = (uint)(upRange + downRange); //Add the two rangees together to get the total range of notes.

                AssignNotespawnReferences(spawnList);
                return new ProcessedMIDI(name, seconds, tempo, bars, noteRange, timeSignature, spawnList.ToArray());
            }

            TimeSignature ConvertToDuple(TimeSignature signature)
            {
                if (signature.metre == Metre.DUPLE)
                    return signature;
                return new TimeSignature(signature.beatsPerBar / 3, Metre.DUPLE);
            }

            void AddToDownList(List<int> downList, int item)
            {
                downList.Contains(item);
            }

            void AssignNotespawnReferences(List<NoteSpawnInfo> spawns)
            {
                if (spawns == null)
                    return;
                int lastIndex = spawns.Count-1, i;
                spawns[0].SetAdjecentReferences(null, spawns[1]);
                spawns[lastIndex].SetAdjecentReferences(spawns[lastIndex - 1],null);
                for (i = 1; i < lastIndex - 1; i++)
                {
                    spawns[i].SetAdjecentReferences(spawns[i - 1], spawns[i + 1]);
                }
            }
        }
    }
}
