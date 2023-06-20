using System.Collections;
using System.Collections.Generic; using UnityEditor;
using UnityEngine;

public class ParticleHandler : MonoBehaviour
{
    [System.Serializable]
    public struct Object
    {
        public Vector3Int center;
        public Vector3Int size;
        public Color color;
        public Particle type;
    }

    [Header("Map Values")]
    [SerializeField] public int[,] Map;
    [SerializeField] public Particle[,] mapObjects;
    [SerializeField] public List<Object> objs;
    [SerializeField] GameObject cursor;
    [SerializeField]
    public static readonly int
        mapWidth = 328,
        mapHeight = 188;

    [Header("Selected Value")]
    [SerializeField] GameObject particle;

    [Header("Brush Size")]
    [SerializeField] internal int size;


    // Start is called before the first frame update
    void Start()
    {
        Map = new int[mapWidth, mapHeight];
        mapObjects = new Particle[mapWidth, mapHeight];

        Cursor.visible = false;

        foreach (Object o in objs)
        {
            for (int w = -(o.size.x / 2); w < (o.size.x / 2); w++)
            {
                for (int h = -(o.size.y / 2); h < (o.size.y / 2); h++)
                {
                    if (o.center.x + w > mapWidth || o.center.x + w < 0 || o.center.y + h > mapHeight || o.center.y + h < 0) continue;
                    if (Map[o.center.x + w, o.center.y + h] == 1 || mapObjects[o.center.x + w, o.center.y + h] != null) continue;

                    Map[o.center.x + w, o.center.y + h] = 1;
                    mapObjects[o.center.x + w, o.center.y + h] = Instantiate(o.type, new Vector3(o.center.x + w, o.center.y + h, 0f), Quaternion.identity);
                }
            }
        }

        int seed = Random.Range(-10000, 10000);

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float CaveGen = Mathf.PerlinNoise((x + seed) * 0.02f, (y + seed) * 0.02f);

                if (CaveGen > 0.4f)
                {
                    Particle obj = Instantiate(particle, new Vector3(x, y, 0), Quaternion.identity).GetComponent<Particle>();
                    mapObjects[x, y] = obj;
                    Map[x, y] = 1;
                }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            cursor.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, .25f);

            Vector3 pos = new Vector3(Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y), 0f);

            switch (size)
            {
                case (1):

                    if ((int)pos.x >= mapWidth || (int)pos.x < 0 || (int)pos.y >= mapHeight || (int)pos.y < 0) return;
                    if (Map[(int)pos.x, (int)pos.y] == 1 && mapObjects[(int)pos.x, (int)pos.y].type != Particle.ParticleType.Water) return;

                    Map[(int)pos.x, (int)pos.y] = 1;
                    mapObjects[(int)pos.x, (int)pos.y] = Instantiate(particle, new Vector3((int)pos.x, (int)pos.y, 0), Quaternion.identity).GetComponent<Particle>();

                    break;

                case (<= 100):

                    for (int w = -(size / 2); w < (size / 2); w++)
                    {
                        for (int h = -(size / 2); h < (size / 2); h++)
                        {
                            if ((int)pos.x + w >= mapWidth || (int)pos.x + w < 0 || (int)pos.y + h >= mapHeight || (int)pos.y + h < 0) continue;

                            if (Map[(int)pos.x + w, (int)pos.y + h] == 1 || mapObjects[(int)pos.x + w, (int)pos.y + h] != null) continue;

                            Map[(int)pos.x + w, (int)pos.y + h] = 1;
                            mapObjects[(int)pos.x + w, (int)pos.y + h] = Instantiate(particle, new Vector3((int)pos.x + w, (int)pos.y + h, 0), Quaternion.identity).GetComponent<Particle>();
                        }
                    }

                    break;
            }
        }
        if (Input.GetMouseButton(1))
        {
            cursor.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, .25f);

            Vector3 pos = new Vector3(Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y), 0f);

            switch (size)
            {
                case (1):

                    if ((int)pos.x >= mapWidth || (int)pos.x < 0 || (int)pos.y >= mapHeight || (int)pos.y < 0) return;
                    if (Map[(int)pos.x, (int)pos.y] == 0 || mapObjects[(int)pos.x, (int)pos.y] == null) return;

                    Map[(int)pos.x, (int)pos.y] = 0;
                    Destroy(mapObjects[(int)pos.x, (int)pos.y].gameObject);
                    mapObjects[(int)pos.x, (int)pos.y] = null;

                    break;

                case (<= 100):

                    for (int w = -(size / 2); w < (size / 2); w++)
                    {
                        for (int h = -(size / 2); h < (size / 2); h++)
                        {
                            if ((int)pos.x + w >= mapWidth || (int)pos.x + w < 0 || (int)pos.y + h >= mapHeight || (int)pos.y + h < 0) continue;

                            if (Map[(int)pos.x + w, (int)pos.y + h] == 0 || mapObjects[(int)pos.x + w, (int)pos.y + h] == null) continue;

                            Map[(int)pos.x + w, (int)pos.y + h] = 0;
                            Destroy(mapObjects[(int)pos.x + w, (int)pos.y + h].gameObject);
                            mapObjects[(int)pos.x + w, (int)pos.y + h] = null;
                        }
                    }

                    break;
            }
        }

        UpdateParticles();
        UpdateCursor();
    }

    void UpdateParticles()
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (Map[x, y] == 0 || mapObjects[x, y].type == Particle.ParticleType.Wood) continue;

                mapObjects[x, y].CheckPos(x, y, Map, mapObjects);
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach (Object o in objs)
        {
            Gizmos.color = o.color;
            Gizmos.DrawCube(o.center, o.size);
        }
    }

    void UpdateCursor()
    {
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) cursor.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        cursor.transform.position = new Vector3(Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y), 10f);
        cursor.transform.localScale = new Vector3(size, size, 0);
    }
}

public abstract class Particle : MonoBehaviour
{
    public enum ParticleType
    {
        None,
        Gravel,
        Water,
        Wood,
        Fire,
        Smoke,
        Steam,
    }


    [Header("State")]
    [SerializeField] public ParticleType type;
    [SerializeField] public bool killsPlayer = false;

    public abstract void MoveParticle(Vector2Int position);

    public abstract bool CheckPos(int x, int y, int[,] Map, Particle[,] mapObjects);
}