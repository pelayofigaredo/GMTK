using System.Collections.Generic;
using UnityEngine;

public class SnakeBody : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] Transform head;
    [Tooltip("Solo los huesos de la serpiente, en orden desde la cabeza hacia la cola.")]
    [SerializeField] Transform[] bones;
    [SerializeField] SkinnedMeshRenderer smr;     

    [Header("Simulación")]
    [SerializeField, Range(0f, 1f)] float stiffness = 0.6f;
    [SerializeField, Range(0f, 1f)] float damping = 0.92f;
    [SerializeField] int iterations = 6;
    [SerializeField, Range(1f, 180f)] float maxBend = 30f;

    Vector3[] pos, prev;
    float[] restLengths;
    Quaternion[] bindRot;     
    Quaternion[] boneOffset;   
    Vector3 headTailLocal;    


    //#region DEBUG_DRIVER  // <-- borrar entero cuando ya no haga falta

    //[Header("Debug driver (WASD)")]
    //[SerializeField] float debugMoveSpeed = 4f;     // unidades/seg
    //[SerializeField] float debugTurnSpeed = 720f;   // grados/seg al reorientar

    //void Update()   // mueve la cabeza; la simulación del cuerpo va en LateUpdate
    //{
    //    float x = 0f, z = 0f;
    //    if (Input.GetKey(KeyCode.W)) z += 1f;
    //    if (Input.GetKey(KeyCode.S)) z -= 1f;
    //    if (Input.GetKey(KeyCode.D)) x += 1f;
    //    if (Input.GetKey(KeyCode.A)) x -= 1f;

    //    Vector3 input = new Vector3(x, 0f, z);
    //    if (input.sqrMagnitude < 1e-6f) return;      // sin tecla: no mover ni girar

    //    Vector3 dir = input.normalized;

    //    head.position += dir * q * Time.deltaTime;

    //    Quaternion target = Quaternion.LookRotation(dir, Vector3.up);
    //    head.rotation = Quaternion.RotateTowards(
    //        head.rotation, target, debugTurnSpeed * Time.deltaTime);
    //}

    //#endregion

    void Start()
    {
        if (smr) smr.updateWhenOffscreen = true;

        int n = bones.Length;
        pos = new Vector3[n];
        prev = new Vector3[n];
        restLengths = new float[n];
        bindRot = new Quaternion[n];

        for (int i = 0; i < n; i++)
        {
            pos[i] = prev[i] = bones[i].position;
            bindRot[i] = bones[i].rotation;
            if (i > 0)
                restLengths[i] = Vector3.Distance(bones[i].position, bones[i - 1].position);
        }

        Vector3 headTail = pos[1] - pos[0];
        headTailLocal = Quaternion.Inverse(head.rotation) *
                        (headTail.sqrMagnitude > 1e-8f ? headTail.normalized : head.forward);

        // offsets: transportamos el frame por la cadena en bind pose y guardamos
        // la diferencia con la rotación real de cada hueso
        boneOffset = new Quaternion[n];
        Quaternion frame = head.rotation;
        Vector3 prevF = Forward(pos, 0, head.forward);
        boneOffset[0] = Quaternion.Inverse(frame) * bindRot[0];
        for (int i = 1; i < n; i++)
        {
            Vector3 f = Forward(pos, i, prevF);
            frame = Quaternion.FromToRotation(prevF, f) * frame;
            boneOffset[i] = Quaternion.Inverse(frame) * bindRot[i];
            prevF = f;
        }
    }

    void LateUpdate()
    {
        int n = pos.Length;
        float maxRad = maxBend * Mathf.Deg2Rad;

        // 1) inercia
        for (int i = 1; i < n; i++)
        {
            Vector3 vel = (pos[i] - prev[i]) * damping;
            prev[i] = pos[i];
            pos[i] += vel;
        }

        Vector3 headDir = SafeNormalize(head.rotation * headTailLocal, head.forward);

        // 2) restricción de distancia (exacta) + ángulo
        for (int it = 0; it < iterations; it++)
        {
            pos[0] = head.position;
            for (int i = 1; i < n; i++)
            {
                Vector3 desiredDir = (i == 1)
                    ? headDir
                    : SafeNormalize(pos[i - 1] - pos[i - 2], headDir);

                Vector3 curDir = SafeNormalize(pos[i] - pos[i - 1], desiredDir);
                Vector3 clamped = Vector3.RotateTowards(desiredDir, curDir, maxRad, 0f).normalized;

                // blend SOLO de dirección; la distancia se fija exacta -> no hay acortamiento
                Vector3 dir = Vector3.Slerp(curDir, clamped, stiffness);
                dir = SafeNormalize(dir, clamped);
                pos[i] = pos[i - 1] + dir * restLengths[i];
            }
        }
        pos[0] = head.position;

        // 3) volcar al esqueleto con transporte de rotación (roll estable)
        Quaternion frame = head.rotation;
        Vector3 prevF = Forward(pos, 0, head.forward);
        bones[0].position = pos[0];
        bones[0].rotation = frame * boneOffset[0];
        for (int i = 1; i < n; i++)
        {
            bones[i].position = pos[i];
            Vector3 f = Forward(pos, i, prevF);
            frame = Quaternion.FromToRotation(prevF, f) * frame;
            bones[i].rotation = frame * boneOffset[i];
            prevF = f;
        }
    }

    // dirección "hacia adelante" (tailward) del hueso i
    static Vector3 Forward(Vector3[] p, int i, Vector3 fallback)
    {
        Vector3 d = (i < p.Length - 1) ? p[i + 1] - p[i] : p[i] - p[i - 1];
        return SafeNormalize(d, fallback);
    }

    static Vector3 SafeNormalize(Vector3 v, Vector3 fallback)
        => v.sqrMagnitude > 1e-10f ? v.normalized : fallback;
}