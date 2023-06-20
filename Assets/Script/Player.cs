using System.Collections;
using System.Collections.Generic; using UnityEngine.SceneManagement;
using UnityEngine;

public class Player : MonoBehaviour
{
    public ParticleHandler particleMap;

    [Header("Speed Values")]
    [SerializeField] float speed;
    [SerializeField] Vector2Int size;

    bool gravity = true;
    internal Vector2Int position;
    Vector2 inputs;

    // Start is called before the first frame update
    void Start()
    {
        particleMap = FindObjectOfType<ParticleHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        inputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        position = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

        if (!CheckPos(0, 0, false) && particleMap.mapObjects[position.x, position.y] != null && particleMap.mapObjects[position.x, position.y].killsPlayer) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (Input.GetKeyDown(KeyCode.Space) && (!CheckPos(0, -2, false) || (particleMap.mapObjects[position.x, position.y] != null && particleMap.mapObjects[position.x, position.y].type == Particle.ParticleType.Water)))
        {
            StartCoroutine(Jump());
        }
    }

    private void FixedUpdate()
    {
        bool inputtingX = inputs.x != 0;

        if (inputtingX)
        {
            if (Mathf.Sign(inputs.x) == 1)
            {
                for (int s = 0; s <= speed; s++)
                {
                    if (!CheckPos(s, 0))
                    {
                        if (CheckPos(s, 1)) break;
                    }
                }
            }
            else
            {
                for (int s = 0; s >= -speed; s--)
                {
                    if (!CheckPos(s, 0))
                    {
                        if (CheckPos(s, 1)) break;
                    }
                }
            }
        }

        if (gravity) CheckPos(0, -1);
    }

    IEnumerator Jump()
    {
        gravity = false;

        for (int i = 0; i < 16; i++)
        {
            if (!CheckPos(0, 1) || !Input.GetKey(KeyCode.Space))
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
                    if (particleMap.mapObjects[position.x + x + scaleX, position.y + y + scaleY].killsPlayer) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

                    if (particleMap.mapObjects[position.x + x + scaleX, position.y + y + scaleY].type != Particle.ParticleType.Water) return false;
                }      
            }
        }

        if (!move) return true;

        transform.position += new Vector3Int(x, y, 0);

        return true;
    }
}
