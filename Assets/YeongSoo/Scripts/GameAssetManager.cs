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
    public EquipmentAssets equipmentAssets;
    public FoodAssets foodAssets;
    public MiscAssets miscAssets;
    public BagAssets bagAssets;
    public GemAssets gemAssets;

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
            case GoogleSheetLoader.Sheets.EQUIPMENT:
                return equipmentAssets.GetEquipmentImageBySpecID(itemSpecID);
            case GoogleSheetLoader.Sheets.FOOD:
                return foodAssets.GetFoodImageBySpecID(itemSpecID);
            case GoogleSheetLoader.Sheets.MISC:
                return miscAssets.GetGemImageBySpecID(itemSpecID);
            case GoogleSheetLoader.Sheets.BAG:
                return bagAssets.GetBagImageBySpecID(itemSpecID);
            case GoogleSheetLoader.Sheets.GEM:
                return gemAssets.GetGemImageBySpecID(itemSpecID);
            default: return null;
        }
    }
}
