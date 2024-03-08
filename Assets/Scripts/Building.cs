using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Building : MonoBehaviour
{
    public string Name;
    //public float GlobalPollutionAmount = 0.1f;
    public float LocalPollutionAmount = 0.01f;

    //  NOTE: Pollution will only occur if we are in the "CreatureState.Placed" state
    public BuildingState CurrentState;
    public enum BuildingState { Hidden, InitialSpawn, Selected, Placed }

    public Material BaseMaterial;
    public Material HighlightMaterial;
    private Renderer[] childRenders;

    public LayerMask GroundLayer;
    private float screenX;
    private float screenY;
    private Vector3 mousePosition;
    private Vector3 placementPosition;

    private Territory localTerritory;

    private void Awake()
    {
        childRenders = GetComponentsInChildren<Renderer>();
    }

    private void Start()
    {
        ChangeBuildingState(BuildingState.InitialSpawn);
    }

    private void Update()
    {
        //  Only highlight when the building is selected?
        if (CurrentState != BuildingState.Selected)
            BuildingHighlighter(false);


        //  Grabbing the position of the mouse
        screenX = Input.mousePosition.x;
        screenY = Input.mousePosition.y;
        mousePosition = new Vector3(screenX, screenY);


        //  Draw rays from the camera down to the ground
        Ray cameraRay = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        Debug.DrawRay(cameraRay.origin, cameraRay.direction * 100f, Color.yellow);
        if (Physics.Raycast(cameraRay, out hit, Mathf.Infinity, GroundLayer))
        {
            placementPosition = hit.point;
        }


        switch (CurrentState)
        {
            case BuildingState.Hidden:
                //  When hidden, just hide the renderer and disable the ability to place the building
                //  We don't want to place buildings if we can't see them

                break;
            case BuildingState.InitialSpawn:
                //  Give the building a slight glow to show this is the building being focused

                break;
            case BuildingState.Selected:
                BuildingHighlighter(true);
                transform.position = placementPosition;
              
                break;
            case BuildingState.Placed:
                //  Maybe animate a "placing" effect
                //  Produce pollution
                if(localTerritory != null)
                {
                    localTerritory.LocalPollution += LocalPollutionAmount * Time.deltaTime;
                }

                break;
            default:
                break;
        }

    }

    public void ChangeBuildingState(BuildingState nextState)
    {
        CurrentState = nextState;
    }

    public void BuildingHighlighter(bool highlight)
    {
        //  If there's nothing in the array, just bail
        if (childRenders.Length <= 0)
            return;

        if (highlight)
        {
            //  Change the material on the child renderer to the highlight material
            for (int i = 0; i < childRenders.Length; i++)
            {
                childRenders[i].material = HighlightMaterial;
            }
        }
        else
        {
            //  Change the material back to the Base material
            for (int i = 0; i < childRenders.Length; i++)
            {
                childRenders[i].material = BaseMaterial;
            }

        }
    }

    public void SelfDestruct()
    {
        if(CurrentState == BuildingState.Selected)
        {
            Destroy(gameObject);
        }
    }

    private void OnMouseDown()
    {
        if(CurrentState == BuildingState.Selected)
        {
            //  Once a building is placed, determine it's local territory
            localTerritory = GameManager.Instance.DetermineLocalTerritory(this.transform);
            ChangeBuildingState(BuildingState.Placed);
            return;
        }

        if (CurrentState == BuildingState.Placed || CurrentState == BuildingState.InitialSpawn)
        {
            ChangeBuildingState(BuildingState.Selected);
            return;
        }
    }

    //private Territory DetermineLocalTerritory()
    //{
    //    //  Loop through the GamerManger's territories to determine the closest territory
    //    float bestDist = Mathf.Infinity;
    //    Territory bestTerr = null;

    //    for (int i = 0; i < GameManager.Instance.Territories.Count; i++)
    //    {
    //        var nextDist = GameManager.Instance.Territories[i].transform.position;
    //        if (Vector3.Distance(this.transform.position, nextDist) < bestDist)
    //        {
    //            bestDist = Vector3.Distance(this.transform.position, nextDist);
    //            bestTerr = GameManager.Instance.Territories[i];
    //        }
    //    }

    //    return bestTerr;
    //}

    private void OnEnable()
    {
        BuildingPlacer.OnDestroyBuilding += SelfDestruct;
    }

    private void OnDisable()
    {
        BuildingPlacer.OnDestroyBuilding -= SelfDestruct;
    }

}
