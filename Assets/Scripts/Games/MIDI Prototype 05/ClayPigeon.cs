using UnityEngine;
using System.Collections;

namespace PrototypeFive
{
    public class ClayPigeon : TargetableBase
    {
        public Vector3 forwardVector;
        float accelaration = 2, distanceTraveled;

        public delegate void BasicDelegate();
        public delegate void BasicDelegateObnbjCallback(ClayPigeon pigeon);

        public BasicDelegateObnbjCallback OnPreDestroy;

        const float destroyAfterTraveled = 10f;

        protected override void Update()
        {
            base.Update();
            transform.Translate(forwardVector*Time.deltaTime*(accelaration+1), Space.Self);
            accelaration -= Time.deltaTime;
            accelaration = accelaration > 0 ? accelaration : 0;
            distanceTraveled += Time.deltaTime * (accelaration + 1);
            if (distanceTraveled > destroyAfterTraveled)
            {
                BeginDestroy(); 
            }
        }

        public override Transform GetTransform(out Vector3 positionOffset)
        {
            positionOffset = Vector3.up * 2f;
            return transform;
        }

        public void BeginDestroy()
        {
            if (OnPreDestroy != null)
                OnPreDestroy.Invoke(this);
            OnPreDestroy = null;
            DestroyObject(gameObject);
        }

        public override void FireAt()
        {
            BeginDestroy();
        }
    }
}
