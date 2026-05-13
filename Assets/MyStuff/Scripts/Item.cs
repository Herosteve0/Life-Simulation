using UnityEngine;

public enum ItemTypes {
    Wood,
    Apple,
    Acorn
}

public class Item : Object {
    public Item(ItemTypes type) : base(ItemManager.Prefab, ItemManager.GetTransform(), "Item") {
        this.type = type;

        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        gameObject.GetComponent<SpriteRenderer>().sprite = GetSprite(type);
    }

    public Rigidbody2D rigidbody;
    public Vector2 velocity;
    public ItemTypes type;

    static Sprite GetSprite(ItemTypes type) {
        switch (type) {
            case ItemTypes.Wood: return ItemManager.Textures[0];
            case ItemTypes.Apple: return ItemManager.Textures[1];
            case ItemTypes.Acorn: return ItemManager.Textures[2];
        }
        return null;
    }
}
