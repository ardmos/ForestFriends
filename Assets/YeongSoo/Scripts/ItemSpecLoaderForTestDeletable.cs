using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpecLoaderForTestDeletable : MonoBehaviour
{
    private async void Start()
    {
        // �׽�Ʈ.

        // 1. ���۽�Ʈ���� ������ ���� ������ �ε��Ѵ�.
        await ItemSpecManager.LoadItemSpecs();

        // 2. �ε��� ������ �������� 
    }
}
