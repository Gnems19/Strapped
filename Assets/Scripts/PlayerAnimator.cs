using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class PlayerAnimator : MonoBehaviour {
    [SerializeField] private Animator _anim;

    private IPlayerController _player;
    private bool _playerGrounded;
    private void Start()
    {
        _playerGrounded = true;
    }

    void Awake() => _player = GetComponentInParent<IPlayerController>();

    void Update() {
        if (_player == null) return;
        // Flip the sprite
        if (_player.Input.X != 0) transform.localScale = new Vector3(_player.Input.X > 0 ? 1 : -1, 1, 1);
        

        // Land animations
        if (_player.LandingThisFrame) {
            //TODO
        }
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
    }

}
