using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;

        if (draggedObject == null || !draggedObject.GetComponent<Draggable>()) return;

        GameSaveManager.SaveGame();
        //trigger animations 
    }

}
