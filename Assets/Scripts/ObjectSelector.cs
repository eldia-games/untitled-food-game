using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectSelector : MonoBehaviour
{
    public GameObject upgrades; 
    public GameObject missions; 
    public GameObject achivements; 
    public GameObject panel;
	private Camera _camera;

    private Transform _highlight; //objeto impactado
    private Transform _selection; //objeto seleccionado

    void Awake(){
		_camera = Camera.main;
	}

    void Update()
    {
        if (_highlight != null)
        {
            _highlight.gameObject.GetComponent<Outline>().enabled = false;
            _highlight = null;
        }

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition); // Lanza un rayo desde la cámara
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // Si el rayo golpea algo...
        {
            _highlight = hit.transform;
            if (_highlight.CompareTag("Selectable") && _highlight != _selection && panel.activeSelf)
            {
                if (_highlight.gameObject.GetComponent<Outline>() != null) _highlight.gameObject.GetComponent<Outline>().enabled = true; // si ya tiene outline se activa
                else // si no se crea
                {
                    Outline outline = _highlight.gameObject.AddComponent<Outline>();
                    outline.enabled = true;
                    _highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.green;
                    _highlight.gameObject.GetComponent<Outline>().OutlineWidth = 4.0f;
                }
            }
            else
            {
                _highlight = null;
            }

            if (Input.GetMouseButtonDown(0)) // Detecta clic izquierdo
            {
                if (_highlight)
                {

                    if (_selection != null)
                    {
                        _selection.gameObject.GetComponent<Outline>().enabled = false;
                    }
                    _selection = hit.transform;
                    _selection.gameObject.SetActive(true);
                    _highlight = null;

                    switch (hit.collider.gameObject.name)
                    {
                        case "wall_doorway_scaffold_door":
                            //SceneManager.LoadScene("GameScene"); // Cargar seleccion de arma
                            if (panel.activeSelf)
                            {
                                panel.SetActive(false);
                                Debug.Log("Cargar seleccion de arma");
                                panel.SetActive(true);
                            }
                            break;
                        case "WaiterTable":
                            if (panel.activeSelf)
                            {
                                panel.SetActive(false);
                                missions.SetActive(true);// Mostrar misiones
                            }
                            break;
                        case "armour":
                            if (panel.activeSelf)
                            {
                                panel.SetActive(false);
                                upgrades.SetActive(true);// Mostrar mejoras
                            }
                            break;
                        case "sword_shield_gold":
                            if (panel.activeSelf)
                            {
                                panel.SetActive(false);
                                achivements.SetActive(true);// Mostrar logros
                            }
                            break;
                        case "selectable_objects":
                            if (panel.activeSelf)
                            {
                                panel.SetActive(false);
                                Debug.Log("Cargar zona entrenamiento");
                                panel.SetActive(true);
                            }
                            break;
                    }
                }
                else
                {
                    if (_selection)
                    {
                        _selection.gameObject.GetComponent<Outline>().enabled = false;
                        _selection = null;
                    }
                }
            }
        }
    }
    public void onClose(GameObject panelToClose)
    {
        panelToClose.SetActive(false);
        panel.SetActive(true);
    }
}
