using UnityEngine;
using System.Collections;

namespace PrototypeFive
{
    [System.Serializable]
    public class PlayerEvent : UnityEngine.Events.UnityEvent { }

    public class PlayerAvatar : MonoBehaviour
    {
        Vector3 mousePos;
        public Texture2D crosshair, beatReticule;

        public UnityMIDI.Metronome metronome;
        
        [Range(0,360)]
        public float beatRotation = 22.5f;
        Ray lastRay;
        RaycastHit lastHit;

        public PlayerEvent OnTargetSuccesfull, OnFireAtTargets, OnFireAtTargetsFail, OnPlayerHitTarget;

        public bool inputEnabled;

        bool fireAtTargeted { get { return Input.GetMouseButtonUp(1) && targeting; } }

        bool targeting;

        float fireDelay, delayFireFor = 0.1f;

        void Update()
        {
            if (!inputEnabled)
            {
                targeting = false;
                return;
            }

            mousePos = Input.mousePosition;
            if (Input.GetMouseButtonDown(0) && fireDelay <= 0)
            {
                Shoot();
            }

            //Target on Enter beat.
            if(targeting == true && metronome.EnteredBeat())
            {
                if(OnTargetSuccesfull != null)
                    OnTargetSuccesfull.Invoke();
            }
            //Begin targeting check.
            if (Input.GetMouseButtonDown(1) && metronome.IsInBeatOffset())
            {
                targeting = true;
                if (OnTargetSuccesfull != null)
                    OnTargetSuccesfull.Invoke();
            }

            //Player releases mouse when objects are targeted.
            if (fireAtTargeted)
            {
                if (OnFireAtTargets != null)
                    OnFireAtTargets.Invoke();
                targeting = false;
            }
            fireDelay -= Time.deltaTime;
            fireDelay = fireDelay > 0 ? fireDelay : 0;
        }

        void OnGUI()
        {
            if (!inputEnabled)
                return;

            int width, height;
            Vector2 pos;
            Rect canvas;
            if (beatReticule == null)
                return;
            Vector3 pos3 = new Vector3(mousePos.x, Screen.height - mousePos.y);
            Vector3 euler = new Vector3(0, 0, beatRotation * (metronome.ScaleBeatElapsed(0)));
            Matrix4x4 matrix = Matrix4x4.TRS(pos3, Quaternion.Euler(euler), Vector3.one);
            GUI.matrix = matrix;

            GUI.color = metronome.IsInBeatOffset() ? Color.green : Color.red;

            width = beatReticule.width;
            height = beatReticule.height;
            
            pos = new Vector2(0,0);
            pos.x -= width * 0.5f;
            pos.y -= height * 0.5f;
            canvas = new Rect(pos.x, pos.y, width, height);
            GUI.DrawTexture(canvas, beatReticule);

            GUI.matrix = Matrix4x4.identity;
            GUI.color = Color.white;

            if (crosshair == null)
                return;

            width = crosshair.width; 
            height = crosshair.height;
            pos = new Vector2(mousePos.x, Screen.height - mousePos.y);
            pos.x -= width * 0.5f;
            pos.y -= height * 0.5f;
            canvas = new Rect(pos.x, pos.y, width, height);
            GUI.DrawTexture(canvas, crosshair);
        }

        void Shoot()
        {
            Vector3 cameraFwd = Camera.main.transform.TransformVector(Vector3.forward);
            lastRay = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(lastRay, out lastHit))
            {
                ReactToRaycast reactObj = lastHit.collider.gameObject.GetComponent<ReactToRaycast>();
                if (reactObj != null)
                    reactObj.InvokeOnRayCast();
            }
            fireDelay = delayFireFor;
        }
    }
}
