using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField] Vector2 mapSize = new Vector2(250, 250);
    [SerializeField] Transform MapBackground;

    [SerializeField] int StartingTrees = 40;
    [SerializeField] int StartingHumans = 10;
    
    public static GameManager Instance;

    public static bool isPlaying = false;

    private void OnEnable() {
        Instance = this;

        MapBackground.position = Vector2.zero;
        MapBackground.localScale = mapSize;

        isPlaying = false;
        ResetSimulation();
    }
    async void ResetSimulation() {
        await Task.Delay(1);

        ResetWorld();
    }

    public static Vector2 MapSize => Instance.mapSize;

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
    }

    public void PlaySimulation() {
        isPlaying = true;
        Human human = HumanManager.CreateRandom();
        human.transform.AddComponent<ControllableHuman>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            if (!isPlaying) PlaySimulation();
            else ControllableHuman.KillPlayer();
        }
    }
}
