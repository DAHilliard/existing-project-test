using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Creature : MonoBehaviour
{
    public string Name;
    public float MoveSpeed = 3f;
    public float MoveRadius = 1f;
    public float MoveTimer = 10f;
    public bool IsMoving;
    private Vector3 nextMoveLocation;
    private Vector3 refVelocity;

    //  Evolutions
    public CreatureState CurrentState;
    public enum CreatureState { Ground, Sea, Flying, Evolving }
    public GameObject[] EvolutionPrefabs;

    //  We can read the pollutions levels directly from myTerritory.LocalPollution
    Territory myTerritory;
    private float readLocalP;
    private float readGlobalP;
    public float GroundLPMax, GroundGPMax;
    public float SeaLPMax, SeaGPMax;

    //  Item drops
    public float ItemDropTimer = 5f;
    public GameObject[] ItemDrops;  //  Index matches with the CreatureState index
    public GameObject ItemContainer;

    private void Start()
    {
        myTerritory = GameManager.Instance.DetermineLocalTerritory(this.transform);

        //  Do a manual call on the visuals updater
        ChangeCreatureStates(CreatureState.Ground);
        UpdateVisuals(CurrentState);

        //  We're NOT moving at the start
        //  and trigger a waiting function
        IsMoving = false;

        //  Start the ItemDrop Enum
        ItemContainer = GameObject.FindGameObjectWithTag("Item Container");
        if (ItemContainer != null)
            StartCoroutine(ItemDropCycle());
    }

    private void Update()
    {
        //  Bookkeeping the pollution levels
        readLocalP = myTerritory.LocalPollution;
        readGlobalP = GameManager.Instance.GlobalPollution;

        if (IsMoving == false)
        {
            StartCoroutine(WalkCycle());
        }

        this.transform.position = Vector3.SmoothDamp(this.transform.position, nextMoveLocation, ref refVelocity, MoveSpeed);

        //   Determine thresholds
        PollutionThresholds(readLocalP, readGlobalP);

    }

    private IEnumerator WalkCycle()
    {
        nextMoveLocation = DetermineMoveLocation();
        yield return new WaitForSeconds(MoveTimer);
        IsMoving = false;
    }

    private IEnumerator ItemDropCycle()
    {
        yield return new WaitForSeconds(ItemDropTimer);
        if (ItemDrops[(int)CurrentState] != null)
            Instantiate(ItemDrops[(int)CurrentState], this.transform.position, Quaternion.identity, ItemContainer.transform);

        //  Restart the cycle after an Item drop
        StartCoroutine(ItemDropCycle());
    }

    private Vector3 DetermineMoveLocation()
    {
        //  From the center of the myTerritory transform
        var terrTransform = myTerritory.transform.position;

        //  pick a random spot along spot X and spot Z away from the center of myTerritory transform
        var randX = Random.Range(-MoveRadius, MoveRadius);
        var randZ = Random.Range(-MoveRadius, MoveRadius);
        var newMoveVector = new Vector3(terrTransform.x + randX, 0f, terrTransform.z + randZ);

        //  Once a newMoveVector is found, we are assumed to be moving
        IsMoving = true;

        return newMoveVector;
    }

    private void PollutionThresholds(float localP, float globalP)
    {
        if (localP <= GroundLPMax && globalP <= GroundGPMax)
        {
            ChangeCreatureStates(CreatureState.Ground);
        }
        else if (localP > GroundLPMax && localP <= SeaLPMax)

        {
            ChangeCreatureStates(CreatureState.Sea);
        }
        else if (localP > SeaLPMax && globalP > SeaGPMax)
        {
            ChangeCreatureStates(CreatureState.Flying);
        }
    }

    private void ChangeCreatureStates(CreatureState newState)
    {
        //  Just ignore this if we are passing the same state
        if (newState == CurrentState) return;

        //  Otherwise change the states
        CurrentState = newState;
        UpdateVisuals(newState);
    }

    private void UpdateVisuals(CreatureState current)
    {
        //   take the current state and use it to index the evolutionprefabs[]
        for (int i = 0; i < EvolutionPrefabs.Length; i++)
        {
            if (i == (int)current)
            {
                //  If we find the same index, just ignore for now
                continue;
            }
            else
            {
                //  Else we are going to deactivate the visuals
                EvolutionPrefabs[i].SetActive(false);
            }
        }
        //  Activate the correct visuals here
        EvolutionPrefabs[(int)current].SetActive(true);

    }
}
