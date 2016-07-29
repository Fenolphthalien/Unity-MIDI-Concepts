using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace PrototypeFive
{
    [System.Serializable]
    public class OnRaycastEvent : UnityEvent { }

    public class ReactToRaycast : MonoBehaviour
    {
        public OnRaycastEvent OnRaycastHit;

        public void InvokeOnRayCast()
        {
            if (OnRaycastHit != null)
                OnRaycastHit.Invoke();
        }
    }
}
