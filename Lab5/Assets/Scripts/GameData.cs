using System;
using System.Collections.Generic;

[Serializable]
public class RecordEntry
{
    public string playerName;
    public int coins;
    public int collisions;
    public float time;
    public string date;
}

[Serializable]
public class GameSaveData
{
    public int totalCoinsCollected;
    public int totalDeaths;
    public int totalAttempts;
    public List<RecordEntry> records = new List<RecordEntry>();
}
