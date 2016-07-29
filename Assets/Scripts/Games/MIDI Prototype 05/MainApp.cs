using UnityEngine;
using System.Collections;

namespace PrototypeFive
{
    public enum GameState { MainMenu, Gameplay};
    public class MainApp : MonoBehaviour, IApplicationMain
    {
        public BaseGameState mainMenu, gameplay;
        private BaseGameState current;

        public GameState entryState;

        BaseUpdateState updateState;

        bool transitionOnStateChange;

        void Awake()
        {
            mainMenu.Initialise(this);
            gameplay.Initialise(this);
            ChangeStateImplicitly(entryState,null);
            SetTransition(true);
        }

        void Update()
        {
            if (updateState == null)
                return;
            if(updateState.Update())
            {
                updateState.Exit();
                updateState = null;
            }
        }

        void ChangeUpdateState(BaseUpdateState newUpdateState)
        {
            if (updateState != null)
                updateState.Exit();
            updateState = newUpdateState;
            updateState.Enter();
        }

        public void ChangeStateImplicitly(GameState newState, BaseGameState sender)
        {
            BaseGameState toState = GetState(newState);
            BeginChangeState(toState);
        }

        void BeginChangeState(BaseGameState newState)
        {
            if (newState == null || newState == current)
                return;

            if (transitionOnStateChange)
            {
                AppStateTransition transition = new AppStateTransition(Color.black, 0.5f, current, newState);
                transition.OnStateTransitioned = ForceReplaceCurrent;
                ChangeUpdateState(transition);
                return;
            }
            BaseGameState oldState = current;
            if (current != null)
            {
                current.OnPreExitState();
                current.OnExitState();
            }
            current = newState;
            current.OnEnterState(oldState);
            current.OnReadyState();
        }

        public void MainMenuToGameplay(MainMenuControl mainMenuState)
        {
            if (current != mainMenuState)
                return;
            BeginChangeState(gameplay);
        }

        public void GameplayToMainMenu(GameplayControl gameplayState)
        {
            if (current != gameplayState)
                return;
            BeginChangeState(mainMenu);
        }

        BaseGameState GetState(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    return mainMenu;
                case GameState.Gameplay:
                    return gameplay;
            }
            return null;
        }
       
        public void ForceReplaceCurrent(BaseGameState newState, bool call)
        {
            if (newState == null)
                return;
            if (call && current != null)
                current.OnExitState();
            BaseGameState oldState = current;
            current = newState;
            if(call)
                current.OnEnterState(oldState);
        }

        public void SetTransition(bool transition)
        {
            transitionOnStateChange = transition;
        }

        void OnGUI()
        {
            if (updateState != null)
                updateState.OnGUI();
        }
    }
}
