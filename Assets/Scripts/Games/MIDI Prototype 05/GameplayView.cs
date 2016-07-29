using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PrototypeFive
{
    public class GameplayView : MonoBehaviour
    {
        public TargetedGizmo gizmo;
        public SongCard card;

        ScreenFade screenFade;

        public Color fadeColour = Color.black;

        public void DisplayTargetedGizmos(GameplayModel model)
        {
            if(gizmo == null)
                return;

            List<ITargetable> targets = model.targetedObjects;
            if(targets.Count == 0)
                return;

            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null)
                    continue;
                gizmo.Draw(targets[i]);
            }
        }

        public void BeginFadeIn(float maxAlpha, float overSeconds)
        {
            screenFade = ScreenFade.BeginFade(screenFade, maxAlpha, overSeconds);
        }

        public void BeginFadeOut()
        {
            if (screenFade != null)
                screenFade.BeginFadeOut();
        }

        public void ClearAndDestroyFade()
        {
            if (screenFade != null)
                screenFade.Clear();
        }
    }
}