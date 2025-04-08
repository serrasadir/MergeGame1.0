using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProducerData
{
    public int x, y;
    public int capacity;
}

[System.Serializable]
public class ApplianceData
{
    public int x, y;
    public int level;
}
[System.Serializable]
public class TaskData
{
    public int requiredLevel;
    public bool isCompleted;
}
public class ScoreData
{
    public int score;
}
[System.Serializable]
public class GameData
{
    public List<ProducerData> producers = new List<ProducerData>();
    public List<ApplianceData> appliances = new List<ApplianceData>();
    public List<int> inventoryAppliances = new List<int>();
    public List<TaskData> tasks = new List<TaskData>();
    public int score;
}


//created data class since we want to save them