using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectSelector : MonoBehaviour
{
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
            if (_highlight.CompareTag("Selectable") && _highlight != _selection && UIManager.Instance.LobbyOutlinesState())
            {
                if (_highlight.gameObject.GetComponent<Outline>() != null)
                { 
                    _highlight.gameObject.GetComponent<Outline>().enabled = true;// si ya tiene outline se activa
                    //Debug.Log("Activo outline");
                } 
                else // si no se crea
                {
                    Outline outline = _highlight.gameObject.AddComponent<Outline>();
                    outline.enabled = true;
                    _highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.green;
                    _highlight.gameObject.GetComponent<Outline>().OutlineWidth = 4.0f;
                    //Debug.Log("Creo el outline");
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
                    if (UIManager.Instance.LobbyOutlinesState())
                    {
                        switch (hit.collider.gameObject.name)
                        {
                            case "wall_doorway_scaffold_door":
                                UIManager.Instance.ShowWeaponsCanvas(); // Mostrar seleccion de arma
                                break;
                            case "WaiterTable":
                                UIManager.Instance.ShowMisionCanvas(); // Mostrar misiones
                                break;
                            case "armour":
                                UIManager.Instance.ShowUpgradesCanvas(); // Mostrar mejoras
                                break;
                            case "sword_shield_gold":
                                UIManager.Instance.ShowAchievementsCanvas(); // Mostrar logros
                                break;
                            case "selectable_objects":
                                Debug.Log("Cargar zona entrenamiento");
                                break;
                        }
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
    public void OnClose(GameObject panelToClose)
    {
        panelToClose.SetActive(false);
    }
}
