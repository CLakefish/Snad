using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : Particle
{
    public override bool CheckPos(int x, int y, int[,] Map, Particle[,] mapObjects)
    {
        return false;
        //throw new System.NotImplementedException();
    }

    public override void MoveParticle(Vector2Int position)
    {
        return;
        //throw new System.NotImplementedException();
    }
}
