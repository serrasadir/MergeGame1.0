using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [SerializeField] private GameObject taskSlotPrefab;
    [SerializeField] private Transform taskPanel; 
    private List<TaskSlot> activeTasks = new List<TaskSlot>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        GenerateNewTask();
        GenerateNewTask(); //2 tasks
    }

    public void GenerateNewTask()
    {
        if (activeTasks.Count >= 2) return; //max 2 tasks
       
        int randomLevel = GetRandomApplianceLevel(); //pick a level 
        GameObject newTaskObj = Instantiate(taskSlotPrefab, taskPanel);
        TaskSlot newTask = newTaskObj.GetComponent<TaskSlot>();

        newTask.Initialize(randomLevel);
        activeTasks.Add(newTask);
        HighlightMatchingAppliances();
        Debug.Log("Task is generated with level: " + randomLevel);
    }

    public void MarkTaskAsCompleted(int level)
    {
        foreach (TaskSlot task in activeTasks)
        {
            if (task != null && task.GetRequiredLevel() == level)
            {
                task.MarkAsCompleted();
                HighlightMatchingAppliances();
                break;
            }
        }
    }
    public void RetryTasks() //for retry button
    {
        ClearTasks();
        GenerateNewTask();
        GenerateNewTask();
    }

    public void ClearTasks()
    {
        foreach (TaskSlot task in activeTasks)
        {
            Destroy(task.gameObject);
        }
        activeTasks.Clear();
    }

    public void RestoreTask(int level, bool isCompleted) //while loading a saved game
    {
        GameObject newTaskObj = Instantiate(taskSlotPrefab, taskPanel);
        TaskSlot newTask = newTaskObj.GetComponent<TaskSlot>();

        newTask.Initialize(level);
        if (isCompleted) newTask.MarkAsCompleted();

        activeTasks.Add(newTask);
    }
    private void HighlightMatchingAppliances()//cell is not visible because of the appliances so highlight the appliances
    {
        foreach (Appliance appliance in FindObjectsOfType<Appliance>())
        {
            foreach (TaskSlot task in activeTasks)
            {
                if (appliance.GetLevel() == task.GetRequiredLevel())
                {
                    appliance.MakeApplianceGreen();
                }
            }
        }
    }

    public List<TaskSlot> GetActiveTasks()
    {
        return activeTasks;
    }

    public void RemoveTask(TaskSlot task)
    {
        activeTasks.Remove(task);
        GenerateNewTask(); //generate new task after removing
    }

    private int GetRandomApplianceLevel()
    {
        int[] validLevels = { 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048 };
        return validLevels[Random.Range(0, validLevels.Length)];
    }
}
