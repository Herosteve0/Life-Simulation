using System.Collections.Generic;
using UnityEngine;

class HumanSenses {
    public HumanSenses(Human human) {
        Instance = human;
        HumanDelta = 0;
    }

    public Human Instance;
    public List<Human> NearbyHumans;

    public int HumanDelta;

    public void CheckNearbyHumans() {
        List<Human> nearbyHumans = new List<Human>();

        int preCount = NearbyHumans.Count;
        NearbyHumans.Clear();
        foreach (Human human in nearbyHumans) {
            if (Vector2.Distance(human.Position, human.Position) > human.Vision) continue;
            NearbyHumans.Add(human);
        }
        HumanDelta += NearbyHumans.Count - preCount;
    }
}

class HumanInstinct {
    float hunger;
    float energy;
    float sexual_desire;

    public float GetHungerScore => hunger;
    public float GetEnergyScore => energy;
    public float GetSexualDesireScore => sexual_desire;
}
public enum Emotion {
    None,
    Fear,
    Boredom,
    Anger,
    Curiosity,
    Safety,
    Sorrow,
    Disgust,
    Guilt
}
class HumanEmotions : HumanInstinct {
    float fear;
    float boredom;
    float anger;
    float curiosity;
    float safety;
    float sorrow;
    float disgust;
    float guilt;

    public float GetFearScore => fear;
    public float GetBoredomScore => boredom;
    public float GetAngerScore => anger;
    public float GetCuriosityScore => curiosity;
    public float GetSafetyScore => safety;
    public float GetSorrowScore => sorrow;
    public float GetDisgustScore => disgust;
    public float GetGuiltScore => guilt;
}

public class HumanBrain {
    // https://www.desmos.com/calculator/lzo2nqma6v
    public static float RationalFunction(float x, float start_value, float tendto_value, float change_rate) {
        return (tendto_value * x + start_value * change_rate) / (x + change_rate);
    }

    public HumanBrain(Human human) {
        Instance = human;

        senseFrequency = RationalFunction(human.Intelligence, 5f, 4f, 10f);

        senseTimer = 0;
    }

    Human Instance;
    HumanSenses Senses;
    HumanEmotions Emotions;

    float senseFrequency;
    float senseTimer;

    public List<Human> NearbyHumans => Senses.NearbyHumans;
    public int HumanDelta => Senses.HumanDelta;

    public void Process() {
        senseTimer -= Time.deltaTime;
        if (senseTimer <= 0) {
            Senses.CheckNearbyHumans();
            senseTimer = senseFrequency;
        }

    }
}