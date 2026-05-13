using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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

// Basic Human functions. This is also the human created for the controllable human
public class BaseHuman : Object {
    public BaseHuman(HumanStats stats) : base(HumanManager.Prefab, HumanManager.GetTransform(), "Human") {
        this.stats = stats;
        this.isAlive = true;
        isPunching = false;

        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        SetGender();
        SetColor();

        transform.localScale = Vector2.one * stats.size;
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

    public Rigidbody2D rigidbody;

    public HumanStats stats;

    public bool isAlive;
    public int CoarseTime;

    public bool isPunching;

    public float Speed => stats.speed;
    public float Size => stats.size;
    public float Vision => stats.vision;
    public float Strength => stats.strength;
    public float Endurance => stats.endurance;
    public float Intelligence => stats.intelligence;

    void SetColor() {
        float v = (float)CoarseTime / HumanManager.CoarpseDelayTime;
        float v1 = 1f - v;
        float v2 = v1 * 0.8f;
        SetColor(new Color(v1, v2, v2));
    }
    void SetColor(Color color) {
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
        transform.GetChild(1).GetComponent<SpriteRenderer>().color = color;
        transform.GetChild(2).GetComponent<SpriteRenderer>().color = color;
    }


    public void LookAtPos(Vector2 position) {
        LookAt(position - (Vector2)transform.position);
    }
    public void LookAt(Vector2 direction) {
        LookAt(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }
    public void LookAt(float angle) {
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle - 90f));
    }

    public virtual void Move(Vector2 movement) {
        Vector2 pos = Position;
        pos += movement.normalized * Speed * Time.fixedDeltaTime;
        rigidbody.MovePosition(pos);
    }

    const float punch_distane = 0.85f;
    const float reset_punch_hand_time = 2.5f;
    int last_hand_used = 0;
    public virtual async Task Punch(float force) {
        if (isPunching) return;
        isPunching = true;

        if (last_hand_used == 0) last_hand_used = Random.Range(1, 3);
        else last_hand_used = last_hand_used == 1 ? 2 : 1;

        Transform hand = transform.GetChild(last_hand_used).transform;
        Vector2 real_starting_position = hand.position;
        float start = hand.localPosition.y;
        float dis = start + punch_distane;
        float speed = dis * (4 * force + 1);
        while (hand.localPosition.y < dis) {
            hand.localPosition += speed * Time.deltaTime * Vector3.up;
            await Task.Yield();
        }
        PunchCollsion(real_starting_position, hand.localScale.x, force);
        while (hand.localPosition.y > start) {
            hand.localPosition -= speed * Time.deltaTime * Vector3.up;
            await Task.Yield();
        }
        Vector3 pos = hand.localPosition;
        pos.y = start;
        hand.localPosition = pos;

        isPunching = false;

        float timer = 0f;
        while (timer < reset_punch_hand_time) {
            timer += Time.deltaTime;
            if (isPunching) return;
            await Task.Yield();
        }
        last_hand_used = 0;
    }
    void PunchCollsion(Vector2 position, float size, float force) {
        float angle = transform.rotation.eulerAngles.z + 90f;
        angle *= Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        float ray_distance = 1.75f * size + punch_distane;
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        Vector2 r = new Vector2(Mathf.Cos(angle + Mathf.PI / 2f), Mathf.Sin(angle + Mathf.PI / 2f)) * size / 2f;
        hits.AddRange(Physics2D.RaycastAll(position - r, dir, ray_distance));
        hits.AddRange(Physics2D.RaycastAll(position, dir, ray_distance));
        hits.AddRange(Physics2D.RaycastAll(position + r, dir, ray_distance));

        Debug.DrawLine(position + r, position + r + dir * ray_distance, Color.red, 1);
        Debug.DrawLine(position, position + dir * ray_distance, Color.red, 1);
        Debug.DrawLine(position - r, position - r + dir * ray_distance, Color.red, 1);

        if (hits.Count == 0) return;

        Transform target = null;
        foreach (var hit in hits) {
            if (hit.transform == transform) continue;
            if (hit.transform.tag == "Item") continue;
            target = hit.transform;
            break;
        }
        if (target == null) return;

        PunchEffect(target, force);
    }
    void PunchEffect(Transform target, float force) {
        float damage = force * Strength;
        object obj = target.GetComponent<Identity>().type;
        switch (target.tag) {
            case "Tree":
                ((Tree)obj).Damage(Mathf.RoundToInt(damage));
                break;
            case "Human":
                ((Human)obj).Damage(Position, force * Strength);
                break;
        }
    }


    const int damage_animation_ms = 50;
    const float damage_force_multiplier = 10f;
    public async virtual Task Damage(Vector2 position, float amount) {
        Vector2 dir = Position - position;
        rigidbody.AddForce(amount * damage_force_multiplier * dir, ForceMode2D.Impulse);
        await DamageAnimation();
        Kill();
    }
    async Task DamageAnimation() {
        SetColor(Color.red);
        await Task.Delay(damage_animation_ms);
        SetColor();
    }

    public virtual void Kill() {
        isAlive = false;
    }
}

// A Human with a mind and "life"
public class Human : BaseHuman {
    public Human(HumanStats stats) : base(stats) {
        Brain = new HumanBrain(this);

        health = 25f * Size;
        hunger = 100f;
        energy = 100f;
        reproduction_meter = 0f;

        damage_reduction = HumanBrain.RationalFunction(Endurance, 0f, 1f, 5f);
    }

    HumanBrain Brain;

    float health;
    float hunger;
    float energy;
    float reproduction_meter;

    float damage_reduction;

    public float Hunger => hunger;
    public float Health => health;
    public float Energy => energy;
    public float ReproductionMeter => reproduction_meter;

    public override async Task Punch(float force) {
        energy -= force * force * force;
        await base.Punch(force);
    }

    public async override Task Damage(Vector2 position, float amount) {
        amount -= amount * damage_reduction;
        energy -= amount;
        health -= amount;
        await base.Damage(position, amount);
    }
}
