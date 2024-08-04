using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpecLoaderForTestDeletable : MonoBehaviour
{
    private async void Start()
    {
        await ItemSpecManager.LoadItemSpecs();
    }
}
