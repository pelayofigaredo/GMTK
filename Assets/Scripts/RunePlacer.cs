using UnityEngine;

public class RunePlacer : AbstractAttacker
{
    public static readonly GameObject prefab = Resources.Load<GameObject>("RuneMine");

    public override void ActualAttack(Vector3 origin, Vector3 forward)
    {
        GameObject mine = GameObject.Instantiate(prefab, origin+forward*2, Quaternion.identity);
    }
}
