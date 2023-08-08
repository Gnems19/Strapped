using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    private IPlayerController _player;
    private Vector2 _movement;
    void Awake() => _player = GetComponentInParent<IPlayerController>();
    void Update() {
        if (_player == null) return;
        if (_player.Input.X != 0 && _player.Grounded)
        {
            if(!SoundManager.Instance.sfxSource.isPlaying) SoundManager.Instance.PlayRunSound();
        }
        if (_player.LandingThisFrame) {
            SoundManager.Instance.PlayLandSound();
        }
        if (_player.JumpingThisFrame) {
            SoundManager.Instance.PlayJumpSound();
        }
    }
    
}
