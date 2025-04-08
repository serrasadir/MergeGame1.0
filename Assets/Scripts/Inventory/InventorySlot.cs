using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerDownHandler
{
    public Appliance StoredAppliance { get; private set; }
    public bool IsOccupied { get; private set; }

    //store an appliance in this slot
    public void StoreAppliance(Appliance appliance)
    {
        if (!IsOccupied) // Only store if the slot is empty
        {
            StoredAppliance = appliance;
            appliance.transform.SetParent(transform, false);
            SetOccupied(true);
        }
    }

    //remove appliance from slot
    public void RemoveAppliance()
    {
        if (StoredAppliance != null)
        {
            InventoryManager.Instance.RemoveFromInventory(StoredAppliance);
            StoredAppliance = null;
            SetOccupied(false);
        }
    }
    public void DestroyAppliance()
    {
        Destroy(StoredAppliance.gameObject);
        SetOccupied(false);
    }
    //place appliance back on the board when clicked
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Click on a slot");
        if (StoredAppliance != null)
        {
            InventoryManager.Instance.PlaceApplianceBackOnBoard(StoredAppliance);
            RemoveAppliance();
            SetOccupied(false);
        }
    }
    public void SetOccupied(bool occupied)
    {
        IsOccupied = occupied;
    }

 }
