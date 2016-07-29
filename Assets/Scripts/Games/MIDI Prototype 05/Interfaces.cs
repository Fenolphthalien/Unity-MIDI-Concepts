using UnityEngine;
using System.Collections;

namespace PrototypeFive
{
    public interface IShootable
    {
        void Shoot();
    }

    public interface ITargetable
    {
        Transform GetTransform(out Vector3 positionOffset);
        void FireAt();
    }
}
