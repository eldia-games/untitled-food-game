using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {
  [SerializeField] private List<GameObject> road;
  // [SerializeField] private Vector2 tileOffset;
  [SerializeField] private Vector2Int mapSize;

  private Transform transform_;
  [SerializeField] private List<GameObject> map_;

  // Start is called before the first frame update
  void Start() {
    transform_ = transform;
    map_ = new List<GameObject>(mapSize.x * mapSize.y);
    
    GameObject template = road[0];
    Vector2 tileOffset = new Vector2(Mathf.Cos(Mathf.PI / 3.0f) * 2, Mathf.Sin(Mathf.PI / 3.0f));
    for (int i = 0; i < mapSize.x * mapSize.y; ++i) {
      Vector2Int index = new Vector2Int(i % mapSize.x, i / mapSize.x);
      Vector3 position = new Vector3(index.y * tileOffset.y, 0, index.x * tileOffset.x) * 2;
      if (index.y % 2 == 1) position += new Vector3(0, 0, tileOffset.x);
      GameObject tile = Instantiate(template, position, template.transform.rotation, transform_);
      tile.SetActive(true);
      map_.Add(tile);
    }
  }

  // Update is called once per frame
  void Update() {
    
  }
}
