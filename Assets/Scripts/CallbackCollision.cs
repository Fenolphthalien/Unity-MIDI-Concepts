using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class CallbackCollision : MonoBehaviour
{
    public delegate void CollisionCallback(Collider c);

    public CollisionCallback onTriggerEnter, onTriggerStay, onTriggerExit;

    void OnDestroy()
    {
        onTriggerEnter = null;
        onTriggerStay = null;
        onTriggerExit = null;
    }

    void OnTriggerEnter(Collider c)
    {
        if (onTriggerEnter != null)
            onTriggerEnter.Invoke(c);
    }

    void OnTriggerStay(Collider c)
    {
        if (onTriggerStay != null)
            onTriggerStay.Invoke(c);
    }

    void OnTriggerExit(Collider c)
    {
        if (onTriggerExit != null)
            onTriggerExit.Invoke(c);
    }
}
