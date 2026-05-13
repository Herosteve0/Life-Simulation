using UnityEngine;
using System.Collections.Generic;

public class HumanManager : MonoBehaviour {

    static List<Human> HumanList;

    public float DistributionPoints = 1f;
    public float DistributionPower = 0.1f;

    public int rotTime = 60;

    public GameObject HumanPrefab;
    static HumanManager Instance;

    private void OnEnable() {
        Instance = this;
        HumanList = new List<Human>();
    }
    private void OnDisable() {
        ResetHumans();
    }

    public static GameObject Prefab => Instance.HumanPrefab;
    public static float mutationPoints => Instance.DistributionPoints;
    public static float mutationPower => Instance.DistributionPower;
    public static int CoarpseDelayTime => Instance.rotTime;

    public static Transform GetTransform() {
        return Instance.transform;
    }

    public static Gender GetRandomGender() {
        if (Random.value >= 0.5f) return Gender.Male;
        return Gender.Female;
    }

    public static Human CreateRandom() {
        HumanStats stats = new HumanStats(
            GetRandomGender(),
            Random.Range(3f, 11f), // speed
            Random.Range(0.75f, 2f), // size
            Random.Range(8f, 17f), // vision
            Random.Range(3f, 10f), // strength
            Random.Range(10f, 45f), // stamina
            Random.Range(0f, 5f), // endurance
            Random.Range(0f, 2f), // fertility
            Random.Range(2f, 10f) // intelligence
            );

        Human human = new Human(stats);

        float x = (Random.value - 0.5f) * GameManager.MapSize.x;
        float y = (Random.value - 0.5f) * GameManager.MapSize.y;
        human.transform.position = new Vector2(x, y);

        return SettupCreation(human);
    }
    public static Human Create(Human mother, Human father) {
        HumanStats basestats = HumanStats.Average(mother.stats, father.stats);
        basestats.ApplyRandomAttributes();

        Human human = new Human(basestats);

        Vector3 pos = mother.transform.position + father.transform.position;
        pos.z = 0f;
        human.transform.position = pos / 2f;

        return SettupCreation(human);
    }
    static Human SettupCreation(Human human) {
        HumanList.Add(human);
        return human;
    }

    public static void Delete(Human human) {
        HumanList.Remove(human);
        Destroy(human.gameObject);
    }

    public static void ResetHumans() {
        if (HumanList == null) return;

        Human[] humans = HumanList.ToArray();
        foreach (Human human in humans) {
            Delete(human);
        }

        HumanList.Clear();
    }
}
