using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectSelector : MonoBehaviour
{
    private Camera _camera;

    private Transform _highlight;  // Objeto impactado
    private Transform _selection;  // Objeto seleccionado

    [SerializeField] GameObject Explore;
    [SerializeField] GameObject Upgrades;
    [SerializeField] GameObject Missions;
    [SerializeField] GameObject Achievements;
    [SerializeField] GameObject Training;

    void Awake()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        if (_highlight != null)
        {
            _highlight.gameObject.GetComponent<Outline>().enabled = false;
            _highlight = null;

            Explore.SetActive(false);
            Upgrades.SetActive(false);
            Missions.SetActive(false);
            Achievements.SetActive(false);
            Training.SetActive(false);
        }

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);  // Lanza un rayo desde la cámara
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit)) return;  // Si el rayo golpea algo...

        _highlight = hit.transform;
        if (_highlight.CompareTag("Selectable") && _highlight != _selection && UIManager.Instance.LobbyOutlinesState())
        {
            if (_highlight.gameObject.GetComponent<Outline>() != null)
            {
                _highlight.gameObject.GetComponent<Outline>().enabled = true;  // si ya tiene outline se activa
            }
            else
            { // si no se crea
                Outline outline = _highlight.gameObject.AddComponent<Outline>();
                outline.enabled = true;
                _highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.green;
                _highlight.gameObject.GetComponent<Outline>().OutlineWidth = 4.0f;
            }
            switch (hit.collider.gameObject.name)
            {
                case "wall_doorway_scaffold_door":
                    Explore.SetActive(true);
                    Upgrades.SetActive(false);
                    Missions.SetActive(false);
                    Achievements.SetActive(false);
                    Training.SetActive(false);
                    break;
                case "WaiterTable":
                    Missions.SetActive(true);
                    Explore.SetActive(false);
                    Upgrades.SetActive(false);
                    Achievements.SetActive(false);
                    Training.SetActive(false);
                    break;
                case "armour":
                    Upgrades.SetActive(true);
                    Explore.SetActive(false);
                    Missions.SetActive(false);
                    Achievements.SetActive(false);
                    Training.SetActive(false);
                    break;
                case "sword_shield_gold":
                    Achievements.SetActive(true);
                    Explore.SetActive(false);
                    Upgrades.SetActive(false);
                    Missions.SetActive(false);
                    Training.SetActive(false);
                    break;
                case "selectable_objects":
                    Training.SetActive(true);
                    Explore.SetActive(false);
                    Upgrades.SetActive(false);
                    Missions.SetActive(false);
                    Achievements.SetActive(false);
                    break;
            }

        }
        else
        {
            _highlight = null;
        }

        if (Input.GetMouseButtonDown(0))
        {  // Detecta clic izquierdo
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
                            Explore.SetActive(false);
                            UIManager.Instance.ShowWeaponsCanvas();  // Mostrar seleccion de arma
                            break;
                        case "WaiterTable":
                            Missions.SetActive(false);
                            UIManager.Instance.ShowMisionCanvas();  // Mostrar misiones
                            break;
                        case "armour":
                            Upgrades.SetActive(false);
                            UIManager.Instance.ShowUpgradesCanvas();  // Mostrar mejoras
                            break;
                        case "sword_shield_gold":
                            Achievements.SetActive(false);
                            UIManager.Instance.ShowAchievementsCanvas();  // Mostrar logros
                            break;
                        case "selectable_objects":
                            Training.SetActive(false);
                            GameManager.Instance.EnterTrainingScene(); // carga escena entrenamiento
                            break;
                    }
                }
            }
            else if (_selection)
            {
                _selection.gameObject.GetComponent<Outline>().enabled = false;
                _selection = null;
            }
        }
    }
    public void OnClose(GameObject panelToClose)
    {
        panelToClose.SetActive(false);
    }
}
