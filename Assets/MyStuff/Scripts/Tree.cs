using UnityEditor;
using UnityEngine;

public class Tree {
    public Tree(float size) {
        health = CalculateMaxHealth(size);

        CreateObject();
        transform.localScale = Vector2.one * size;

    }

    void CreateObject() {
        gameObject = GameObject.Instantiate(TreeManager.Prefab, TreeManager.GetTransform(), false);
        transform = gameObject.transform;
        gameObject.name = "Tree";
    }

    public GameObject gameObject;
    public Transform transform;
    int health;

    public static int CalculateMaxHealth(float size) { 
        return (int)(100 * Mathf.Pow(2f, 4 * (size - 1))); 
    }

    public void Damage(int damage) {
        health -= damage;
        if (health <= 0) Destroy();
    }

    public void Destroy() {
        TreeManager.Delete(this);
    }
}
