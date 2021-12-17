using System.Collections.Generic;

namespace FlowFree.Serializable
{
    [System.Serializable]
    public class GameData
    {
        public string hash;
        public int hints;
        public List<CategoryData> categories;

        public GameData()
        {
            categories = new List<CategoryData>();
        }
    }

    [System.Serializable]
    public class CategoryData
    {
        public string name;
        public List<PackData> packs;
        public CategoryData()
        {
            packs = new List<PackData>();
        }
    }

    [System.Serializable]
    public class PackData
    {
        public string name;
        public bool blocked;
        public int totalLevels;
        public int completedLevels;
        public int lastUnlockedLevel;
        public List<int> bestMoves;
    }
}
