using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeChamberGenerator : MonoBehaviour {
  [SerializeField] private RoomType ChamberType;
  [SerializeField] private List<GameObject> chambers;

  public void createChamber(int level) {
    int TypeDesign = Random.Range(0, chambers.Count);
    GameObject instancedObject = Instantiate(chambers[TypeDesign], new Vector3(0, 0, 0), Quaternion.identity);
    instancedObject.GetComponent<DungeonController>()?.SetTrap(ChamberType == RoomType.Elite);
    instancedObject.GetComponent<IChamberController>().initiallise(level);
  }

  public RoomType getChamberType() {
    return ChamberType;
  }
}
