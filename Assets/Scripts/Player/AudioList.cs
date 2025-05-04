using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AudioList", menuName = "Audio/Audio List", order = 1)]
public class AudioList : ScriptableObject
{
    [Tooltip("Lista de clips de audio que se reproducirán aleatoriamente.")]
    public List<AudioClip> audioClips = new List<AudioClip>();
}