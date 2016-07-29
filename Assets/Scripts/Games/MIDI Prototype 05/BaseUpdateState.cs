using UnityEngine;
using System.Collections;

namespace PrototypeFive
{
    public abstract class BaseUpdateState
    {
        public abstract void Enter();

        public abstract void Exit();

        public abstract bool Update();

        public virtual void OnGUI() { }
    }
}