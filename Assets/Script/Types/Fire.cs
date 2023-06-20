using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : Particle
{
    [SerializeField] GameObject SmokeParticle;
    internal bool placed = false;
    int lifetime = 40;

    public override bool CheckPos(int x, int y, int[,] Map, Particle[,] mapObjects)
    {
        if (y - 1 < 0 || x - 1 < 0 || x + 1 >= ParticleHandler.mapWidth)
        {
            Map[x, y] = 0;
            mapObjects[x, y] = null;
            Destroy(gameObject);

            return false;
        };

        if (Map[x, y - 1] == 0)
        {
            lifetime--;

            if (lifetime <= 0)
            {
                Map[x, y] = 0;
                mapObjects[x, y] = null;
                Destroy(gameObject);

                return false;
            }

            Map[x, y - 1] = Map[x, y];
            Map[x, y] = 0;

            mapObjects[x, y - 1] = this;
            mapObjects[x, y] = null;

            MoveParticle(new Vector2Int(x, y - 1));

            return true;
        }
        else if (Map[x, y - 1] == 1)
        {
            switch (mapObjects[x, y - 1].type)
            {
                case (ParticleType.Wood):

                    Map[x, y] = 0;
                    if (mapObjects[x, y - 1] != null) Destroy(mapObjects[x, y - 1].gameObject, 0.5f);

                    GameObject fire = Instantiate(gameObject, new Vector3(x, y, 0), Quaternion.identity);
                    fire.GetComponent<Fire>().placed = true;
                    mapObjects[x, y] = fire.GetComponent<Fire>();

                    if (Random.Range(0, 3) == 1) CheckPos(x, y - 1, Map, mapObjects);

                    return true;

                case (ParticleType.Fire):

                    if (Random.Range(0, 3) != 1)
                    {
                        Map[x, y] = 0;

                        mapObjects[x, y] = null;
                        Destroy(gameObject);

                        return false;
                    }

                    break;

                case (ParticleType.Water):

                    Map[x, y] = Map[x, y - 1] = 0;

                    Destroy(mapObjects[x, y - 1].gameObject);

                    mapObjects[x, y - 1] = null;

                    mapObjects[x, y] = Instantiate(SmokeParticle, new Vector3(x, y + 1, 0), Quaternion.identity).GetComponent<Particle>();


                    Destroy(gameObject);

                    return false;
            }
        }

        return false;
        //throw new System.NotImplementedException();
    }

    public override void MoveParticle(Vector2Int position)
    {
        transform.position = new Vector3(position.x, position.y, 0f);
    }

    private void Start()
    {
        Destroy(gameObject, (placed ? Random.Range(0, 3) : 0.5f));
    }
}
