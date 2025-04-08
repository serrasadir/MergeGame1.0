using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSaveManager : MonoBehaviour
{
    private static string saveFilePath => Application.persistentDataPath + "/GameSave.json";

    public static void SaveGame()
    {
        GameData gameData = new GameData();
        if (BoardManager.Instance == null)
        {
            Debug.LogError("BoardManager is null cannot save the game.");
            return;
        }

        //save Producers
        foreach (Producer producer in FindObjectsOfType<Producer>())
        {
            if (producer == null || producer.transform.parent == null) continue;

            Cell cell = producer.transform.parent.GetComponent<Cell>();
            if (cell != null)
            {
                gameData.producers.Add(new ProducerData
                {
                    x = cell.x,
                    y = cell.y,
                    capacity = producer.capacity
                });
            }
        }

        //save appliances
        foreach (Appliance appliance in FindObjectsOfType<Appliance>())
        {
            Cell cell = appliance.transform.parent.GetComponent<Cell>();
            if (cell != null)
            {
                gameData.appliances.Add(new ApplianceData
                {
                    x = cell.x,
                    y = cell.y,
                    level = appliance.GetLevel()
                });
            }
            else
            {
                //appliance is in inventory so store only its level
                gameData.inventoryAppliances.Add(appliance.GetLevel());
            }
        }
        //save Tasks
        foreach (TaskSlot task in TaskManager.Instance.GetActiveTasks())
        {
            gameData.tasks.Add(new TaskData
            {
                requiredLevel = task.GetRequiredLevel(),
                isCompleted = task.IsCompleted()
            });
        }

        gameData.score = ScoreManager.Instance.GetScore();

        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(Application.persistentDataPath + "/gameSave.json", json);
    }


    public static void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("No save file found");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        GameData gameData = JsonUtility.FromJson<GameData>(json);
        //clear board before loading
        //BoardManager.Instance.ClearBoard();

        //load Producers
        foreach (ProducerData producerData in gameData.producers)
        {
            Cell cell = BoardManager.Instance.GetCell(producerData.x, producerData.y);
            if (cell != null)
            {
                GameObject newProducer = Instantiate(BoardManager.Instance.GetProducerPrefab(), cell.transform);
                newProducer.transform.SetParent(cell.transform, false);
                newProducer.transform.localPosition = Vector3.zero;
                newProducer.SetActive(true);

                Producer producer = newProducer.GetComponent<Producer>();
                producer.capacity = producerData.capacity;
                producer.SetCell(cell);
            }
        }

        //load Appliances onto the board
        foreach (ApplianceData applianceData in gameData.appliances)
        {
            Cell cell = BoardManager.Instance.GetCell(applianceData.x, applianceData.y);
            if (cell != null)
            {
                GameObject newAppliance = AppliancePool.Instance.GetPooledObject(applianceData.level);
                newAppliance.transform.SetParent(cell.transform, false);
                newAppliance.transform.localPosition = Vector3.zero;
                newAppliance.SetActive(true);

                Appliance appliance = newAppliance.GetComponent<Appliance>();
                appliance.SetLevel(applianceData.level);
                cell.SetOccupied(true);
            }
        }

        //load Appliances into Inventory
        InventoryManager.Instance.RestoreInventory(gameData.inventoryAppliances);
        //load tasks
        TaskManager.Instance.ClearTasks(); //clear old tasks before loading
        foreach (TaskData taskData in gameData.tasks)
        {
            TaskManager.Instance.RestoreTask(taskData.requiredLevel, taskData.isCompleted);
        }

        ScoreManager.Instance.SetScore(gameData.score);

        Debug.Log("Game Loaded!");
    }


    public static void DeleteSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted");
        }
        else
        {
            Debug.LogWarning("No save file found to delete.");
        }
    }

}
