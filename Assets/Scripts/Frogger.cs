using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frogger : MonoBehaviour
{
    public Sprite staySprite;
    public Sprite jumpSprite;
    public Sprite deadSprite;

    private SpriteRenderer spriteRenderer;
    private Vector3 spawnPosition;
    private float farthestRow;
    private bool cooldown;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            Move(Vector3.up);
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            Move(Vector3.left);
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            Move(Vector3.down);
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            Move(Vector3.right);
        }
    }

    private void Move(Vector3 direction)
    {
        if (cooldown) return;
        //transform.position += direction; cambiaba instantaneamente la posicion

        //calculamos nuestro destino
        Vector3 destination = transform.position + direction;

        //chequeareamos si hay existe un objeto en el destino que nos movemos, retornara un collider si existe
        //OverLapBox retornara un collider dada un destino
        Collider2D barrier = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Barrier"));
        //hacemos lo mismo para las plataformas
        Collider2D plataform = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Platform"));
        //hacemos lo mismo para las obstaculos
        Collider2D obstacle = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Obstacle"));

        //si no recibimos null entonces nos chocamos y no hacemos nada
        if (barrier != null)
        {
            return;
        }

        //si estamos en una plataforma nos movemos con la plataforma asi que asignamos que esa plataforma sea parent de la rana
        if (plataform != null)
        {
            transform.SetParent(plataform.transform);
        }
        //si no existe plataforma en esa locacion entonces no tenemos padre
        else
        {
            transform.SetParent(null);
        }

        //en caso de chocar con un obstaculo moriremos Y QUE NO ESTE ENCIMA DE UNA PLATAFORMA
        if (obstacle != null && plataform == null)
        {
            transform.position = destination;
            Die();
        }
        else
        {
            //aqui nos aseguramos que el jugador supere lo que ya tenia como farthest row y no pueda hacer trampa
            if (destination.y > farthestRow)
            {
                farthestRow = destination.y;
                FindAnyObjectByType<GameManager>().AdvancedRow();
            }
            //Va a cambiar el valor de position a lo largo del tiempo, es una opcion hacer correr otro codigo fuera del update loop
            StopAllCoroutines();
            StartCoroutine(Jumpp(destination));
        }
    }

    private IEnumerator Jumpp(Vector3 destination)
    {
        Vector3 startPosition = transform.position;

        float timeElapsed = 0f;
        float duration = 0.125f;

        spriteRenderer.sprite = jumpSprite;
        cooldown = true;

        //mientras nuestra animacion no a terminado
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            //cambiamos nuestra posicion
            transform.position = Vector3.Lerp(startPosition, destination, t);
            timeElapsed += Time.deltaTime;
            //lo que hace es que pausara esta corrutina y espera hasta el siguiente frame
            yield return null;
        }
        //seteamos el estado final
        transform.position = destination;
        spriteRenderer.sprite = staySprite;
        cooldown = false;
    }

    public void Respawn()
    {
        //debemos tomar en cuenta para las corrutinas como cuando morimos para respawnear
        StopAllCoroutines();

        // Reset transform to spawn
        transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
        farthestRow = spawnPosition.y;

        // Reset sprite
        spriteRenderer.sprite = staySprite;

        // Enable control
        gameObject.SetActive(true);
        enabled = true;
        cooldown = false;
    }

    public void Die()
    {
        //debemos parar las corrutinas es decir las animaciones porque si no seguiremos saltando a pesar de estar muertos
        StopAllCoroutines();
        //dishabilitamos
        enabled = false;
        //reseteamos la rotacion
        transform.rotation = Quaternion.identity;
        spriteRenderer.sprite = deadSprite;


        GameManager.Instance.Died();
    }


    //chequearemos que cuando frogger este quieto reciba un impacto con otra cosa
    private void OnTriggerEnter2D(Collider2D other)
    {
        bool hitObstacle = other.gameObject.layer == LayerMask.NameToLayer("Obstacle");
        bool onPlatform = transform.parent != null;

        if (enabled && hitObstacle && !onPlatform)
        {
            Die();
        }
    }
}
