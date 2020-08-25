using UnityEngine;

/**
 * Controller for spikes that damage Ruby
 */
public class Spike : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        var rubyController = other.GetComponent<Ruby>();
        if (rubyController == null) return;
        
        rubyController.ChangeHealth(-1);
    }
}
