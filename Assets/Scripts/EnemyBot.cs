using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Controller for Enemy Bots, who move around, damage Ruby on collision and can be fixed
 */
public class EnemyBot : MonoBehaviour
{
    private static readonly int AnimationMoveX = Animator.StringToHash("Move X");
    private static readonly int AnimationMoveY = Animator.StringToHash("Move Y");
    private static readonly int AnimationFixed = Animator.StringToHash("Fixed");

    // Components
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip fixAudioClip;
    [SerializeField] private ParticleSystem smokeEffect;

    // State
    public bool broken = true;

    // Movement
    private const float Speed = 3.0f;
    private const float TimeToChangeDirection = 4.0f;
    [SerializeField] private bool vertical;
    private float _changeDirectionTimer;
    private int _directionMultiplier = 1;

    public void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (!broken)
        {
            return;
        }

        _changeDirectionTimer += Time.deltaTime;

        if (_changeDirectionTimer >= TimeToChangeDirection)
        {
            _directionMultiplier *= -1;
            _changeDirectionTimer = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        if (!broken)
        {
            return;
        }

        var newPosition = _rigidbody2D.position;

        if (vertical)
        {
            newPosition.y += _directionMultiplier * Speed * Time.deltaTime;
            _animator.SetFloat(AnimationMoveX, 0);
            _animator.SetFloat(AnimationMoveY, _directionMultiplier);
        }
        else
        {
            newPosition.x += _directionMultiplier * Speed * Time.deltaTime;
            _animator.SetFloat(AnimationMoveX, _directionMultiplier);
            _animator.SetFloat(AnimationMoveY, 0);
        }

        _rigidbody2D.MovePosition(newPosition);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var rubyController = other.gameObject.GetComponent<Ruby>();
        if (rubyController == null) return;

        rubyController.ChangeHealth(-1);
    }

    public void Fix()
    {
        broken = false;
        _animator.SetBool(AnimationFixed, true);
        _rigidbody2D.simulated = false;
        smokeEffect.Stop();

        _audioSource.Stop();
        _audioSource.PlayOneShot(fixAudioClip);

        CheckForWinCondition();
    }

    private static void CheckForWinCondition()
    {
        var foundObjects = FindObjectsOfType<EnemyBot>();

        foreach (var enemyBot in foundObjects)
        {
            if (enemyBot.broken)
            {
                return;
            }
        }

        SceneManager.LoadScene("WinScreen");
    }
}