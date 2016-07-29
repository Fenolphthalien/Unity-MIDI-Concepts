using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    public class MoveHorizontallyDependantDynamic : MoveHorizontallyDependant
    {
        [SerializeField]
        protected float m_speed = 1;
        public float speed { get { return m_speed; } }

        [SerializeField]
        protected float m_accelaration; //per second;

        public bool limitSpeed, beginOnAwake;
        public float limitSpeedValue = 1;

        bool m_paused;

        void Awake()
        {
            m_paused = !beginOnAwake;
        }

        public override void HandleHorizontalMovement(float delta)
        {
            if (m_paused || !enabled)
                return;
            m_speed += m_accelaration * Time.deltaTime;
            if (limitSpeed && limitSpeedValue < m_speed)
                m_speed = limitSpeedValue;
            transform.Translate(Vector3.right * (delta*m_speed), Space.World);
        }

        public void Pause(bool value) { m_paused = value; }
        public void TogglePause() { m_paused = !m_paused; }
       
        public void Stop()
        {
            m_speed = 0;
        }

        public void SlowTo(float newSpeed)
        {
            m_speed = newSpeed < m_speed ? newSpeed : m_speed;
        }

        public void SlowBy(float speedDivider)
        {
            if(speedDivider > 0)
                m_speed /= speedDivider;
        }

        public void StartMoving(float _speed, float _accelaration)
        {
            m_speed = _speed;
            m_accelaration = _accelaration;
        }

        public void Freeze()
        {
            m_speed = 0;
            m_accelaration = 0;
        }
    }
}
