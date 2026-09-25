using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Boulder : MonoBehaviour
{
    [Header("Caída")]
    public float fallSpeed = 5f;
    public bool moveDown = true; // true = cae en Y negativo, false = cae en X (por si tus carriles son horizontales)

    [Header("Colisión con el suelo")]
    public string groundTag = "Ground";

    void Reset()
    {
        // Aseguramos que el Rigidbody2D sea Kinematic para no verse afectado por la física
        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    void Update()
    {
        Vector3 direction = moveDown ? Vector3.down : Vector3.left;
        transform.Translate(direction * fallSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null)
            return;

        if (other.TryGetComponent<Character>(out Character character))
        {
            character.toque();
        }

        if (other.CompareTag(groundTag))
        {
            Character player = FindAnyObjectByType<Character>();
            if (player != null && !player.gameOverPanel.activeSelf)
            {
                player.score += 1;
            }

            Destroy(gameObject);
        }
    }
}

