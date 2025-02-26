using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectSelector : MonoBehaviour
{
    public GameObject upgrades; 
    public GameObject missions; 
	private Camera _camera;

	void Awake(){
		_camera = Camera.main;
	}

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detecta clic izquierdo
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition); // Lanza un rayo desde la cámara
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // Si el rayo golpea algo...
            {
                Debug.Log("Clic en: " + hit.collider.gameObject.name); // Para verificar qué objeto tocamos

                switch (hit.collider.gameObject.name)
                {
                    case "wall_doorway_scaffold_door":
                        //SceneManager.LoadScene("GameScene"); // Cargar la escena del juego real
                        break;
                    case "WaiterTable":
                        missions.SetActive(true);
                        break;
                    case "banner_triple_yellow":
                        upgrades.SetActive(true);// Mostrar menú de misiones
                        break;
                }
            }
        }
    }
}
