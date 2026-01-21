
using System;

[System.Serializable]
public struct OrderDifficultyCount
{
    public int OrderDifficulty;
    public int Count;
}

[System.Serializable]
public struct DayDifficulty
{
    public int Day;
    public int TimeLimit;
    public OrderDifficultyCount[] OrderDifficulties;
}