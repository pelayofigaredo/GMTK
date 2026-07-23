using UnityEngine;

public class MissileLauncher : AbstractAttacker
{
    public static readonly GameObject prefab = Resources.Load<GameObject>("Missile");

    public override void Attack(Vector3 origin, Vector3 forward)
    {
        GameObject missile = GameObject.Instantiate(prefab, origin, Quaternion.identity);
        missile.transform.forward = forward;
    } 
}
