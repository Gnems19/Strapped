using UnityEngine;
using Random = UnityEngine.Random;


public class PlayerAnimator : MonoBehaviour {
    [SerializeField] private Animator _anim;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private AudioClip[] _footsteps;
    private IPlayerController _player;
    private bool _playerGrounded;
    private Vector2 _movement;

    void Awake() => _player = GetComponentInParent<IPlayerController>();

    void Update() {
        if (_player == null) return;

        // Flip the sprite
        if (_player.Input.X != 0) transform.localScale = new Vector3(_player.Input.X > 0 ? 1 : -1, 1, 1);

        // Lean while running
        // var targetRotVector = new Vector3(0, 0, Mathf.Lerp(-_maxTilt, _maxTilt, Mathf.InverseLerp(-1, 1, _player.Input.X)));
        // _anim.transform.rotation = Quaternion.RotateTowards(_anim.transform.rotation, Quaternion.Euler(targetRotVector), _tiltSpeed * Time.deltaTime);


        // Splat
        if (_player.LandingThisFrame) {
            //_anim.SetTrigger(GroundedKey);
        }
        // Jump animations
        if (!_player.Grounded)
        {
            _anim.SetBool("Jumping", true);
        }
        else
        {
            _anim.SetBool("Jumping", false);
            
        }
        // Jump effects
        if (_player.JumpingThisFrame) {
            //_anim.SetTrigger(JumpKey);
            //_anim.ResetTrigger(GroundedKey);
            // Only play particles when grounded (avoid coyote)
            if (_player.Grounded) {
            }
        }

        // Play landing effects and begin ground movement effects
        if (!_playerGrounded && _player.Grounded) {
            _playerGrounded = true;
            
            _anim.SetBool("Jumping", false);

        }
        else if (_playerGrounded && !_player.Grounded) {
            _playerGrounded = false;

            _anim.SetBool("Jumping", true);
        }

        // Detect ground color
        var groundHit = Physics2D.Raycast(transform.position, Vector3.down, 2, _groundMask);
        if (groundHit && groundHit.transform.TryGetComponent(out SpriteRenderer r)) {
           
        }

        _movement = _player.RawMovement; // Previous frame movement is more valuable
    }

    void SetColor(ParticleSystem ps) {
        var main = ps.main;
    }

    #region Animation Keys

    private static readonly int GroundedKey = Animator.StringToHash("Grounded");
    private static readonly int IdleSpeedKey = Animator.StringToHash("IdleSpeed");
    private static readonly int JumpKey = Animator.StringToHash("Jump");

    #endregion
    

}
