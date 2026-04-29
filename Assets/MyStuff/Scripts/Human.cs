using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public enum Gender {
    Male,
    Female
}

public struct HumanStats {
    public HumanStats(Gender gender, float speed, float size, float vision, float strength, float stamina, float endurance, float fertility, float intelligence) {
        this.gender = gender;
        this.speed = speed;
        this.size = size;
        this.vision = vision;
        this.strength = strength;
        this.stamina = stamina;
        this.endurance = endurance;
        this.fertility = fertility;
        this.intelligence = intelligence;
    }

    // Physical
    public Gender gender;
    public float speed;
    public float size;
    public float strength;

    // Perception
    public float vision;

    // Survival
    public float stamina;
    public float endurance;

    // Others
    public float fertility;
    public float intelligence;

    public static HumanStats Average(HumanStats mother, HumanStats father) {
        return new HumanStats(
            HumanManager.GetRandomGender(),
            (mother.speed + father.speed) / 2f,
            (mother.size + father.size) / 2f,
            (mother.vision + father.vision) / 2f,
            (mother.strength + father.strength) / 2f,
            (mother.stamina + father.stamina) / 2f,
            (mother.endurance + father.endurance) / 2f,
            (mother.fertility + father.fertility) / 2f,
            (mother.intelligence + father.intelligence) / 2f
            );
    }

    public void ApplyRandomAttributes() {
        float[] distribution = Attributes(HumanManager.mutationPoints, 8, HumanManager.mutationPower);

        speed *= distribution[0];
        size *= distribution[1];
        vision *= distribution[2];
        strength *= distribution[3];
        stamina *= distribution[4];
        endurance *= distribution[5];
        fertility *= distribution[6];
        intelligence *= distribution[7];
    }
    float[] Attributes(float a, int n, float power) {
        float[] values = new float[n];
        float totalWeight = 0f;

        for (int i = 0; i < n; i++) {
            values[i] = 1 + 2 * (Random.value - 0.5f) * power;
            totalWeight += values[i];
        }

        float tmp = a * power * power / totalWeight;
        for (int i = 0; i < n; i++) {
            values[i] = (values[i] + 1f + n / totalWeight) * a;
        }

        return values;
    }

    public override string ToString() {
        return
            $"Speed = {speed}\n" +
            $"Size = {size}\n" +
            $"Vision = {vision}\n" +
            $"Strength = {strength}\n" +
            $"Stamina = {stamina}\n" +
            $"Endurance = {endurance}\n" +
            $"Fertility = {fertility}\n" +
            $"Intelligence = {intelligence}";
    }
}

public class Human {
    public Human(HumanStats stats) {
        this.stats = stats;
        this.isAlive = true;
        this.isMoving = false;

        CreateObject();
        SetGender();
        SetColor();

        transform.localScale = Vector2.one * stats.size;
    }

    void CreateObject() {
        gameObject = GameObject.Instantiate(HumanManager.Prefab, HumanManager.GetTransform(), false);
        transform = gameObject.transform;
        gameObject.name = "Human";
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        navigationAgent = gameObject.GetComponent<NavMeshAgent>();
        destPath = new NavMeshPath();
    }
    void SetGender() {
        Color color;
        float rotation;
        if (stats.gender == Gender.Male) {
            color = Color.blue;
            rotation = 0f;
        } else {
            color = Color.red;
            rotation = 180f;
        }
        transform.GetChild(3).GetComponent<SpriteRenderer>().color = color;
        transform.GetChild(3).transform.eulerAngles = new Vector3(0f, 0f, rotation);
    }

    public GameObject gameObject;
    public Transform transform;
    public Rigidbody2D rigidbody;
    public NavMeshAgent navigationAgent;

    public HumanStats stats;

    public bool isAlive;
    public int CoarseTime;

    public bool isMoving;
    NavMeshPath destPath;

    void SetColor() {
        float v = (float)CoarseTime / HumanManager.CoarpseDelayTime;
        float v1 = 1f - v;
        float v2 = v1 * 0.8f;

        Color color = new Color(v1, v2, v2);
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
        transform.GetChild(1).GetComponent<SpriteRenderer>().color = color;
        transform.GetChild(2).GetComponent<SpriteRenderer>().color = color;
    }


    public void LookAtPos(Vector2 position) {
        Vector2 pos = position - (Vector2)transform.position;
        float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        LookAt(angle);
    }
    public void LookAt(float angle) {
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle - 90f));
    }
    public void LookAt(Vector2 direction) {
        LookAt(Mathf.Atan2(direction.y, direction.x));
    }

    public async Task WalkTowards(Vector2 destination) {
        NavMesh.CalculatePath(
            transform.position,
            destination,
            NavMesh.AllAreas,
            destPath
        );
        Debug.Log(destPath.corners.Length);
        
        if (destPath.corners.Length == 0) {
            isMoving = false;
            return;
        }
        isMoving = true;

        float speed = stats.speed;

        for (int i = 0; i < destPath.corners.Length; i++) {
            Vector2 target = (Vector2)destPath.corners[i];

            while (Vector2.Distance(transform.position, target) > 0.05f) {
                Vector2 dir = (target - (Vector2)transform.position).normalized;
                LookAt(dir);
                transform.position += (Vector3)dir * speed * Time.deltaTime;
                await Task.Yield();
            }
        }

        isMoving = false;
    }

    public void Kill() {
        isAlive = false;
    }
}
