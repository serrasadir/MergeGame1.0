using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private GameObject inventoryPanel; //the popup panel
    [SerializeField] private GameObject inventorySlotPrefab; //prefab for inventory slots
    [SerializeField] private Transform inventoryContent; // parent object for inventory slots
    [SerializeField] private int initialSlotCount = 9;
    private int slotCount = 9;
    [SerializeField] private int additionalSlotCount = 3;
    [SerializeField] private ScrollRect inventoryScrollRect;

    private List<GameObject> inventorySlots = new List<GameObject>(); //list of slots
    private List<Appliance> appliancesInInventory = new List<Appliance>(); //list of stored appliances

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitializeInventory();
        CloseInventory();
    }

    private void InitializeInventory()
    {
        
        for (int i = 0; i < initialSlotCount; i++)
        {
            AddNewSlot();
        }
    }

    private void AddNewSlot()
    {
        GameObject newSlot = Instantiate(inventorySlotPrefab, inventoryContent);
        inventorySlots.Add(newSlot);
        slotCount++;

        Canvas.ForceUpdateCanvases();

        RectTransform contentRect = inventoryContent.GetComponent<RectTransform>();
        GridLayoutGroup grid = inventoryContent.GetComponent<GridLayoutGroup>();

        if (grid != null)
        {
            int rowCount = slotCount / 3;
            float newHeight = rowCount * (grid.cellSize.y); //calculate the height
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, newHeight);
        }
        ScrollToBottom();

    }



    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);
        ScrollToBottom();
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
    }

    public GameObject AddToInventory(Appliance appliance)
    {
        foreach (GameObject slot in inventorySlots)
        {
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();

            if (!inventorySlot.IsOccupied) //find an empty slot
            {
                appliance.GetComponent<Draggable>().canvasGroup.blocksRaycasts = false;
                inventorySlot.StoreAppliance(appliance);
                appliancesInInventory.Add(appliance);
                return slot;
            }
        }

        //if all slots are full expand inventory
        ExpandInventory();
        return AddToInventory(appliance); //try again after expansion
    }

    private void ExpandInventory()
    {
        for (int i = 0; i < additionalSlotCount; i++)
        {
            AddNewSlot();
        }
    }
    public Transform GetInventoryContentTransform()
    {
        return inventoryContent; //returns the inventory UI panel where items should be placed
    }

    public void ClearInventory()
    {
        foreach(GameObject slot in inventorySlots)
        {
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
            if(inventorySlot.IsOccupied)
            {

                inventorySlot.DestroyAppliance();
            }
        }
        appliancesInInventory.Clear();
    }

    public void RemoveFromInventory(Appliance appliance)
    {
        appliancesInInventory.Remove(appliance);
    }

    public void PlaceApplianceBackOnBoard(Appliance appliance)
    {
        Cell emptyCell = BoardManager.Instance.GetNearestEmptyCell(Vector2.zero); //find first empty cell
        if (emptyCell != null)
        {
            appliance.SetCell(emptyCell);
            appliance.GetComponent<Draggable>().canvasGroup.blocksRaycasts = true; //first it was false to prevent disable clicking slots
            RemoveFromInventory(appliance);
        }
        else
        {
            Debug.LogWarning("No empty cell available!");
        }
    }

    public void RestoreInventory(List<int> applianceLevels)
    {

        foreach (int level in applianceLevels)
        {
            GameObject newApplianceObj = AppliancePool.Instance.GetPooledObject(level);
            Appliance newAppliance = newApplianceObj.GetComponent<Appliance>();
            newAppliance.SetLevel(level);
            newAppliance.GetComponent<Draggable>().canvasGroup.blocksRaycasts = false;
            AddToInventory(newAppliance);
            newAppliance.gameObject.SetActive(true);
        }
    }

    private void ScrollToBottom()
    {
        if (inventoryScrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            inventoryScrollRect.verticalNormalizedPosition = 0f; //scroll to the bottom
           
        }
    }
    

}
