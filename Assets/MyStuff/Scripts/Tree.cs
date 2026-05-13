using UnityEngine;

public class Tree : Object {
    public Tree(float size) : base(TreeManager.Prefab, TreeManager.GetTransform(), "Tree") {
        health = CalculateMaxHealth(size);
        transform.localScale = Vector2.one * size;
    }

    int health;

    public float Size => transform.localScale.x;

    public static int CalculateMaxHealth(float size) { 
        return (int)(100 * Mathf.Pow(2f, 4 * (size - 1))); 
    }

    public void Damage(int damage) {
        health -= damage;
        if (health <= 0) Destroy();
    }

    public void Destroy() {
        TreeManager.Delete(this);

        int woodamount = Random.Range(
            Mathf.Min(1, (int)(3*Size)),
            2*(int)Mathf.Pow(Size + 1, 4)
            );
        ItemManager.Create(ItemTypes.Wood, woodamount, Position, true);

        int acornamount = Random.Range(
            Mathf.CeilToInt(Size),
            5 * Mathf.CeilToInt(Size * Size)
            );
        ItemManager.Create(ItemTypes.Acorn, acornamount, Position, true);
    }
}
