using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FootstepAudioList", menuName = "Audio/Footstep Audio List", order = 1)]
public class FootstepAudioList : ScriptableObject
{
    [Tooltip("Lista de clips de audio que se reproducirán aleatoriamente al hacer un paso.")]
    public List<AudioClip> footstepClips = new List<AudioClip>();
}