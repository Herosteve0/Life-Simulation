using System.Collections.Generic;
using UnityEngine;

public class Identity : MonoBehaviour {
    public object type;
}

public class Object {
    public Object(GameObject prefab, Transform parent, string name) {
        gameObject = GameObject.Instantiate(prefab, parent, false);
        transform = gameObject.transform;
        gameObject.name = name;
        gameObject.AddComponent<Identity>().type = this;
    }

    public GameObject gameObject;
    public Transform transform;

    public Vector2 Position => (Vector2)transform.position;
}