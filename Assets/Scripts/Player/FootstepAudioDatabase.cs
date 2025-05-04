using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FootstepAudioDatabase", menuName = "Audio/Footstep Audio Database", order = 2)]
public class FootstepAudioDatabase : ScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        [Tooltip("PhysicMaterial asignado al collider del suelo (p.ej. ‘Grass’, ‘Rock’, etc.)")]
        public PhysicMaterial material;

        [Tooltip("La lista de sonidos a usar sobre este material.")]
        public FootstepAudioList audioList;
    }

    [Tooltip("Cada entrada asocia un PhysicMaterial con su lista de sonidos de paso.")]
    public List<Entry> entries = new List<Entry>();

    /// <summary>
    /// Busca en entries la que coincide con el material dado;
    /// devuelve null si no encuentra.
    /// </summary>
    public FootstepAudioList GetListForMaterial(PhysicMaterial mat)
    {
        if (mat == null) return null;
        foreach (var e in entries)
            if (e.material == mat)
                return e.audioList;
        return null;
    }
}