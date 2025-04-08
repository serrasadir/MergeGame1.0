using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class Producer : MonoBehaviour, IPointerDownHandler
{
    public int capacity = 10; 
    private static int maxCapacity = 10;

    [SerializeField] private float cooldownTimer = 3f;
    [SerializeField] private GameObject producerPrefab;
    
    public Cell currentCell; //track which cell this appliance is on


    private void Start()
    {

        InvokeRepeating(nameof(AutoProduce), 4f, 4f); //belirli aralılarla çağırılr
    }

    private void AutoProduce()
    {
        if (capacity > 0)
        {
            ProduceAppliance();
        }
        else
        {
            CancelInvoke(nameof(AutoProduce));
            ReplaceProducer(this);
        }
    }

    public void ProduceAppliance()
    {
        if (capacity <= 0) return; //cant produce if capacity is 0

        Cell nearestCell = BoardManager.Instance.GetNearestEmptyCell(transform.position);
        if (nearestCell == null) return; //no cell is available

        int[] levels = { 2, 4, 8 };
        levels = levels.OrderBy(x => Random.value).ToArray(); // Listeyi karıştır
        int randomlvl = levels[0]; // İlk elemanı al

        GameObject applianceObj = AppliancePool.Instance.GetPooledObject(randomlvl);
        if (applianceObj != null)
        {
            applianceObj.transform.SetParent(nearestCell.transform, false); //attach to cell
            applianceObj.transform.localPosition = Vector3.zero; //centered
            applianceObj.SetActive(true); // activate object
            nearestCell.SetOccupied(true);//occupied the cell
        }

        capacity--;
        GameSaveManager.SaveGame();
        if (capacity <= 0)
        {
            //create a new producer at a random location
            ReplaceProducer(this);
        }

    }
    public void GenerateProducer(Cell cell)
    {
        GameObject newProducer = Instantiate(producerPrefab, cell.transform); //attach to cell
        newProducer.transform.localPosition = Vector3.zero; 
        newProducer.SetActive(true);

        Producer producerComponent = newProducer.GetComponent<Producer>();
        producerComponent.capacity = maxCapacity;
        producerComponent.SetCell(cell); 
        cell.SetOccupied(true); //mark cell as occupied

        ProducerAnimation animation = newProducer.GetComponent<ProducerAnimation>();
        animation.GenerateProducer(newProducer.transform.position);//circle animation when new producer is generated

        GameSaveManager.SaveGame();
    }


    public void ReplaceProducer(Producer oldProducer)
    {
        Cell randomEmptyCell = BoardManager.Instance.GetRandomEmptyCell();
        if (randomEmptyCell != null)
        {
            GenerateProducer(randomEmptyCell);
            ProducerAnimation animation = oldProducer.GetComponent<ProducerAnimation>();
            animation.DestroyProducer(oldProducer.transform.position);//explosion animation when old producer is destroyed

            oldProducer.currentCell.SetOccupied(false);//set free the cell
            Destroy(oldProducer.gameObject);
        }
        GameSaveManager.SaveGame();
    }

    public void SetCell(Cell cell)
    {
        if (currentCell != null)
            currentCell.SetOccupied(false); //free previous cell


        currentCell = cell;
        if (currentCell != null)
            currentCell.SetOccupied(true); //mark new cell as occupied

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ProduceAppliance();
    }

}

