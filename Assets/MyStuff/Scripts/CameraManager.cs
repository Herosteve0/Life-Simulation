using UnityEngine;

public class CameraManager : MonoBehaviour {
    Camera camera;

    [SerializeField] float scrollPower = 10;
    [SerializeField] float scrollLimit = 10;
    float limitMultipler;

    [SerializeField] float cameraSmoothness = 4f;

    bool isMoving;
    Vector3 StartPos;
    Vector3 PosDelta;

    static CameraManager Instance;

    private void OnEnable() {
        Instance = this;
        camera = GetComponent<Camera>();

        transform.position = new Vector3(0f, 0f, -10f);
    }

    private void LateUpdate() {
        if (!Application.isPlaying) return;

        if (GameManager.isPlaying) PlayerCamera();
        else MainCamera();
    }

    public static Vector3 GetMousePosition => Instance.camera.ScreenToWorldPoint(Input.mousePosition);
    public static void Initialize() {
        Instance.limitMultipler = Mathf.Min(GameManager.MapSize.x, GameManager.MapSize.y) / 65f;
    }


    // Main Camera
    void MainCamera() {
        CheckMove();
        CheckZoom();
    }

    void CheckMove() {
        if (!Input.GetMouseButton(2)) {
            isMoving = false;
            return;
        }
        if (!isMoving) { 
            StartPos = GetMousePosition;
        }
        isMoving = true;

        PosDelta = GetMousePosition - transform.position;
        Vector3 pos = StartPos - PosDelta;

        Vector2 mapsize = GameManager.MapSize / 2f;
        mapsize -= new Vector2(camera.aspect, 1f) * camera.orthographicSize;
        pos.x = Mathf.Clamp(pos.x, -mapsize.x, mapsize.x);
        pos.y = Mathf.Clamp(pos.y, -mapsize.y, mapsize.y);
        transform.position = pos;
    }

    void CheckBorder() {
        Vector3 pos = transform.position;
        Vector2 mapsize = GameManager.MapSize / 2f;
        mapsize -= new Vector2(camera.aspect, 1f) * camera.orthographicSize;
        pos.x = Mathf.Clamp(pos.x, -mapsize.x, mapsize.x);
        pos.y = Mathf.Clamp(pos.y, -mapsize.y, mapsize.y);
        transform.position = pos;
    }

    void CheckZoom() {
        Vector3 preLoc = GetMousePosition;

        float size = camera.orthographicSize - Input.mouseScrollDelta.y * scrollPower;
        size = Mathf.Clamp(size, scrollLimit, scrollLimit * limitMultipler);
        camera.orthographicSize = size;

        transform.position += preLoc - GetMousePosition;
        CheckBorder();
    }

    // Player Movements
    void PlayerCamera() {
        SmoothMove();
        CameraZoom();
    }

    void SmoothMove() {
        Vector2 delta = ControllableHuman.Instance.transform.position - transform.position;
        transform.position += (Vector3)delta / cameraSmoothness;
        CheckBorder();
    }

    void CameraZoom() {
        camera.orthographicSize = ControllableHuman.Instance.Size * ControllableHuman.Instance.Vision;
    }
}
