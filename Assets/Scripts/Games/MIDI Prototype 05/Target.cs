using UnityEngine;
using System.Collections;

namespace PrototypeFive
{
    public class Target : TargetableBase
    {
        public delegate void TargetCallback(Target obj);

        [Header("Do Not Modify")]
        public Transform targetObj;
        public Collider targetCollider;

        public TargetCallback OnPushDown, OnPopUp;

        public AudioClip HitSound;

        bool canHit;

        [Header("Editor Only")]
        [ContextMenuItem("Evaluate Anim Curve", "EvaluateAnimCurve_Debug"), Range(0, 1)]
        public float debugAnim = 0;
        public Vector3 gizmoOffset;
        const int startAngle = -1, neutral = 0, movingDown  = -1, movingUp = 1, animSpeed = 8;
        readonly static AnimationCurve animCurve = AnimationCurve.EaseInOut(0, startAngle, 1, -90);

        float animProgress = 0;
        int animDir = -1; //Tribool -1,0,1.

        public bool isEnabled { get {return canHit; } }

        protected override void Update()
        {
            base.Update();
            if (animDir == neutral)
                return;
            //Moving down
            else if (animDir == movingDown && animProgress > 0)
            {
                animProgress -= Time.deltaTime * animSpeed;
                if (animProgress <= 0)
                {
                    animProgress = 0;
                    animDir = 0;
                }
                EvaluateAnimCurve();
            }
            //Moving up.
            else if (animDir == movingUp && animProgress < 1)
            {
                animProgress += Time.deltaTime * animSpeed;
                if (animProgress >= 1)
                {
                    animProgress = 1;
                    animDir = 0;
                }
                EvaluateAnimCurve();
            }
        }

        void EvaluateAnimCurve()
        {
            if (targetObj == null)
                return;
            targetObj.localRotation = Quaternion.Euler(new Vector3(animCurve.Evaluate(animProgress), 0, 0));
        }

#if UNITY_EDITOR
        void Awake()
        {
            EvaluateAnimCurve();
        }

        void EvaluateAnimCurve_Debug()
        {
            debugAnim = Mathf.Clamp(debugAnim, 0, 1);
            if (targetObj == null)
                return;
            targetObj.localRotation = Quaternion.Euler(new Vector3(animCurve.Evaluate(debugAnim), 0, 0));
        }
#endif

        public void PopUp()
        {
            if(canHit)
                return;
            canHit = true;
            animDir = 1;
            targetCollider.enabled = true;
            if (OnPopUp != null)
                OnPopUp.Invoke(this);
        }

        public void PushDown()
        {
            if (!canHit)
                return;
            animDir = -1;
            canHit = false;
            targetCollider.enabled = false;
            if (OnPushDown != null)
                OnPushDown.Invoke(this);
        }

        public void ForceDown()
        {
            animProgress = 0;
            animDir = 0;
            EvaluateAnimCurve();
        }

        public void PushDownAndPlaySound()
        {
            PushDown();
            if (HitSound != null)
                AudioSource.PlayClipAtPoint(HitSound, this.transform.position, 0.2f);
        }

        public override void FireAt()
        {
            base.FireAt();
            PushDownAndPlaySound();
        }

        public override Transform GetTransform(out Vector3 positionOffset)
        {
            positionOffset = gizmoOffset;
            return transform;
        }

        void OnDrawGizmos()
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
            Gizmos.DrawWireCube(gizmoOffset, Vector3.one * 0.1f);

            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}