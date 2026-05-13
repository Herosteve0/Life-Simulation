using UnityEngine;
using System.Collections.Generic;

public class TreeManager : MonoBehaviour {

    static List<Tree> TreeList;

    public GameObject TreePrefab;
    static TreeManager Instance;

    private void OnEnable() {
        Instance = this;
        TreeList = new List<Tree>();
    }
    private void OnDisable() {
        ResetTrees();
    }
    
    public static GameObject Prefab => Instance.TreePrefab;
    public static List<Tree> AllTrees => TreeList;

    public static Transform GetTransform() {
        return Instance.transform;
    }

    public static Tree CreateRandom() {
        float x = (Random.value - 0.5f) * GameManager.MapSize.x;
        float y = (Random.value - 0.5f) * GameManager.MapSize.y;
        float size = Random.Range(0.5f, 3f);
        return Create(new Vector2(x, y), size);
    }
    public static Tree Create(Vector2 position, float size) {
        Tree tree = new Tree(size);
        tree.transform.position = position;
        TreeList.Add(tree);
        return tree;
    }

    public static void Delete(Tree tree) {
        TreeList.Remove(tree);
        Destroy(tree.gameObject);
    }

    public static void ResetTrees() {
        if (TreeList == null) return;

        Tree[] trees = TreeList.ToArray();
        foreach (Tree tree in trees) {
            Delete(tree);
        }

        TreeList.Clear(); 
    }
}
