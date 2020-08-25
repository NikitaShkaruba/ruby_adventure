using UnityEngine;

/**
 * Controller for collectible health pickups. They increase Ruby's health
 */
public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var rubyController = other.GetComponent<Ruby>();

        if (rubyController == null) return;
        if (rubyController.Health >= rubyController.maxHealth) return;

        rubyController.ChangeHealth(1);
        // We play sound on ruby because I'm lazy to do it properly in this game. I don't want to polish it to the extreme :)
        rubyController.PlaySound(audioClip);

        Destroy(gameObject);
    }
}