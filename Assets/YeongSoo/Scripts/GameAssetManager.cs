using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Ȯ�� �Ϸ�. SO ��� ������� ���� ����
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
