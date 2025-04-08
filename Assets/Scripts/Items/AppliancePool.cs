using System.Collections.Generic;
using UnityEngine;

public class AppliancePool : MonoBehaviour
{

    //used an hybrid pooling system to avoid destroy()-instantiate() costs
    //but also dont want to wste too much memory - thus, a used hybrid system
     public static AppliancePool Instance { get; private set; }

    [SerializeField] private GameObject appliancePrefab;
    [SerializeField] private int newApplianceCount = 10;
    [SerializeField] private int poolSize = 20;
    [SerializeField] private int limitLevelForPool = 16;

    private Queue<GameObject> appliancePool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(appliancePrefab, transform);
            obj.SetActive(false);
            appliancePool.Enqueue(obj);
        }
    }

    public GameObject GetPooledObject(int level)
    {
        GameObject obj;
        if (level <= limitLevelForPool) //we have to create and destroy mostly 2,4,8. Just dequeue them and instantiate rest of them
        {
            if (appliancePool.Count > 3)//ran out of appliances soon, create new ones
            {
                obj = appliancePool.Dequeue();
            }
            else
            {
                for (int count = 0; count < newApplianceCount; count++)
                {
                    GameObject newObj = Instantiate(appliancePrefab, transform);
                    newObj.SetActive(false);
                    appliancePool.Enqueue(newObj); 
                }
                obj = appliancePool.Dequeue();
            }

        }
        else
        {
            obj = Instantiate(appliancePrefab);
        }

        obj.SetActive(true);
        Appliance applianceScript = obj.GetComponent<Appliance>();
        applianceScript.SetLevel(level);
        return obj;
    }

    public void ReturnToPool(GameObject obj, int level)
    {
        if (level <= limitLevelForPool) //we have to create and destroy mostly 2,4,8. Just enqueue them and destroy rest of them
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            appliancePool.Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
}
