using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GruntController : MonoBehaviour
{
    [Tooltip("Referencia al ScriptableObject que contiene la lista de clips")]
    public AudioList audioList;

    private AudioSource audioSource;

    void Awake()
    {
        // Aseguramos que haya un AudioSource
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Reproduce un clip aleatorio de la lista.
    /// </summary>
    public void PlayRandomGrunt()
    {
        if (audioList == null || audioList.audioClips.Count == 0)
        {
            Debug.LogWarning("GruntController: no hay clips en AudioList");
            return;
        }
        // Escogemos uno aleatoriamente
        var clip = audioList.audioClips[Random.Range(0, audioList.audioClips.Count)];
        audioSource.PlayOneShot(clip);
    }
}