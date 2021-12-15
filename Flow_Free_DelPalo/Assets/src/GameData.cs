using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// boceto de lo que guardariamos
struct CategoryData
{
    bool blocked;
    PackData[] packs;
}
struct PackData
{
    LevelData[] levels;
}
struct LevelData
{
    bool levelBlocked;
    bool bestMoves;
}

public class GameData 
{
    public int hintNum;
    CategoryData[] categories;
    public string hash;
}
