using UnityEngine;

public class ThrowableCog : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (transform.position.magnitude > 1000.0f)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 direction, float force)
    {
        _rigidbody2D.AddForce(direction * force);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var enemyBotController = other.collider.GetComponent<EnemyBot>();
        if (enemyBotController != null)
        {
            enemyBotController.Fix();
        }
        
        Destroy(gameObject);
    }
}
