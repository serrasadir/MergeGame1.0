using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class TaskSlot : MonoBehaviour, IPointerDownHandler
{
    private int requiredLevel;
    private bool isCompleted = false;
    [SerializeField] private GameObject greenTick; //completed task visual
    [SerializeField] private TextMeshProUGUI levelText; //leveltext
    [SerializeField] private Image applianceVisual;
    public void Initialize(int level)
    {
        requiredLevel = level;
        isCompleted = false;

        levelText.text = requiredLevel.ToString();
        UpdateColor(requiredLevel); //set the color
        greenTick.SetActive(false); 
    }

    public void MarkAsCompleted()
    {
        isCompleted = true;
        if (greenTick != null)
            greenTick.SetActive(true); //show the green tick
    }

    public int GetRequiredLevel()
    {
        return requiredLevel;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (isCompleted)
        {
            foreach (Appliance appliance in FindObjectsOfType<Appliance>())
            {
                if (appliance.GetLevel() == requiredLevel)
                {
                    appliance.UpdateColor(); // Restore original color
                }
            }
            Debug.Log($"Task completed and clicked: Level {requiredLevel}");
            TaskManager.Instance.RemoveTask(this);
            Destroy(gameObject); //remove the task slot
        }
    }

    public bool IsCompleted()
    {
        return isCompleted;
    }

    private void UpdateColor(int level)
    {
        if (levelColors.TryGetValue(level, out Color color))
        {
            if (applianceVisual != null)
            {
                applianceVisual.color = color;
            }
        }
    }

    //dictionary to store colors for each level
    private static readonly Dictionary<int, Color> levelColors = new Dictionary<int, Color>
    {
        { 2, new Color(1.0f, 0.65f, 0.94f) },  //pink
        { 4, new Color(0.9f, 0.8f, 0.5f) },  //yellow
        { 8, new Color(1.0f, 0.6f, 0.4f) },  //orange
        { 16, new Color(1.0f, 0.4f, 0.4f) }, //red
        { 32, new Color(0.8f, 0.4f, 1.0f) },  //purple
        { 64, new Color(0.5f, 0.5f, 1.0f) }, //blue
        { 128, new Color(0.3f, 0.9f, 0.5f) }, //green
        { 256, new Color(0.3f, 1.0f, 1.0f) },
        { 512, new Color(1.0f, 0.3f, 1.0f) }, //pink
        { 1024, new Color(1.0f, 0.7f, 0.2f) }, //yellow
        { 2048, new Color(0.2f, 0.3f, 1.0f) }  //
    };
}
