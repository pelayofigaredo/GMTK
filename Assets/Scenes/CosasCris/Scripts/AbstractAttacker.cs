using UnityEngine;

public abstract class AbstractAttacker : IAttacker
{

    public bool alive;

    public AbstractAttacker()
    {
        alive = true;
    }

    public abstract void Attack(Vector3 origin, Vector3 direction);

    public bool isAlive()
    {
        return alive;
    }
}
