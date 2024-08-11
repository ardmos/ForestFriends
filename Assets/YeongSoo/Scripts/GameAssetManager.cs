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
    public BagAssets bagAssets;

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

    public Sprite GetItemImageBySpecID(GoogleSheetLoader.Sheets sheetName, int itemSpecID)
    {
        switch (sheetName)
        {
            case GoogleSheetLoader.Sheets.WEAPON:
                return weaponAssets.GetWeaponImageBySpecID(itemSpecID);
            case GoogleSheetLoader.Sheets.BAG:
                return bagAssets.GetBagImageBySpecID(itemSpecID);
            default: return null;
        }
    }
}
