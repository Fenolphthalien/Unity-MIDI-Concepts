using UnityEngine;
using System.Collections.Generic;

namespace PrototypeFour
{
    public static class ResourceLUT
    {
        public static readonly Material[] pelletSkin = new Material[]
        {
            Resources.Load<Material>("M_ShortPellet_Neutral"),
            Resources.Load<Material>("M_ShortPellet_Up"),
            Resources.Load<Material>("M_ShortPellet_Down")
        };
    }
}
