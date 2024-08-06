using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 확인 완료. SO 사용 방식으로 변경 차례
/// </summary>
public class GameAssetManager : MonoBehaviour 
{
    public static GameAssetManager Instance { get; private set; }

    public WeaponAssets weaponAssets;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
