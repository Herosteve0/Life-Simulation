using UnityEngine;

enum Goal {
    None,
    Food,
    Explore
}

class MemoryEvent {
    public Goal type;

    public Human target; // Target involved in the memory
    public Vector2 generalLocation; // general location memory is associated with

    public Emotion emotion;
    public float emotionalWeight;
    public float memoryStrength;
}

public class HumanMemory {
    
}
