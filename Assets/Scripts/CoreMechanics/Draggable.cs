using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentBeforeDrag;
    public CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentBeforeDrag = transform.parent; //store original parent
        transform.SetParent(transform.root, true);
        canvasGroup.blocksRaycasts = false; //allow drop detection
        canvasGroup.alpha = 0.7f; //make it slightly transparent when dragging - visual purposes
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        Appliance draggedAppliance = GetComponent<Appliance>();
        GameObject targetObject = eventData.pointerEnter; //the object the appliance was dropped on
        Cell targetCell = targetObject != null ? targetObject.GetComponentInParent<Cell>() : null;

        Producer existingProducer = targetCell != null ? targetCell.GetComponentInChildren<Producer>() : null;


        if (targetObject != null && targetObject.name == "InventoryText") //if dropped on inventory button
        {
            Debug.Log("Dropped on Inventory");

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OpenInventory(); //activate the inventory so we can attach the appliance on it

                GameObject addedToInventory = InventoryManager.Instance.AddToInventory(draggedAppliance);

                if (addedToInventory != null) //deattach the appliance from the cell then attach it to the inventory slot 
                {

                    Cell previousCell = parentBeforeDrag.GetComponent<Cell>();
                    if (previousCell != null)
                    {
                        previousCell.SetOccupied(false);
                    }

                    draggedAppliance.transform.SetParent(addedToInventory.transform, false);
                    draggedAppliance.transform.localPosition = Vector3.zero; //ensure it stays within slot
                    GameSaveManager.SaveGame();
                    InventoryManager.Instance.CloseInventory();
                    canvasGroup.alpha = 1.0f;
                    return;

                }
            }
        }
        else if (targetCell != null) //if dropped onto a valid board cell
        {
            
            Appliance existingAppliance = targetCell.GetComponentInChildren<Appliance>();

            if (existingAppliance != null) // There is another appliance on this cell
            {
                if (draggedAppliance.GetLevel() == existingAppliance.GetLevel()) // Merge if levels match
                {
                    existingAppliance.MergeAppliances(draggedAppliance);
                    Cell previousCell = parentBeforeDrag.GetComponent<Cell>();
                    if (previousCell != null)
                    {
                        previousCell.SetOccupied(false);
                    }


                }
                else //,f they dont match, reset position
                {
                    Debug.Log("Levels do not match.");
                    ResetPosition();
                }
            }
            else if (existingProducer != null)//producer on the cell
            {
                ResetPosition();
            }
            else //if the target cell is empty
            {
                Cell previousCell = parentBeforeDrag.GetComponent<Cell>();
                if (previousCell != null)
                {
                    previousCell.SetOccupied(false);
                }

                transform.SetParent(targetCell.transform, false);
                transform.localPosition = Vector3.zero;
                targetCell.SetOccupied(true);
            }
        }
        else //dropped outside a valid cell
        {
            Debug.LogWarning("Dropped outside the board.");
            ResetPosition();
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1.0f;
    }

    private void ResetPosition()
    {
        transform.SetParent(parentBeforeDrag, false);
        transform.localPosition = Vector3.zero;
    }


}
