using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private int rows = 8;
    [SerializeField] private int cols = 8;
    
    [SerializeField] private int numbersOfInitialProducers = 5;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject producerPrefab;

    [SerializeField] private Transform boardParent; //BoardPanel
    private Cell[,] boardCells;

    private bool isGameOver = false;

    public static BoardManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private IEnumerator InitializeExistingBoard() //if there is a saved game
    {
        UIManager.Instance.ShowLoadingScreen(); //oading screen

        GenerateBoard(); //create the board first
        yield return new WaitForSeconds(0.5f); //wait 0.5 seconds for loading
        Debug.Log("Game is loading from an existing save.");
        GameSaveManager.LoadGame(); //load the game on start

        UIManager.Instance.HideLoadingScreen(); //hide after loading
    }

    private void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/gameSave.json"))
        {
            StartCoroutine(InitializeExistingBoard());
        }
        else
        {
            GenerateBoard();
            PlaceInitialProducers(numbersOfInitialProducers);
            UIManager.Instance.HideLoadingScreen(); 
        }
    }
    private void GenerateBoard()
    {

        boardCells = new Cell[cols, rows];

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GameObject cellObj = Instantiate(cellPrefab, boardParent);
                cellObj.transform.SetParent(boardParent, false); //keeps the hierarchy clean by keeping the cells in the panel

                Cell cell = cellObj.GetComponent<Cell>();
                cell.Setup(x, y);
                boardCells[x, y] = cell;
            }
        }
    }

    private void PlaceInitialProducers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Cell randomEmptyCell = GetRandomEmptyCell();
            if (randomEmptyCell != null)
            {
                GameObject newProducer = Instantiate(producerPrefab);
                newProducer.transform.SetParent(randomEmptyCell.transform, false); //attach to the cell
                newProducer.transform.localPosition = Vector3.zero; //center inside the cell
                newProducer.SetActive(true);
                newProducer.GetComponent<Producer>().SetCell(randomEmptyCell);
            }
        }
        GameSaveManager.SaveGame();
    }

    public void ClearBoard()
    {
        foreach (Transform child in boardParent) //iterate through all children of the board panel
        {
            foreach (Transform grandChild in child)
            {
                if (grandChild.TryGetComponent<Producer>(out Producer producer))
                {
                    Destroy(producer.gameObject);
                }
                else if (grandChild.TryGetComponent<Appliance>(out Appliance appliance))
                {
                    AppliancePool.Instance.ReturnToPool(appliance.gameObject, appliance.GetLevel()); //return appliances to pool
                }
            }

        }

        //reset all cell occupancy
        foreach (Cell cell in boardCells)
        {
            cell.SetOccupied(false);
        }

        Debug.Log("cleared");
        GameSaveManager.SaveGame();

    }
    public void RetryGame()
    {
        isGameOver = false;
        UIManager.Instance.HideGameOverScreen();
        Debug.Log("Restarting game...");
        RemoveAllProducers(); // somehow producers cannot be removed properly, so i had to be persistent. thats why i add this function
        ClearBoard();

        TaskManager.Instance.RetryTasks();

        GameSaveManager.DeleteSaveData();

        InventoryManager.Instance.ClearInventory();


        PlaceInitialProducers(numbersOfInitialProducers);
        Debug.Log("After restarting");
        GameSaveManager.SaveGame();
    }

   

    public Cell GetNearestEmptyCell(Vector2 position)
    {
        Cell nearestCell = null;
        float minDistance = float.MaxValue;
        bool thereAreEmptyCells = false;
        foreach (Cell cell in boardCells)
        {
            if (!cell.IsOccupied) //only check unoccupied cells
            {
                float distance = Vector2.Distance(position, cell.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestCell = cell;
                    thereAreEmptyCells = true;
                }
            }
        }

        if (!thereAreEmptyCells)
        {
            Debug.Log("Game over");
            isGameOver = true;
            UIManager.Instance.ShowGameOverScreen();
        }
        return nearestCell;
    }

    public Cell GetRandomEmptyCell()
    {
        List<Cell> emptyCells = new List<Cell>();

        foreach (Cell cell in boardCells)
        {
            if (!cell.IsOccupied)
                emptyCells.Add(cell);
        }

        if (emptyCells.Count == 0)
        {
            return null;
        }

        return emptyCells[Random.Range(0, emptyCells.Count)];
    }


    public Cell GetCell(int x, int y)
    {
        if (x >= 0 && x < cols && y >= 0 && y < rows)
        {
            return boardCells[x, y]; //return the cell at the given coordinates
        }
        return null; //return null if out of bounds
    }

    public GameObject GetProducerPrefab()
    {
        return producerPrefab;
    }


    public void RemoveAllProducers()//to destroy all the producers properly
    {

        List<GameObject> producersToDestroy = new List<GameObject>();

        //iterate through all board cells to find producers
        foreach (Cell cell in boardCells)
        {
            Producer producer = cell.GetComponentInChildren<Producer>();
            if (producer != null)
            {
                producersToDestroy.Add(producer.gameObject);
                cell.SetOccupied(false); // Mark the cell as empty
            }
        }

        //destroy all producers
        foreach (GameObject producer in producersToDestroy)
        {
            Destroy(producer);
        }

        GameSaveManager.SaveGame();
    }


}
