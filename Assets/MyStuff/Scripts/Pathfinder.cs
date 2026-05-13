using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

class Node {
    public Vector2 position;
    public List<Node> neighbors = new List<Node>();

    public float gCost;
    public float hCost;
    public Node parent;
    
    public float fCost => gCost + hCost;

    public Node(Vector2 pos) {
        position = pos;
    }

    public void AddNeighbor(Node node) {
        this.neighbors.Add(node);
        node.neighbors.Add(this);
    }
}

class NodePack {
    public List<Node> Pack;

    public void Add(Node node) {
        Pack.Add(node);
    }
    public void Add(Node[] nodes) {
        Pack.AddRange(nodes);
    }
    public void Add(List<Node> nodes) {
        Pack.AddRange(nodes);
    }

    public List<Vector2> GetPositions() {
        List<Vector2> r = new List<Vector2>();

        foreach (Node node in Pack) {
            r.Add(node.position);
        }

        return r;
    }
}

public class Pathfinder {
    public Pathfinder(Human human) {
        playerSize = human.Size;
    }

    float margin = 0.05f;
    int vision_quality = 3;
    int point_quality = 8;

    float playerSize;

    Vector2[] PointLineIntersectingCircle(float direction, float radius) {
        Vector2 r = new Vector2(Mathf.Cos(direction), Mathf.Sin(direction)) * radius;
        return new Vector2[]{r, -r};
    }

    Vector2[] PointsAroundCircle(Vector2 pos, float radius) {
        Vector2[] r = new Vector2[point_quality];

        float n = 2f * Mathf.PI / point_quality;
        for (int i = 0; i < point_quality; i++) {
            float a = n * i;
            r[i] = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius;
        }

        return r;
    }

    public List<Vector2> GeneratePositions(Vector2 origin, Vector2 destination) { 
        return GeneratePositions(origin, destination, new List<Tree>());
    }
    public List<Vector2> GeneratePositions(Vector2 origin, Vector2 destination, List<Tree> treeCalc) {
        float direction = Mathf.Atan2((destination.y - origin.y), (destination.x - origin.x));
        Vector2 dir = new Vector2(Mathf.Cos(direction), Mathf.Sin(direction));
        float distance = Vector2.Distance(origin, destination);
        LayerMask mask = LayerMask.GetMask("Tree");

        Vector2[] new_points = PointLineIntersectingCircle(direction + Mathf.PI / 2f, playerSize);

        List<Vector2> points = new List<Vector2>();
        points.Add(Vector2.zero);
        for (int i = 0; i < vision_quality - 1; i++) {
            points.AddRange(PointLineIntersectingCircle(direction + Mathf.PI / 2f, playerSize * (i + 1) / vision_quality));
        }
        if (vision_quality > 0) points.AddRange(new_points);

        Tree objecthit = null;
        foreach (Vector2 point in points) {
            RaycastHit2D hit = Physics2D.Raycast(origin + point, dir, distance, mask);
            Debug.DrawLine(origin + point, origin + point + distance * dir, Color.white, 1);
            if (hit) {
                objecthit = hit.transform.GetComponent<Tree>();
                break;
            }
        }

        List<Vector2> r = new List<Vector2> { origin, destination };

        if (objecthit != null) {
            r.AddRange(PointsAroundCircle(objecthit.Position, objecthit.Size));
        }

        return r;
    }

    public List<Vector2> GeneratePath(Vector2 origin, Vector2 destination) {
        NodePack pack = new NodePack();
        

        
        return pack.GetPositions();
    }
}
