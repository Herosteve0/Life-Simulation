using UnityEngine;

public enum ItemTypes {
    Wood
}

public class Item {
    public Item(ItemTypes type) {
        this.type = type;

        CreateObject();
    }

    void CreateObject() {
        gameObject = GameObject.Instantiate(TreeManager.Prefab, TreeManager.GetTransform(), false);
        transform = gameObject.transform;
        gameObject.name = "Item";
    }

    public GameObject gameObject;
    public Transform transform;

    public ItemTypes type;

    static Sprite GetSprite(ItemTypes type) {
        switch (type) {
            case ItemTypes.Wood: return ItemManager.Textures[0];
        }
        return null;
    }
}
