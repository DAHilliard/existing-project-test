using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    //  Have a list/ or array of buildings that we can select
    public Building[] Buildings;
    public List<Building> ChildScripts;
    public GameObject BuildingContainer;

    public static Action OnDestroyBuilding;
    public static BuildingPlacer Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(this);
    }


    public void CreateBuilding(int buildingIndex)
    {
        //  Have the UIManager show which building is currently selected on this script

        var newBuilding = Instantiate(Buildings[buildingIndex].gameObject, Vector3.zero, Quaternion.identity, BuildingContainer.transform);
        ChildScripts.Add(newBuilding.GetComponent<Building>());
    }

    public void DestroyBuilding()
    {
        if (ChildScripts.Count == 0)
        {
            Debug.LogWarning("No buildings exist");
            return;
        }
        else
        {
            //  Destroy the selected buildings
            OnDestroyBuilding?.Invoke();

            //  Clear the ChildScripts
            ChildScripts.Clear();

            //  Recreate the ChildScripts list
            var restructure = BuildingContainer.GetComponentsInChildren<Building>();
            foreach (var building in restructure)
            {
                ChildScripts.Add(building);
            }

        }


    }

  
}
