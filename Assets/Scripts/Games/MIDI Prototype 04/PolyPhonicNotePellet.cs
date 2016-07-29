using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    public class PolyPhonicNotePellet : NotePellet
    {
        [SerializeField]
        Transform m_childPelletTransform = null, m_transformA = null, m_transformB = null;

        [SerializeField]
        Transform[] m_pelletClones;

        [SerializeField]
        CallbackCollision callbackObj = null, callbackA = null, callbackB = null;

        [SerializeField]
        PingPongPath m_path = new PingPongPath(-Vector3.up,Vector3.up, null);

        public float unitsPerSecond = 1;

        public bool invertDirection;
        
        [SerializeField, HideInInspector]
        float m_upsDiv, m_cloneSpacing;

        public void Initialise(Vector3 LocalA, Vector3 LocalB)
        {
            m_path.A = LocalA;
            m_path.B = LocalB;
            callbackObj.onTriggerEnter = OnTriggerEnter;
            callbackA.onTriggerEnter = OnTriggerEnter;
            callbackB.onTriggerEnter = OnTriggerEnter;
          
            UpdateTransforms(); 
        }

        public void Initialise(Vector3 LocalA, Vector3 LocalB, float _unitsPerSecond)
        {
            unitsPerSecond = _unitsPerSecond;
            Initialise(LocalA, LocalB);
        }

        public void Initialise(Vector3 LocalA, Vector3 LocalB, float _unitsPerSecond, int clones, bool pelletsMoveDown)
        {
            if (clones > 0)
            {
                int i;
                m_pelletClones = new Transform[clones];
                m_cloneSpacing = 1f / (clones + 1);
                for (i = 0; i < clones; i++)
                {
                    m_pelletClones[i] = Instantiate(m_childPelletTransform.gameObject).transform;
                    m_pelletClones[i].SetParent(this.transform);
                    CallbackCollision cc = m_pelletClones[i].GetComponent<CallbackCollision>();
                    if (cc == null)
                        continue;
                    cc.onTriggerEnter = OnTriggerEnter;
                }
            }
            invertDirection = pelletsMoveDown;
            Initialise(LocalA, LocalB, _unitsPerSecond);
        }

        void Update()
        {
            m_childPelletTransform.position = m_path.Evaluate(Time.time * m_upsDiv,invertDirection);
            if (m_pelletClones == null)
                return;
            for (int i = 0; i < m_pelletClones.Length; i++)
            {
                m_pelletClones[i].position = m_path.Evaluate(Time.time * m_upsDiv + (i + 1) * m_cloneSpacing,invertDirection);
            }
        }

        void UpdateTransforms()
        {
            if (m_transformA == null || m_transformB == null)
                return;
            float AMult = m_path.B.y > m_path.A.y ? -0.5f : 0.5f, BMult = AMult * -1f;
            m_transformA.localPosition = m_path.A + Vector3.up * AMult;
            m_transformB.localPosition = m_path.B + Vector3.up * BMult;
            m_childPelletTransform.position = m_path.Evaluate(0.5f);
            m_upsDiv = unitsPerSecond / m_path.DistanceBetweenAB();
        }

        public void Translate(Vector3 translation)
        {
            m_path.A += translation;
            m_path.B += translation;
            UpdateTransforms();
        }

        protected override void OnDrawGizmos()
        {
            UpdateTransforms();
            Gizmos.color = Color.white;
            Gizmos.DrawLine(m_path.A + transform.position, m_path.B + transform.position);

            Gizmos.DrawWireSphere(m_transformA.position, m_snapRadius);
            Gizmos.DrawWireSphere(m_transformB.position, m_snapRadius);
            Gizmos.DrawWireSphere(m_childPelletTransform.position, m_snapRadius);
            if (m_pelletClones == null)
                return;
            for (int i = 0; i < m_pelletClones.Length; i++)
            {
                Gizmos.DrawWireSphere(m_pelletClones[i].position, m_snapRadius);
            }
        }

        public override bool InSnapBounds(Vector3 position, ref Vector3 snapTarget)
        {
            float m_snapSq = m_snapRadius * m_snapRadius;
           
            Vector3 difference_Pellet = position - m_childPelletTransform.position;
            float distanceSq_pellet = (difference_Pellet.x * difference_Pellet.x) + (difference_Pellet.y * difference_Pellet.y);
            if (distanceSq_pellet <= m_snapSq)
            {
                snapTarget = m_childPelletTransform.position;
                return true;
            }

            if (m_pelletClones != null)
            {
               Vector3 difference_clone;
               float distanceSq_clone;
               for (int i = 0; i < m_pelletClones.Length; i++)
               {
                   difference_clone = position - m_pelletClones[i].position;
                   distanceSq_clone = (difference_clone.x * difference_clone.x) + (difference_clone.y * difference_clone.y);
                   if (distanceSq_clone <= m_snapSq)
                   {
                       snapTarget = m_pelletClones[i].position;
                       return true;
                   }
               }
            }

            Vector3 difference_A = position - m_transformA.position;
            float distanceSq_A = (difference_A.x * difference_A.x) + (difference_A.y * difference_A.y);
            if (distanceSq_A <= m_snapSq)
            {
                snapTarget = m_transformA.position;
                return true;
            }

            Vector3 difference_B = position - m_transformB.position;
            float distanceSq_B = (difference_B.x * difference_B.x) + (difference_B.y * difference_B.y);
            if (distanceSq_B <= m_snapSq)
            {
                snapTarget = m_transformB.position;
                return true;
            }
            return false;

        }
    }
}