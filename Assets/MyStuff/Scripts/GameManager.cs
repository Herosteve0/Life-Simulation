using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField] int seed = 50;

    [SerializeField] Vector2 mapSize = new Vector2(250, 250);
    [SerializeField] Transform MapBackground;

    [SerializeField] int StartingTrees = 40;
    [SerializeField] int StartingHumans = 10;
    
    public static GameManager Instance;

    const float minMapSize = 250;

    public static Vector2 MapSize;
    public static bool isPlaying = false;

    private void OnEnable() {
        Instance = this;

        ResetSimulation();
    }
    public async void ResetSimulation() {
        if (Instance == null) return;
        await Task.Delay(1);

        ControllableHuman.KillPlayer();

        Random.InitState(seed);
        if (mapSize.magnitude < minMapSize) {
            mapSize = Vector2.one * minMapSize / Mathf.Sqrt(2);
        }
        MapSize = mapSize;

        CameraManager.Initialize();
        MapBackground.position = Vector2.zero;
        MapBackground.localScale = MapSize;

        ResetWorld();
    }

    public void ResetWorld() {
        Debug.Log("Resetting world.");

        TreeManager.ResetTrees();
        for (int i = 0; i < StartingTrees; i++) {
            TreeManager.CreateRandom();
        }

        HumanManager.ResetHumans();
        for (int i = 0; i < StartingHumans; i++) {
            HumanManager.CreateRandom();
        }

        ItemManager.ResetItems();
    }

    public void PlaySimulation() {
        isPlaying = true;
        Human human = HumanManager.CreateRandom();
        human.transform.AddComponent<ControllableHuman>();
        ControllableHuman.Instance = human;
        Debug.Log(human.stats);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            if (!isPlaying) PlaySimulation();
            else ControllableHuman.KillPlayer();
        }
    }
}
