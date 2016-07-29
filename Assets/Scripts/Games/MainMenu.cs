using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PrototypeUIGeneric;

namespace NOsu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        Button m_newGameButton = null;

        [SerializeField]
        SongSelectionViewPresentation m_songSelection = null;

        [SerializeField]
        NosuGame nosuGame = null;

        [SerializeField]
        CanvasGroup instructions = null;

        void Awake()
        {
            m_newGameButton.onClick.AddListener(InvokeNewGame);
        }

        void InvokeNewGame()
        {
            if (m_songSelection != null && nosuGame != null)
                nosuGame.StartNewGame(m_songSelection.GetCurrentMIDI(), m_songSelection.GetCurrentTrackIndex());
        }

        public void ToggleVisibility(bool visibility)
        {
            this.gameObject.SetActive(visibility);
            instructions.alpha = visibility ? 1 : 0;
        }
    }
}
