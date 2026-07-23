using UnityEngine;

public interface IAttacker
{
    void Attack(Vector3 origin, Vector3 direction);

    bool isAlive();
}
