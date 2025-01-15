using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class PlayerAnimator : MonoBehaviour {
    [SerializeField] private Animator _anim;
    [SerializeField] private Logger logger;
    
    private IPlayerController _player;
    
    private bool _playerGrounded;
    
    
    private void Start()
    {
        logger.Log("PlayerAnimator Start called");
        _playerGrounded = true;
    }

    void Awake() => _player = GetComponentInParent<IPlayerController>();

    void Update() {
        if (_player == null) return;

        if (_player.IsDead)
        {
            //logger.Log("PlayerAnimator Update called when dead");
            _anim.SetBool("isDead", true);
            _anim.SetFloat("Speed", 0);
            _anim.SetFloat("YVelocity", 0);
            _anim.SetBool("Jumping", false);
        }
        else
        {
            // Flip the sprite
            if (_player.Input.X != 0) transform.localScale = new Vector3(_player.Input.X > 0 ? 1 : -1, 1, 1);
            // Run animations
            _anim.SetFloat("Speed", Mathf.Abs(_player.Velocity.x));
            // Jump / Fall animations 
            _anim.SetFloat("YVelocity", _player.Velocity.y);
            // Jump animations
            if (!_player.Grounded)
            {
                _anim.SetBool("Jumping", true);
            }else
            {
                _anim.SetBool("Jumping", false);
            }
            // Play landing effects and begin ground movement effects
            if (!_playerGrounded && _player.Grounded) {
                _anim.SetBool("Jumping", false);
            }
            else if (_playerGrounded && !_player.Grounded) {
                _playerGrounded = false;
                _anim.SetBool("Jumping", true);
            }
            // Land animations
            if (_player.LandingThisFrame) {
                //TODO
            }
        }
    }

}
