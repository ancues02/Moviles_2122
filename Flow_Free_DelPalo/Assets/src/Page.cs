using UnityEngine;

namespace FlowFree
{
    /**
     * Estructura auxiliar configurar las 
     * paginas de niveles desde los lotes
     */
    [System.Serializable]
    public struct Page
    {
        public string name;
        public Color color;
        public bool blocked;
    }
}