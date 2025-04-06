using UnityEngine;

public class DungeonMinimap : MonoBehaviour
{
    [SerializeField] private GameObject _minimapCamera;
    [SerializeField] private GameObject player;

    void Start()
    {
        _minimapCamera.transform.SetParent(player.transform);
        _minimapCamera.transform.localPosition = new Vector3(0, 20, 0);
    }
}
