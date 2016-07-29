using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace PrototypeFive
{
   public delegate void GameplayEvent ();

    public class GameplayControl : BaseGameState
    {
        public PrototypeFive.GameplayModel model;
        public GameplayView view;

        public GameObject viewRoot;

        GameplayEvent OnPreGameplayExit;

        BaseUpdateState updateState;

        [Header("Editor Only")]
        public bool debugBeatTargeting;

        float fade = 0;

        //BaseGameState implementation
        public override void OnEnterState(BaseGameState callingState)
        {
            base.OnEnterState(callingState);
            ToggleViewRootVisibility(true);

            model.midiInterprator.OnBeginPlay.AddListener(Interprator_OnPlayEnable);
            model.midiInterprator.OnFireLeftPigeon.AddListener(Interprator_OnLeftPigeonFire);
            model.midiInterprator.OnPopupLeft.AddListener(Interprator_OnLeftPopup);
            model.midiInterprator.OnPopupCenter.AddListener(Interprator_OnCenterPopup);
            model.midiInterprator.OnPopupRight.AddListener(Interprator_OnRightPopup);
            model.midiInterprator.OnFireRightPigeon.AddListener(Interprator_OnRightPigeonFire);
            model.midiInterprator.OnStopPlay.AddListener(Interprator_OnPlayDisable);
            model.midiInterprator.OnExitStage.AddListener(Interprator_OnStageEnd);

            model.midiPlayer.OnPlaybackStart.AddListener(OnMIDIPlayerStart);
            model.midiPlayer.OnPlaybackStop.AddListener(Interprator_OnStageEnd);
            model.midiPlayer.OnPlaybackFinish.AddListener(Interprator_OnStageEnd);

            model.player.OnTargetSuccesfull.AddListener(Player_TargetObject);
            model.player.OnFireAtTargets.AddListener(Player_FireAtTargets);
            model.player.OnFireAtTargetsFail.AddListener(Player_FireAtTargetsFail);

            model.playEnabled = false;

            Cursor.visible = false;
        }

        public override void OnReadyState()
        {
            //Read args.
            if (lastArgs == null)
            {
                if (application is MainApp)
                    (application as MainApp).GameplayToMainMenu(this);
            }
            const float leadInSeconds = 2f;

            model.midiPlayer.SetMIDI(lastArgs.midiFile);
            UnityMIDI.LeadIn leadIn = UnityMIDI.LeadIn.PlayLeadIn
            (
                model.leadInClip,
                leadInSeconds, 
                (int)lastArgs.midiFile.BPM, 
                lastArgs.midiFile.timeSignature, 
                new UnityAction[] { new UnityAction(model.midiPlayer.Play) }
           );
           updateState = new PreGameUpdateState(view.card, leadIn, lastArgs.midiFile);
           updateState.Enter();
        }

        public override void OnPreExitState()
        {
            model.midiPlayer.Stop();
            model.playEnabled = false;

            model.midiInterprator.OnBeginPlay.RemoveListener(Interprator_OnPlayEnable);
            model.midiInterprator.OnFireLeftPigeon.RemoveListener(Interprator_OnLeftPigeonFire);
            model.midiInterprator.OnPopupLeft.RemoveListener(Interprator_OnLeftPopup);
            model.midiInterprator.OnPopupCenter.RemoveListener(Interprator_OnCenterPopup);
            model.midiInterprator.OnPopupRight.RemoveListener(Interprator_OnRightPopup);
            model.midiInterprator.OnFireRightPigeon.RemoveListener(Interprator_OnRightPigeonFire);
            model.midiInterprator.OnStopPlay.RemoveListener(Interprator_OnPlayDisable);
            model.midiInterprator.OnExitStage.RemoveListener(Interprator_OnStageEnd);

            model.midiPlayer.OnPlaybackStart.RemoveListener(OnMIDIPlayerStart);
            model.midiPlayer.OnPlaybackStop.RemoveListener(Interprator_OnStageEnd);
            model.midiPlayer.OnPlaybackFinish.RemoveListener(Interprator_OnStageEnd);

            model.player.OnTargetSuccesfull.RemoveListener(Player_TargetObject);
            model.player.OnFireAtTargets.RemoveListener(Player_FireAtTargets);
            model.player.OnFireAtTargetsFail.RemoveListener(Player_FireAtTargetsFail);
        }
       
        public override void OnExitState()
        {
            ToggleViewRootVisibility(false);
           
            DestroyAllPigeons();
            PushAll(true);
           
            model.hitQueue.Clear();
            model.targetedObjects.Clear();
            view.ClearAndDestroyFade();

            model.midiPlayer.StopAndReset();
        }

        public override StateChangeArgs GetArgs()
        {
            return null;
        }

        // Model Event callbacks
        void Interprator_OnPlayEnable()
        {
            model.playEnabled = true;
            model.player.inputEnabled = true;
            view.BeginFadeOut();
        }

        void Interprator_OnLeftPigeonFire()
        {
            ClayPigeon pigeon;
            if (model.rightPigeonLauncher.TryLaunch(out pigeon))
            {
                AddPigeon(pigeon);
            }
        }

        void Interprator_OnLeftPopup()
        {
            const int steps = 0;
            Target target;
            if (model.leftTargetControl.Pop(steps, out target))
            {
                AddTarget(target);
            }
        }

        void Interprator_OnCenterPopup()
        {
            const int steps = 0;
            Target target;
            if (model.centerTargetControl.Pop(steps, out target))
            {
                AddTarget(target);
            }
        }

        void Interprator_OnRightPopup()
        {
            const int steps = 0;
            Target target;
            if (model.rightTargetControl.Pop(steps, out target))
            {
                AddTarget(target);
            }
        }

        void Interprator_OnRightPigeonFire()
        {
            ClayPigeon pigeon;
            if (model.rightPigeonLauncher.TryLaunch(out pigeon))
            {
                AddPigeon(pigeon);
            }
        }

        void Interprator_OnPlayDisable()
        {
            PushAll(false);
            DestroyAllPigeons();
            model.playEnabled = false;
            model.player.inputEnabled = false;
            view.BeginFadeIn(0.6f, 0.25f);
        }

        void Interprator_OnStageEnd()
        {
            model.midiPlayer.StopAndReset();
            model.metronome.Stop(true);

            InvokeGameplayEvent(OnPreGameplayExit);

            //Transtition Back to menu.
            application.ChangeStateImplicitly(GameState.MainMenu, this);

        }

        void Player_TargetObject()
        {
            ITargetable targetObj = null;
            //Pop any null objects off the queue and break loop on the first instance or if the queue is emptied.
            while (targetObj == null && model.hitQueue.Count > 0)
            {
                targetObj = model.hitQueue[0];
                model.hitQueue.RemoveAt(0);
            }
            if(targetObj == null)
                return;
            model.targetedObjects.Add(targetObj);
            Debug.Log(model.targetedObjects.Count);
        }

        void Player_FireAtTargets()
        {
            if (model.targetedObjects.Count <= 0)
                return;
            
            ITargetable[] targetedObjects = model.targetedObjects.ToArray();

            int length = model.targetedObjects.Count;
            foreach (ITargetable t in targetedObjects)
            {
                t.FireAt();
            }
            model.targetedObjects.Clear();
        }

        void Player_FireAtTargetsFail()
        {
            if (model.targetedObjects.Count <= 0)
                return;
            model.targetedObjects.Clear();
        }

        void OnMIDIPlayerStart()
        {
            model.playEnabled = true;
            model.metronome.Stop(false);
            model.metronome.SetTempo((int)model.midiPlayer.midi.BPM);
            model.metronome.Play();
        }

        void OnBeforePigeonIsDestroyed(ClayPigeon pigeon)
        {
            //Clean up destory callback.
            OnPreGameplayExit -= pigeon.BeginDestroy;

            model.hitQueue.Remove(pigeon);
            model.targetedObjects.Remove(pigeon);
        }

        void OnBeforeTargetIsPushed(Target target)
        {
            model.hitQueue.Remove(target);
            model.targetedObjects.Remove(target);
        }

        //Methods
        void ToggleViewRootVisibility(bool visibility)
        {
            if (viewRoot == null)
                return;
            viewRoot.SetActive(visibility);
        }

        void InvokeGameplayEvent(GameplayEvent evnt)
        {
            if (evnt != null)
                evnt.Invoke();
        }

        void AddPigeon(ClayPigeon pigeon)
        {
            model.hitQueue.Add(pigeon);
            model.pigeonList.Add(pigeon);
            pigeon.OnPreDestroy = OnBeforePigeonIsDestroyed;

            //Tell the pigeon to destroy itself when the gameplay ends. 
            //Call back reference is removed from the BeforePigoenIsDestoyed event.
            OnPreGameplayExit += pigeon.BeginDestroy;
        }

        void AddTarget(Target target)
        {
            model.hitQueue.Add(target);
            target.OnPushDown = OnBeforeTargetIsPushed;
            //Debug.Log(model.hitQueue.Count);
        }

        void PushAll(bool force)
        {
            model.rightTargetControl.PushAll(force);
            model.centerTargetControl.PushAll(force);
            model.leftTargetControl.PushAll(force);
        }

        void DestroyAllPigeons()
        {
            for (int i = 0; i < model.pigeonList.Count; i++)
            {
                if (model.pigeonList[i] == null)
                    continue;
                model.pigeonList[i].BeginDestroy();
                model.pigeonList[i] = null;
            }
            model.pigeonList.Clear();
        }

        void Update()
        {
            if (view != null)
                view.DisplayTargetedGizmos(model);

            if (updateState != null)
                updateState.Update();

            if (!debugBeatTargeting)
                return;

            if (Input.GetKeyDown(KeyCode.Space))
                Player_TargetObject();
            if (Input.GetKeyDown(KeyCode.Tab))
                Player_FireAtTargets();
        }
    }
}
