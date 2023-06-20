using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravel : Particle
{
    public override bool CheckPos(int x, int y, int[,] Map, Particle[,] mapObjects)
    {
        if (y - 1 < 0 || x - 1 < 0 || x + 1 >= ParticleHandler.mapWidth) return false;

        if (Map[x, y - 1] == 0)
        {
            Map[x, y - 1] = Map[x, y];
            Map[x, y] = 0;

            mapObjects[x, y - 1] = this;
            mapObjects[x, y] = null;

            MoveParticle(new Vector2Int(x, y - 1));

            return true;
        }
        else if (Map[x - 1, y - 1] == 0)
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
        else if (mapObjects[x, y - 1] != null && mapObjects[x, y - 1].type == ParticleType.Water)
        {
            Particle objectUnderneath = mapObjects[x, y - 1];

            mapObjects[x, y - 1] = this;
            mapObjects[x, y] = objectUnderneath;

            objectUnderneath.MoveParticle(new Vector2Int(x, y));
            MoveParticle(new Vector2Int(x, y - 1));

            return true;
        }
        else if (mapObjects[x - 1, y - 1] != null && mapObjects[x - 1, y - 1].type == ParticleType.Water)
        {
            Particle objectUnderneath = mapObjects[x - 1, y - 1];

            mapObjects[x - 1, y - 1] = this;
            mapObjects[x, y] = objectUnderneath;

            objectUnderneath.MoveParticle(new Vector2Int(x, y));
            MoveParticle(new Vector2Int(x - 1, y - 1));

            return true;
        }
        else if (mapObjects[x + 1, y - 1] != null && mapObjects[x + 1, y - 1].type == ParticleType.Water)
        {
            Particle objectUnderneath = mapObjects[x + 1, y - 1];

            mapObjects[x + 1, y - 1] = this;
            mapObjects[x, y] = objectUnderneath;

            objectUnderneath.MoveParticle(new Vector2Int(x, y));
            MoveParticle(new Vector2Int(x + 1, y - 1));

            return true;
        }
        else
        {
            return false;
        }

        //throw new System.NotImplementedException();
    }

    public override void MoveParticle(Vector2Int position)
    {
        transform.position = new Vector3(position.x, position.y, 0f);
    }
}
