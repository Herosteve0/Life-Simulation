using UnityEngine;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class ControllableHuman : MonoBehaviour {

    public static Human Instance;
    Vector2 movement;

    private void Awake() {
        if (Instance != null) {
            KillPlayer();
        }
    }

    public void Update() {
        if (Instance == null) return;
        CheckControls();
    }
    public void FixedUpdate() {
        if (Instance == null) return;
        SetPosition();
    }

    void CheckControls() {
        movement = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) movement.y++;
        if (Input.GetKey(KeyCode.S)) movement.y--;

        if (Input.GetKey(KeyCode.D)) movement.x++;
        if (Input.GetKey(KeyCode.A)) movement.x--;

        if (Input.GetMouseButtonDown(0)) Instance.Punch(1f);
    }
    void SetPosition() {
        Instance.Move(movement);
        Instance.LookAtPos(CameraManager.GetMousePosition);
    }

    public static void KillPlayer() {
        if (Instance == null) return;
        Instance.Kill();
        Instance = null;
        GameManager.isPlaying = false;
    }
}
