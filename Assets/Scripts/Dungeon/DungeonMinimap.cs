using UnityEngine;

public class DungeonMinimap : MonoBehaviour
{
    [SerializeField] private GameObject _minimapCamera;
    [SerializeField] private GameObject player;
    [SerializeField] private float bossCameraScale;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        _minimapCamera.transform.SetParent(player.transform);
        _minimapCamera.transform.localPosition = new Vector3(0, 20, 0);
        _minimapCamera.GetComponent<Camera>().orthographicSize = 60f;

        if (gameManager.room == RoomType.Boss)
        {
            _minimapCamera.GetComponent<Camera>().orthographicSize *= bossCameraScale;
        }
    }
}
