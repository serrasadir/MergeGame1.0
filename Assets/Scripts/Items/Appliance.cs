using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class Appliance : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private int level = 2;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Cell currentCell; //track which cell this appliance is on
    [SerializeField] private Image childImage;

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
        { 2048, new Color(0.2f, 0.3f, 1.0f) }  //black
    };
    private void Start()
    {
        UpdateLevelText();
        UpdateColor();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        If2048Destroy();//if appliance reached levelü 2048 delete it
    }

    public void MergeAppliances(Appliance other)
    {  
        this.level *= 2;
        int earnedPoints = this.level; // Points equal to the merged level
        ScoreManager.Instance.AddPoints(earnedPoints); // Update points
        UpdateLevelText();
        UpdateColor();

        Debug.Log("Merging. New Level: " + this.level);
        if (other.currentCell != null)
        {
            SetCell(other.currentCell); //move the new appliance to the merged cell
        }
        //remove the old appliance
        other.DestroyAppliance();


        ProducerAnimation animation = this.GetComponent<ProducerAnimation>();
        animation.MergeAppliances(this.transform.position);//sparkle animation

        TaskManager.Instance.MarkTaskAsCompleted(level); //check if any task have been completed

    }

    public void DestroyAppliance()
    {
        if (currentCell != null)
        {
            currentCell.SetOccupied(false); //free the cell
            currentCell = null;
        }
        ResetAppliance();
        gameObject.SetActive(false);
        AppliancePool.Instance.ReturnToPool(gameObject,2);
    }

    private void ResetAppliance()
    {
        level = 2; //reset level to default
        UpdateLevelText();
        UpdateColor();
    }

    public void If2048Destroy()
    {
        if (level == 2048)
        {
            Debug.Log("Level 2048. Removing it.");
            Destroy(this.gameObject);
            currentCell.SetOccupied(false);
        }
    }

    private void UpdateLevelText()
    {
        levelText.text = level.ToString();
    }

    public void UpdateColor()
    {
        if (levelColors.TryGetValue(level, out Color color))
        {
            if (childImage != null)
            {
                childImage.color = color;
            }
        }
    }

    public int GetLevel()
    {
        return this.level;
    }

    public void SetLevel(int newLevel)
    {
        this.level = newLevel;
        UpdateColor();
        
    }

    public void SetCell(Cell cell)
    {
        if (currentCell != null)
            currentCell.SetOccupied(false); //free previous cell

        currentCell = cell;

        if (currentCell != null)
        {
            transform.SetParent(currentCell.transform, false); //attach to new parent
            transform.localPosition = Vector3.zero; 
            currentCell.SetOccupied(true); //mark new cell as occupied
        }
    }
    public void MakeApplianceGreen()
    {
        childImage.color = Color.green;
    }
    public Cell GetCell()
    {
        return currentCell;
    }

}
