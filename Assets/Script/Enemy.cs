using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public ParticleHandler particleMap;
    public Player p;

    [Header("Speed Values")]
    [SerializeField] float inbetweenSpeed;
    float previousTimeMoved;
    [SerializeField] Vector2Int size;
    bool gravity = true;
    Vector2Int position;

    // Start is called before the first frame update
    void Start()
    {
        particleMap = FindObjectOfType<ParticleHandler>();
        p = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        position = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

        if (!CheckPos(0, 0, false) && particleMap.mapObjects[position.x, position.y] != null && particleMap.mapObjects[position.x, position.y].type != Particle.ParticleType.Water) Destroy(gameObject);

        if (p.position.y > position.y + 1 && !CheckPos(0, -1, false) || (particleMap.mapObjects[position.x, position.y] != null && particleMap.mapObjects[position.x, position.y].type == Particle.ParticleType.Water)) StartCoroutine(Jump());
    }

    private void FixedUpdate()
    {
        if (Time.time >= inbetweenSpeed + previousTimeMoved)
        {
            AngleDir(transform.forward, (transform.position - p.transform.position).normalized, transform.up);
            previousTimeMoved = Time.time;
        }

        if (gravity) CheckPos(0, -1);
    }

    public void AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0.0f)
        {
            if (CheckPos(-1, 0)) return;

            if (CheckPos(-1, 1)) return;

            if (!CheckPos(0, -1, false)) StartCoroutine(Jump());
        }
        else if (dir < 0.0f)
        {
            if (CheckPos(1, 0)) return;

            if (CheckPos(1, 1)) return;

            if (!CheckPos(0, -1, false)) StartCoroutine(Jump());
        }
        else
        {
            return;
        }
    }

    IEnumerator Jump()
    {
        gravity = false;

        for (int i = 0; i < 16; i++)
        {
            if (!CheckPos(0, 1))
            {
                gravity = true;
                yield break;
            };

            yield return new WaitForSeconds(i * 0.0025f);
        }

        yield return new WaitForSeconds(0.145f);

        gravity = true;
    }

    bool CheckPos(int x, int y, bool move = true)
    {
        for (int scaleX = -(size.x / 2); scaleX <= (size.x / 2); scaleX++)
        {
            for (int scaleY = -(size.y / 2); scaleY <= (size.y / 2); scaleY++)
            {
                if (position.x + x + scaleX < 0 || position.x + x + scaleX >= ParticleHandler.mapWidth) return false;
                if (position.y + y + scaleY < 0 || position.y + y + scaleY >= ParticleHandler.mapHeight) return false;

                if (particleMap.Map[position.x + x + scaleX, position.y + y + scaleY] != 0)
                {
                    if (particleMap.mapObjects[position.x + x + scaleX, position.y + y + scaleY].killsPlayer) Destroy(gameObject);

                    if (particleMap.mapObjects[position.x + x + scaleX, position.y + y + scaleY].type != Particle.ParticleType.Water) return false;
                }
            }
        }

        if (!move) return true;

        transform.position += new Vector3Int(x, y, 0);

        return true;
    }
}
