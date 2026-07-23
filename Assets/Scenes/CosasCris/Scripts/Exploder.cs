using UnityEngine;

public class Exploder : AbstractAttacker
{
    public static readonly GameObject prefabExplosion = Resources.Load<GameObject>("ExplosionCercana");

    public override void Attack(Vector3 origin, Vector3 forward)
    {
        GameObject.Instantiate(prefabExplosion, origin, Quaternion.identity);
    } 
}
