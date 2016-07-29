using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    public class PolyphonicLongNotePellet : NotePellet
    {
        const float addShotsAfter = 0.1f;
        bool bEntered;
        Player m_player;

        float tickElapsed;

        void OnCollisionEnter(Collision C)
        {
            Player p;
            p = C.gameObject.GetComponent<Player>();
            if (p != null)
                PlayerCollided(p);
        }

        void OnCollisionStay(Collision C)
        {
            if(m_player == null)
                return;
            tickElapsed += Time.deltaTime;
            if (tickElapsed >= addShotsAfter)
            {
                tickElapsed -= addShotsAfter;
                m_player.AddShots(1);
            }
        }

        void onCollisionExit(Collision C)
        {
            if (bEntered && m_player != null)
            {
                RemoveSelfAndDestroy();
            }
            bEntered = false;
        }

        protected override void PlayerCollided(Player p)
        {
            m_player = p;
            bEntered = true;
        }
    }
}
