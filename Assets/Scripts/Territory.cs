using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Territory : MonoBehaviour
{
    public float LocalPollution;
    public float DissapationRate = 0.05f;

    private void Update()
    {
       LocalPollution = (LocalPollution > 0 ) ? LocalPollution -= (DissapationRate * Time.deltaTime) : 0;
    }


}
