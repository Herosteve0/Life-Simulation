using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {

    static List<Item> ItemList;

    static ItemManager Instance;
    [SerializeField] Sprite[] ItemTextures;

    private void OnEnable() {
        Instance = this;
        ItemList = new List<Item>();
    }
    private void OnDisable() {
        ResetItems();
    }

    public static Sprite[] Textures => Instance.ItemTextures;

    public static Item Create(ItemTypes type, Vector2 position) {
        Item item = new Item(type);
        ItemList.Add(item);
        return item;
    }

    public static void Delete(Item item) {
        ItemList.Remove(item);
        Destroy(item.gameObject);
    }

    public void ResetItems() {
        if (ItemList == null) return;

        Item[] items = ItemList.ToArray();
        foreach (Item item in items) {
            Delete(item);
        }

        ItemList.Clear();
    }
}
