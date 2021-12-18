using UnityEngine;

namespace FlowFree
{
    /**
     * Estructura auxiliar configurar las 
     * paginas de niveles desde los lotes
     */
    [System.Serializable]
    public class Page
    {
        public const int LEVELS_PER_PAGE = 30;
        public string name;
        public Color color;
    }
}