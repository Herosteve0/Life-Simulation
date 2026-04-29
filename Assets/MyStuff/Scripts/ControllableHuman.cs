using UnityEngine;
using System.Threading.Tasks;

public class ControllableHuman : MonoBehaviour {

    public static Human Instance;

    private void Awake() {
        if (Instance != null) {
            KillPlayer();
        }
        Instance = HumanManager.CreateRandom();
        Debug.Log(Instance.stats);
    }

    public void Update() {
        if (Instance == null) return;
        CheckMovement();
    }

    void CheckControls() {
        Vector2 movement = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) movement.y++;
        if (Input.GetKey(KeyCode.S)) movement.y--;

        if (Input.GetKey(KeyCode.D)) movement.x++;
        if (Input.GetKey(KeyCode.A)) movement.x--;

        Vector2 pos = (Vector2)Instance.transform.position;
        pos += movement.normalized * Instance.stats.speed * Time.deltaTime;
        Instance.gameObject.GetComponent<Rigidbody2D>().MovePosition(pos);
    }
    async Task CheckMovement() {
        if (!Input.GetMouseButtonDown(1)) return;

        Vector2 pos = CameraManager.GetMousePosition;
        await Instance.WalkTowards(pos);
    }


    public static void KillPlayer() {
        Instance.Kill();
        Instance = null;
        GameManager.isPlaying = false;
    }
}
