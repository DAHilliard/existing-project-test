using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemCounter : MonoBehaviour
{
    private TextMeshProUGUI[] ItemTextCounts;

    private void Start()
    {
        ItemTextCounts = GetComponentsInChildren<TextMeshProUGUI>();
    }

    public void UpdateCounters(int itemID, string itemName)
    {
        ItemTextCounts[0].text = "Fiber: " + GameManager.Instance.FiberAmount;
        ItemTextCounts[1].text = "Sea Stones: " + GameManager.Instance.SeaStoneAmount;
        ItemTextCounts[2].text = "Dragon Scales: " + GameManager.Instance.DragonScaleAmount;

    }

}
