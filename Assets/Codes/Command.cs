using UnityEngine;
using System.Collections;


public class Command
{
    public virtual bool execute(GameObject obj)
    { return false; }
}
 
public class PlayerJumpCommand : Command
{
    public override bool execute(GameObject obj)
    {
        PlayerController pc = obj.GetComponent<PlayerController>();
        if (pc != null)
        {
            return pc.DoJump();
        }

        return false;
    }
}

public class PlayerChangeColorCommand : Command
{
    public override bool execute(GameObject obj)
    {
        PlayerController pc = obj.GetComponent<PlayerController>();
        if (pc != null)
        {
            return pc.DoChangeColor();
        }

        return false;
    }
}
