using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    [RequireComponent(typeof(BoxCollider),typeof(MoveHorizontallyDependantDynamic))]
    public class KillZone : MonoBehaviour, IRequireSingleIntialisation<Callback,Vector3>
    {
        public Vector3 lookTargetOffset;
        MoveHorizontallyDependantDynamic m_horizontalMovement;

        float m_damageTime = 0;

        Callback m_onOverlapPlayer;

        public void Initialise(Callback parametre1, Vector3 parametre2)
        {
            m_onOverlapPlayer = parametre1;
            transform.position = parametre2;
            m_horizontalMovement = GetMovementComponent();
            m_horizontalMovement.Freeze();
            m_damageTime = 0;
        }
        
        void Update()
        {
            if (m_damageTime > 0f)
            {
                m_damageTime = m_damageTime - Time.deltaTime > 0 ? m_damageTime - Time.deltaTime : 0;
                if (m_damageTime > 0)
                    return;
                m_horizontalMovement.Pause(false);
            }
        }

        void OnTriggerEnter(Collider C)
        {
            PlayerShot playerShot = C.GetComponent<PlayerShot>();
            if (playerShot != null)
            {
                HandlePlayerShotEnter(playerShot);
            }
        }

        void HandlePlayerShotEnter(PlayerShot shot)
        {
            shot.RemoveFromContainerAndDestroy();
            Damage(0.25f);
        }

        void OnTriggerStay(Collider C)
        {
            Player player = C.GetComponent<Player>() as Player;
            if (player == null)
                return;
            Bounds bounds = player.GetBounds();
            float xBounds = bounds.extents.x;
            if (m_onOverlapPlayer != null)
                m_onOverlapPlayer.Invoke();
        }

        public Vector3 GetLooktargetPosition()
        {
            return lookTargetOffset + transform.position + (GetSpeedOffset()*Vector3.right);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(GetLooktargetPosition(), Vector3.one);
        }

        float GetSpeedOffset()
        {
            GetMovementComponent();
            if(m_horizontalMovement == null)
                return 0;
            return m_horizontalMovement.speed;
        }

        public MoveHorizontallyDependantDynamic GetMovementComponent()
        {
            if (m_horizontalMovement == null)
                m_horizontalMovement = GetComponent<MoveHorizontallyDependantDynamic>();
            return m_horizontalMovement;
        }

        public void Damage(float AddDamageDelay)
        {
            m_damageTime += AddDamageDelay;
            m_horizontalMovement.Pause(true);
        }

        public void SetHeight(float height)
        { 
            Vector3 oldScale = transform.localScale;
            transform.localScale = new Vector3(oldScale.x, height, oldScale.z);
        }

        public void SetMaxSpeed(float maxSpeed)
        {
            if (m_horizontalMovement == null)
                return;
            m_horizontalMovement.limitSpeedValue = maxSpeed;
        }

        public void Stop()
        {
            if (m_horizontalMovement != null)
                m_horizontalMovement.Freeze();
        }

        public void StartMoving(float speed = 0, float accelaration = 1)
        {
            if (m_horizontalMovement != null)
                m_horizontalMovement.StartMoving(speed, accelaration);
        }
    }
}
