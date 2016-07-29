using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    public class MaterialCollection : MonoBehaviour
    {
        [SerializeField]
        Material[] m_materials = new Material[1];

        public Material GetMaterial(int i)
        {
            return m_materials[i];
        }

        public void AddMaterial(Material material)
        {
            foreach (Material m in m_materials)
            {
                if (m == material)
                    return;
            }
            Material[] buffer = new Material[m_materials.Length + 1];
            System.Array.Copy(m_materials, buffer, m_materials.Length);
            buffer[buffer.Length - 1] = material;
            m_materials = buffer;
        }
    }
}
