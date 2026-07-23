using UnityEngine;

public abstract class AbstractAttacker : IAttacker
{

    public bool alive;

    public AbstractAttacker()
    {
        alive = true;
    }

    public abstract void Attack(Vector3 origin, Vector3 forward);

    public bool isAlive()
    {
        return alive;
    }
}
