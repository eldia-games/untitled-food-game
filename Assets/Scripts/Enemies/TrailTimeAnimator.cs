using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailTimeAnimator : MonoBehaviour
{
    TrailRenderer trail;
    Material trailMat;
    public float noiseSpeed = 0.5f;

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        trailMat = trail.material;
    }

    void Update()
    {
        // Asume que el shader usa _Time.y para desplazar ruido:
        float t = Time.time * noiseSpeed;
        trailMat.SetFloat("_TimeY", t);
        // Si el shader no tiene _TimeY, crea una propiedad en él:
        // float _TimeY; y úsala en el UV del noise.
    }
}