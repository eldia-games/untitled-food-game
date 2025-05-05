using UnityEngine;

public class DungeonMinimap : MonoBehaviour {
  [SerializeField] private GameObject minimapCamera;
  [SerializeField] private PlayerCombat player;
  [SerializeField] private float bossCameraScale;
  private GameManager gameManager;

  void Start() {
    gameManager = GameManager.Instance;
    minimapCamera.transform.SetParent(player.transform);
    minimapCamera.transform.localPosition = new Vector3(0, 20, 0);
    minimapCamera.GetComponent<Camera>().orthographicSize = 60f;

    if (gameManager.room == RoomType.Boss) 
      minimapCamera.GetComponent<Camera>().orthographicSize *= bossCameraScale;
  }
}
