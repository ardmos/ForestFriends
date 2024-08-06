using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpecLoaderForTestDeletable : MonoBehaviour
{
    private async void Start()
    {
        // 테스트.

        // 1. 구글시트에서 아이템 스펙 정보를 로드한다.
        await ItemSpecManager.LoadItemSpecs();

        // 2. 로드한 정보를 바탕으로 
    }
}
