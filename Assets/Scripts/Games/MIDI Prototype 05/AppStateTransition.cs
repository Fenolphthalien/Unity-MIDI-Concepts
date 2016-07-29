using UnityEngine;
using System.Collections;

namespace PrototypeFive
{
    public class AppStateTransition : BaseUpdateState
    {
        enum EState { WAITING, OUT, IN, DONE };
        public float screenFillAmount = 0;
        public Color screenFillColour = Color.black;
        public BaseGameState outState, inState;
        public float transtionSpeed;
        public GameStateEvent OnStateTransitioned;

        Texture2D fadeTex;

        static readonly AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        EState currentState;

        public AppStateTransition(Color fillColour, float transitionTime, BaseGameState _outState, BaseGameState _inState)
        {
            screenFillAmount = 0;
            screenFillColour = fillColour;
            transtionSpeed = 1f / (transitionTime * 0.5f);
            outState = _outState;
            inState = _inState;
            currentState = EState.WAITING;

            fadeTex = new Texture2D(1, 1);
            fadeTex.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        }

        ~AppStateTransition()
        {
            OnStateTransitioned = null;
            fadeTex = null;
        }

        public override void Enter()
        {
            currentState = EState.OUT;
            outState.OnPreExitState();
        }

        public override void Exit()
        {
            inState.OnReadyState();
        }

        public override bool Update()
        {
            Debug.Log(currentState);
            if (currentState == EState.WAITING)
                return false;
            if (currentState == EState.OUT)
            {
                screenFillAmount += transtionSpeed * Time.deltaTime;
                screenFillAmount = screenFillAmount >= 1 ? 1 : screenFillAmount;
             
                if(screenFillAmount >= 1)
                {
                    outState.OnExitState();
                    if(OnStateTransitioned != null)
                        OnStateTransitioned.Invoke(inState,false);
                    inState.OnEnterState(outState);
                    currentState = EState.IN;
                }
                return false;
            }
            if (currentState == EState.IN)
            {
                screenFillAmount -= transtionSpeed * Time.deltaTime;
                screenFillAmount = screenFillAmount <= 0 ? 0 : screenFillAmount;
                if (screenFillAmount <= 0)
                {
                    currentState = EState.DONE;
                    OnStateTransitioned = null;
                }
                return false;
            }
            return true;
        }

        public override void OnGUI()
        {
            float a = curve.Evaluate(screenFillAmount);
            Color c = screenFillColour;
            screenFillColour.a = a;
            fadeTex.SetPixel(1, 1, c);
            fadeTex.Apply();
            Rect screenRect = new Rect(0,0,Screen.width,Screen.height);
            GUI.DrawTexture(screenRect, fadeTex);
        }
    }
}

