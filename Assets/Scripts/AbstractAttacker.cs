using UnityEngine;

public abstract class AbstractAttacker : IAttacker
{

    public bool alive;
    private int uses;

    public AbstractAttacker()
    {
        alive = true;
    }

    public void Attack(Vector3 origin, Vector3 forward)
    {
        uses++;
        ActualAttack(origin, forward);
    }

    public abstract void ActualAttack(Vector3 origin, Vector3 forward);

    public int GetUses()
    {
        return uses;
    }

    public bool isAlive()
    {
        return alive;
    }

    public void Die()
    {
        alive = false;
    }
}
