using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Rigidbody2D rb;
    // Start is called before the first frame update
    public float speed = 3;
    public bool isGrounded;
    public float deaths;
    public Vector2 startPos;
    public enum gameMode {Cube, Ship, Wave, UFO, Ball, Spider}
    public gameMode currentGM;
    public AudioSource audioSource;
    public GameObject PlayerDual;
    public bool createPrefab = true;
    public bool isDualCopy = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentGM = gameMode.Cube;
        startPos = new Vector2(127, 0);
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(speed, rb.velocity.y);
        movePlayer();
    }

    void Jump()
    {
        if(Input.GetMouseButton(0) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, (speed + 5) * rb.gravityScale/2);
            isGrounded = false;
        }
    }

    void Fly()
    {
        if (Input.GetMouseButton(0))
        {
            rb.AddForce(new Vector2(0, 3 * (rb.gravityScale/2)));
        }
    }

    void Wave()
    {
        if(Input.GetMouseButton(0))
        {
            rb.velocity = new Vector2(rb.velocity.x, speed);
        }
        if(Input.GetMouseButtonUp(0))
        {
            rb.velocity = new Vector2(rb.velocity.x, -speed);
        }
    }

    void movePlayer()
    {
        if (currentGM == gameMode.Cube)
        {
            Jump();
        }
        else if (currentGM == gameMode.Ship)
        {
            Fly();
        }
        else if (currentGM == gameMode.Wave)
        {
            Wave();
        }else if(currentGM == gameMode.Ball)
        {
            if(Input.GetMouseButtonDown(0) && isGrounded)
            {
                rb.gravityScale *= -1;
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("ground"))
        {
            if (transform.position.x + 0.49f < collision.gameObject.transform.position.x - collision.gameObject.transform.localScale.x/2)
            {
                RespawnPlayer();
                deaths++;
            }
            if (transform.position.y + 0.49f < collision.gameObject.transform.position.y - collision.gameObject.transform.localScale.y / 2 && rb.gravityScale/3 == 1)
            {
                RespawnPlayer();
                deaths++;
            }
            isGrounded = true;
            if(currentGM == gameMode.Wave)
            {
                RespawnPlayer();
                deaths++;
            }
        }

        if(collision.gameObject.CompareTag("spike"))
        {
            RespawnPlayer();
            deaths++;
        }

        if(collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), collision.gameObject.GetComponent<BoxCollider2D>());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = false;
        }

        if (collision.gameObject.CompareTag("yellowOrb"))
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("shipPortal"))
        {
            currentGM = gameMode.Ship;
            rb.gravityScale = 2;
        }

        if (collision.gameObject.CompareTag("cubePortal"))
        {
            currentGM = gameMode.Cube;
            rb.gravityScale = 2;
        }

        if (collision.gameObject.CompareTag("gravityPortal"))
        {
            rb.gravityScale *= -1;
        }

        if (collision.gameObject.CompareTag("wavePortal"))
        {
            rb.gravityScale = 0;
            currentGM = gameMode.Wave;
        }

        if (collision.gameObject.CompareTag("ballPortal"))
        {
            currentGM = gameMode.Ball;
            rb.gravityScale = 2;
        }

        if(collision.gameObject.CompareTag("SpeedPortalFast"))
        {
            speed = 9f;
            
        }

        if (collision.gameObject.CompareTag("SpeedPortalNormal"))
        {
            speed = 6f;
        }

        if (collision.gameObject.CompareTag("SpeedPortalSlow"))
        {
            speed = 3f;
        }

        if (collision.gameObject.CompareTag("DualPortal"))
        {
            if (createPrefab == true)
            {
                GameObject clone = Instantiate(PlayerDual, transform.position, transform.rotation);
                clone.name = "Clone";
                clone.GetComponent<PlayerControl>().isDualCopy = true;
                clone.GetComponent<PlayerControl>().currentGM = currentGM;
                clone.GetComponent<PlayerControl>().speed = speed;
                createPrefab = false;

            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("yellowOrb"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("blueOrb"))
        {
            isGrounded = true;
            if (Input.GetMouseButtonDown(0))
            {
                rb.gravityScale *= -1;
                Debug.Log("Switched");
            }
        }
    }

    public void RespawnPlayer()
    {
        GameObject playerClone = GameObject.Find("Clone");
        GameObject playerOG = GameObject.Find("Player");
        playerOG.GetComponent<PlayerControl>().createPrefab = true;
        playerOG.transform.position = startPos;
        playerOG.GetComponent<PlayerControl>().ResetGame();
        Destroy(playerClone);
        

        /*GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (!isDualCopy)
        {
            foreach (GameObject player in players)
            {
                if (!player.GetComponent<PlayerControl>().isDualCopy)
                {
                    player.transform.position = startPos;
                    createPrefab = true;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }else
        {
            Destroy(gameObject);
        }*/
    }

    public void ResetGame()
    {
        currentGM = gameMode.Cube;
        rb.gravityScale = 2;
        isGrounded = true;
        speed = 6;
        audioSource.Play();
    }
}
