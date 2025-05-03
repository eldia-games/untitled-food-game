using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootstepController : MonoBehaviour
{
    [Header("Partículas de polvo al pisar")]
    public ParticleSystem footstepDustPrefab;
    [Header("Base de datos de sonidos por terreno")]
    [Tooltip("Referencia al ScriptableObject que mapea PhysicMaterials a listas de pasos.")]
    public FootstepAudioDatabase database;

    [Header("Opcional: lista por defecto si no encuentra material")]
    public FootstepAudioList defaultList;    
    [Tooltip("Altura máxima para detectar el suelo.")]
    public float raycastDistance = 1.2f;
    [Tooltip("Cuánto volumen tendrá el sonido de paso.")]
    [Range(0f, 1f)]
    public float stepVolume = 0.5f;

    public AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region "Footstep"

    /// <summary>
    /// Llama a este método desde tu animador o lógica de movimiento cuando el enemigo da un paso.
    /// </summary>
    public void PlayFootstep()
    {
        // Raycast hacia abajo para detectar el suelo
        if (Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, out RaycastHit hit, raycastDistance))
        {
            // Intentamos extraer el physic material
            PhysicMaterial mat = hit.collider.sharedMaterial;
            FootstepAudioList list = database.GetListForMaterial(mat) ?? defaultList;

            if (list == null || list.footstepClips.Count == 0)
            {
                Debug.LogWarning($"No hay clips asignados para material '{mat?.name}'", this);
                return;
            }

            // Reproducir uno al azar
            int idx = Random.Range(0, list.footstepClips.Count);
            AudioClip clip = list.footstepClips[idx];
            _audioSource.clip = clip;
            _audioSource.volume = stepVolume;
            _audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Raycast de pasos no detectó suelo.", this);
        }
    }

    public virtual void FootstepAnimationEvent()
    {
        PlayFootstep();
        footstepDustPrefab?.Play();
    }

    #endregion
}
