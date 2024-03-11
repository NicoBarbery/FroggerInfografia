using UnityEngine;

public class MoveCycle : MonoBehaviour
{
    public Vector2 direction = Vector2.right;
    public float speed = 1f;
    public int size = 1;

    private Vector3 leftEdge;
    private Vector3 rightEdge;
    // Start is called before the first frame update
    private void Start()
    {
        //obtengo la esquina inferior izquierda. Punto (0,0) coordenadas de la camara
        leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        //obtengo la esquina inferior derecha. Punto (1,0) coordenadas de la camara
        rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
    }

    // Update is called once per frame
    private void Update()
    {
        if (direction.x > 0 && (transform.position.x - size) > rightEdge.x)
        {
            Vector3 position = transform.position;
            position.x = leftEdge.x - size;
            transform.position = position;
        }
        else if (direction.x < 0 && (transform.position.x + size) < leftEdge.x)
        {
            Vector3 position = transform.position;
            position.x = rightEdge.x + size;
            transform.position = position;
        }
        //y si no pasa ningun limite entonces se mueve normalmente
        else
        {
            //usamos deltaTime para que no importa lo rapido que va de computadora a computadora que sea independiente de ello
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }
}
