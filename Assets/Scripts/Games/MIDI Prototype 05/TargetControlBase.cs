using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace PrototypeFive
{
    public enum DestriyReason {NONE = 1, PLAYER_SHOT = 2};

    [System.Serializable]
    public class TargetControlEvent : UnityEvent<ITargetable> { }

    public abstract class TargetControlBase : MonoBehaviour
    {
        [Header("References")]
        public GameObject targetPrefab;

        [Header("Gizmo Parameteres")]
        [Range(0, 1)]
        public float gizmoOpacity = 1f;
        public Mesh gizmoMesh;

        [SerializeField]
        protected Target[] objects = null;

        protected int targetIndex;

        [ContextMenu("Populate")]
        protected abstract void Populate();

        [ContextMenu("Depopulate")]
        protected abstract void Depopulate();

        protected virtual void OnDrawGizmos(){}

        public TargetControlEvent OnTargetPopped;

        public bool Pop(int stepBy, out Target target)
        {
            int steps = objects.Length, increment = stepBy > 0 ? stepBy : 1;
            target = null;
            //Use array length as steps so the loop walks the entire array and then exits false if it can't find an avaliable target.
            for (int i = 0; i < steps; i++)
            {
                if (targetIndex >= steps)
                    targetIndex = 0;

                if (objects[targetIndex] != null && !objects[targetIndex].isEnabled)
                {
                    target = objects[targetIndex];
                    targetIndex++;
                    target.PopUp();
                    if (OnTargetPopped != null)
                        OnTargetPopped.Invoke(target);
                    return true;
                }
                targetIndex++;
            }
            return false;
        }

        public void PushAll(bool force)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (force)
                    objects[i].ForceDown();
                else
                    objects[i].PushDown();
            }
        }
    }
}
