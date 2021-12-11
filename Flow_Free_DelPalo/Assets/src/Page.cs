namespace FlowFree
{
    /**
     * Estructura auxiliar configurar las 
     * paginas de niveles  desde los lotes
     */
    [System.Serializable]
    public struct Page
    {
        public string name;
        public bool blocked;
    }

    // El nº de niveles en el lote es siempre multiplo de 30,
    // cada pagina tiene 30 tiles.
    struct LevelPage
    {
        Page pageAttributes;
        LevelTile[] tiles;
    }
}