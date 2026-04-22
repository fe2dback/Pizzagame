using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class InventoryData
{
    public List<string> items = new List<string>();
}

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public List<string> items = new List<string>();
    string savePath;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Application.persistentDataPath + "/inventory.json";
        LoadInventory();
    }

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        SaveInventory();
        Debug.Log(itemName + " 저장됨 (현재 총 개수: " + items.Count + ")");
    }

    // ⭐ 특정 아이템이 몇 개 있는지 확인하는 함수
    public int GetItemCount(string itemName)
    {
        int count = 0;
        foreach (string item in items)
        {
            if (item == itemName) count++;
        }
        return count;
    }

    public void LoadInventory()
    {
        if (!File.Exists(savePath)) return;
        string json = File.ReadAllText(savePath);
        InventoryData data = JsonUtility.FromJson<InventoryData>(json);
        items = data.items;
        Debug.Log("인벤토리 로드 완료");
    }

    public void SaveInventory()
    {
        InventoryData data = new InventoryData();
        data.items = items;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
    }

    public void ResetInventory()
    {
        items.Clear();
        SaveInventory();
        Debug.Log("인벤토리 초기화 완료");
    }
}