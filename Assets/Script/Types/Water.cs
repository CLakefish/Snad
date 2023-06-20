using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : Particle
{
    public override bool CheckPos(int x, int y, int[,] Map, Particle[,] mapObjects)
    {
        if (y - 1 < 0 || x + 1 > ParticleHandler.mapWidth - 1 || x - 1 < 0) return false;

        if (Map[x, y - 1] == 0)
        {
            Map[x, y - 1] = Map[x, y];
            Map[x, y] = 0;

            mapObjects[x, y - 1] = this;
            mapObjects[x, y] = null;

            MoveParticle(new Vector2Int(x, y - 1));

            return true;
        }
        else
        {
            bool right = false,
                 left = false;

            if (Map[x + 1, y] == 0) right = true;
            if (Map[x - 1, y] == 0) left = true;

            if (!right && !left)
            {
                if (Map[x - 1, y - 1] == 0)
                {
                    Map[x - 1, y - 1] = Map[x, y];
                    Map[x, y] = 0;

                    mapObjects[x - 1, y - 1] = this;
                    mapObjects[x, y] = null;

                    MoveParticle(new Vector2Int(x - 1, y - 1));

                    return true;
                }
                else if (Map[x + 1, y - 1] == 0)
                {
                    Map[x + 1, y - 1] = Map[x, y];
                    Map[x, y] = 0;

                    mapObjects[x + 1, y - 1] = this;
                    mapObjects[x, y] = null;

                    MoveParticle(new Vector2Int(x + 1, y - 1));

                    return true;
                }

                return false;
            }

            Map[x + (right ? 1 : -1), y] = Map[x, y];
            mapObjects[x + (right ? 1 : -1), y] = this;

            Map[x, y] = 0;
            mapObjects[x, y] = null;

            MoveParticle(new Vector2Int(x + (right ? 1 : -1), y));
            return true;
        }
        //throw new System.NotImplementedException();
    }

    public override void MoveParticle(Vector2Int position)
    {
        transform.position = new Vector3(position.x, position.y, 0f);
        //throw new System.NotImplementedException();
    }
}
