using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //  Item identity
    public string Name;
    public int ItemID;

    //  Send out a message claiming that I've been picked up
    public static Action<int, string> OnItemPickupCallback;

    private void Start()
    {
        Name = this.name;
    }

    private void OnMouseDown()
    {
        // Collect this item
        //  Destroy this item
        OnItemPickupCallback?.Invoke(ItemID, Name);
        Destroy(gameObject);
    }
}
