using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ItemManager : MonoBehaviour {

    static List<Item> ItemList;
    static List<Item> inMotion;

    public GameObject ItemPrefab;
    static ItemManager Instance;
    [SerializeField] Sprite[] ItemTextures;

    private void OnEnable() {
        Instance = this;
        ItemList = new List<Item>();
    }
    private void OnDisable() {
        ResetItems();
    }

    public static GameObject Prefab => Instance.ItemPrefab;
    public static Sprite[] Textures => Instance.ItemTextures;

    public static Transform GetTransform() {
        return Instance.transform;
    }

    public static Item[] Create(ItemTypes type, int amount, Vector2 position, bool velocity = false) {
        Item[] items = new Item[amount];
        for (int i = 0; i < amount; i++) {
            items[i] = Create(type, position, velocity);
        }
        return items;
    }
    public static Item Create(ItemTypes type, Vector2 position, bool velocity = false) {
        Item item = new Item(type);
        ItemList.Add(item);
        if (velocity) position += Random.insideUnitCircle;
        item.transform.position = position;
        return item;
    }

    public static void Delete(Item item) {
        ItemList.Remove(item);
        Destroy(item.gameObject);
    }

    public static void ResetItems() {
        if (ItemList == null) return;

        Item[] items = ItemList.ToArray();
        foreach (Item item in items) {
            Delete(item);
        }

        ItemList.Clear();
    }
}
