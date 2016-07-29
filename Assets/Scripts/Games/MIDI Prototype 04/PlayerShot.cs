using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    public class PlayerShot : MonoBehaviour, IHorizontalMovementHandler, IRequireDependancy<IPlayerShotContainer>, IRequireSingleIntialisation<QuadraticBezierTransform, Vector3>
    {
        float x, y;

        IPlayerShotContainer m_container;

        QuadraticBezierTransform qbt;

        public void HandleHorizontalMovement(float delta)
        {
            x -= delta;
            //float xDifference = x / (qbt.B.position.x - qbt.A.position.x);
           // y = qbt.Evaluate(xDifference).y;
            transform.position = new Vector3(x, y);
        }

        public void InjectDependancy(IPlayerShotContainer dependancy)
        {
            m_container = dependancy;
        }

        public void Initialise(QuadraticBezierTransform parametre1, Vector3 parametre2)
        {
            qbt = parametre1;
            transform.position = parametre2;
            x = parametre2.x;
            y = parametre2.y + qbt.height;
        }

        public void RemoveFromContainerAndDestroy()
        {
            if (m_container == null)
                return;
            IHorizontalMovementHandler obj = m_container.Remove(this); //Should return reference to this.gameObject if succesfull. This object should only be able to destroy itself.
            if (obj != null && obj == (IHorizontalMovementHandler)this)
                Destroy(gameObject);
        }
    }
}
