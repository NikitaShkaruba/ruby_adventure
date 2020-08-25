using UnityEngine;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

/**
 * Controller for Ruby - the main character. She can move around, throw cogs, also she has health
 */
public class Ruby : MonoBehaviour
{
    private static readonly int AnimationPropertyLookX = Animator.StringToHash("Look X");
    private static readonly int AnimationPropertyLookY = Animator.StringToHash("Look Y");
    private static readonly int AnimationPropertySpeed = Animator.StringToHash("Speed");
    private static readonly int AnimationPropertyHit = Animator.StringToHash("Hit");
    private static readonly int AnimationPropertyCogLaunch = Animator.StringToHash("Launch");

    // In some situations we add an offset to rigidBody2D to get Ruby's body coordinates, not her feet
    private static readonly Vector2 LegsToBodyPositionOffset = Vector2.up * 0.5f;

    // Components
    private Rigidbody2D _rigidBody2D;
    private Animator _animator;
    private AudioSource _audioSource;
    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] public AudioClip throwCogAudioClip;
    [SerializeField] public AudioClip getHurtAudioClip;

    // Health
    [SerializeField] public int maxHealth = 5;
    public int Health { get; private set; }

    // Invincibility
    [SerializeField] public float timeInvincible = 0.5f;
    private bool _isInvincible;
    private float _invincibleTimer;

    // Movement
    private const float Speed = 3.0f;
    private float _horizontalSpeed;
    private float _verticalSpeed;
    private Vector2 _lookDirection = new Vector2(0, -1);

    // Fighting
    public float throwCogTimeout = 0.5f;
    private float _throwCogTimeoutTimer;

    private void Start()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        Health = maxHealth;
    }

    // Update is called once per frame
    private void Update()
    {
        var horizontalSpeed = Input.GetAxis("Horizontal");
        var verticalSpeed = Input.GetAxis("Vertical");
        var isLaunchCogKeyPressed = Input.GetKeyDown(KeyCode.C);
        var isInteractKeyPressed = Input.GetKeyDown(KeyCode.X);

        Move(horizontalSpeed, verticalSpeed);

        if (isLaunchCogKeyPressed)
        {
            ThrowCog();
        }

        if (isInteractKeyPressed)
        {
            Interact();
        }

        if (_isInvincible)
        {
            ComputeInvincibilityState();
        }

        if (_throwCogTimeoutTimer > 0)
        {
            _throwCogTimeoutTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        var newPosition = _rigidBody2D.position;

        newPosition.x += Speed * _horizontalSpeed * Time.deltaTime;
        newPosition.y += Speed * _verticalSpeed * Time.deltaTime;

        _rigidBody2D.MovePosition(newPosition);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (_isInvincible) return;

            _isInvincible = true;
            _invincibleTimer = timeInvincible;
            _animator.SetTrigger(AnimationPropertyHit);
        }

        Health = Mathf.Clamp(Health + amount, 0, maxHealth);
        UiHealthBar.Instance.SetValue(Health / (float) maxHealth);

        if (amount < 0)
        {
            PlaySound(getHurtAudioClip);
        }

        if (Health == 0)
        {
            SceneManager.LoadScene("LoseScreen");
        }
    }

    private void ThrowCog()
    {
        if (_throwCogTimeoutTimer > 0)
        {
            return;
        }

        _throwCogTimeoutTimer = throwCogTimeout;

        var projectileObject =
            Instantiate(projectilePrefab, _rigidBody2D.position + LegsToBodyPositionOffset, Quaternion.identity);

        var projectile = projectileObject.GetComponent<ThrowableCog>();
        projectile.Launch(_lookDirection, 300);

        _animator.SetTrigger(AnimationPropertyCogLaunch);
        PlaySound(throwCogAudioClip);
    }

    private void Interact()
    {
        var rubyCenter = _rigidBody2D.position + LegsToBodyPositionOffset;

        var raycast = Physics2D.Raycast(rubyCenter, _lookDirection, 1.5f, LayerMask.GetMask("NPC"));
        if (raycast.collider != null)
        {
            var npc = raycast.collider.GetComponent<NonPlayableCharacter>();
            if (npc != null)
            {
                npc.DisplayDialog();
            }
        }
    }

    private void ComputeInvincibilityState()
    {
        _invincibleTimer -= Time.deltaTime;

        if (_invincibleTimer < 0)
        {
            _isInvincible = false;
        }
    }

    private void Move(float horizontalSpeed, float verticalSpeed)
    {
        _horizontalSpeed = horizontalSpeed;
        _verticalSpeed = verticalSpeed;

        var move = new Vector2(_horizontalSpeed, _verticalSpeed);
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            _lookDirection.Set(move.x, move.y);
            _lookDirection.Normalize();
        }

        _animator.SetFloat(AnimationPropertyLookX, _lookDirection.x);
        _animator.SetFloat(AnimationPropertyLookY, _lookDirection.y);
        _animator.SetFloat(AnimationPropertySpeed, move.magnitude);
    }

    public void PlaySound(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }
}