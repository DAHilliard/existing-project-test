using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject TerritoryContainer;
    public List<Territory> Territories;
    public float GlobalPollution;

    //  Grab the Global pollution once every minute or so
    public float GlobalPollutionCountTimer = 60f;
    private bool isWaitingForNextCall;

    //  Item counters
    public ItemCounter theItemCounter;
    public int FiberAmount;
    public int SeaStoneAmount;
    public int DragonScaleAmount;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PopulateTerritoryList();
        isWaitingForNextCall = false;

    }

    private void Update()
    {
        //  Pollution Tester
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.LogAssertion("TESTING CALLED ON THE GAMEMANAGER UPDATE: ");
            foreach (var territory in Territories)
            {
                territory.LocalPollution += 1000f;
                GlobalPollution += 25000f;
            }
        }

        //  Updates the GlobalPollution every *** seconds
        if (isWaitingForNextCall == false)
        {
            StartCoroutine(UpdateGlobalPollution());
        }
    }

    public void IncreaseItemAmount(int itemID, string itemName)
    {
        switch (itemID)
        {
            case 0:
                FiberAmount++;
                break;
            case 1:
                SeaStoneAmount++;
                break;
            case 2:
                DragonScaleAmount++;
                break;
            default:
                Debug.Log("Couldn't identify the passed item.");
                break;
        }
        theItemCounter.UpdateCounters(itemID, itemName);
    }


    public Territory DetermineLocalTerritory(Transform testingObject)
    {
        //  Loop through the GamerManger's territories to determine the closest territory
        float bestDist = Mathf.Infinity;
        Territory bestTerr = null;

        for (int i = 0; i < GameManager.Instance.Territories.Count; i++)
        {
            var nextDist = GameManager.Instance.Territories[i].transform.position;
            if (Vector3.Distance(testingObject.position, nextDist) < bestDist)
            {
                bestDist = Vector3.Distance(testingObject.position, nextDist);
                bestTerr = GameManager.Instance.Territories[i];
            }
        }

        return bestTerr;
    }

    private void PopulateTerritoryList()
    {
        Territories.Clear();

        var temp = TerritoryContainer.GetComponentsInChildren<Territory>();
        foreach (var territory in temp)
        {
            Territories.Add(territory);
        }
    }

    private IEnumerator UpdateGlobalPollution()
    {
        isWaitingForNextCall = true;

        foreach (var territory in Territories)
        {
            GlobalPollution += territory.LocalPollution;
            Mathf.RoundToInt(GlobalPollution);
        }

        yield return new WaitForSeconds(GlobalPollutionCountTimer);
        isWaitingForNextCall = false;
    }

    private void OnEnable()
    {
        Item.OnItemPickupCallback += IncreaseItemAmount;
    }

    private void OnDisable()
    {
        Item.OnItemPickupCallback -= IncreaseItemAmount;
    }

}
