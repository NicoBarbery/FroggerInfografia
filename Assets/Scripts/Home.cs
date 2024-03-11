using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class Home : MonoBehaviour
{
    public GameObject winFrog;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    private void OnEnable()
    {
        winFrog.SetActive(true);
        boxCollider.enabled = false;
    }

    private void OnDisable()
    {
        winFrog.SetActive(false);
        boxCollider.enabled = true;
    }
    //verificar la colision de Home con algo
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled && other.gameObject.CompareTag("Player"))
        {
            enabled = true;
            GameManager.Instance.HomeOccupied();
        }
    }
}
