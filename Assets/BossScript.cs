using UnityEngine;

public class BossScript : MonoBehaviour
{
    [SerializeField] GameObject missileLauncherRockets;
    [SerializeField] Animator _animator;
    [SerializeField] private GameObject player;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float attackInterval = 10f;
    [SerializeField] private float followDistance = 1.5f;

    private float _attackTimer = 0f;
    private bool _isDead = false;
    private bool _facingRight = false;

    private enum BossState { Sleeping, Following, Launching }
    private BossState _state = BossState.Sleeping;

    private static readonly int Moving = Animator.StringToHash("Moving");
    private static readonly int TurnedLeft = Animator.StringToHash("TurnedLeft");
    private static readonly int MovingRight = Animator.StringToHash("MovingRight");
    private static readonly int LaunchingMissiles = Animator.StringToHash("LaunchingMissiles");
    private static readonly int PluggedOut = Animator.StringToHash("PluggedOut");

    private void Update()
    {
        if (_isDead) return;

        switch (_state)
        {
            case BossState.Sleeping:
                break;

            case BossState.Following:
                FacePlayer();
                FollowPlayer();
                _attackTimer += Time.deltaTime;
                if (_attackTimer >= attackInterval)
                {
                    _state = BossState.Launching;
                    _animator.SetBool(Moving, false);
                    _animator.SetBool(LaunchingMissiles, true);
                }
                break;

            case BossState.Launching:
                // Waiting for animation event to call LaunchMissile()
                break;
        }
    }

    private void FacePlayer()
    {
        var playerPos = player.transform.position;
        var bossPos = transform.position;
        bool playerIsRight = playerPos.x > bossPos.x;

        if (playerIsRight != _facingRight)
        {
            _facingRight = playerIsRight;
            _animator.SetBool(MovingRight, _facingRight);
            _animator.SetBool(TurnedLeft, !_facingRight);
        }
    }

    private void FollowPlayer()
    {
        var bossPos = transform.position;
        var playerPos = player.transform.position;
        float dist = Mathf.Abs(bossPos.x - playerPos.x);

        if (dist > followDistance)
        {
            float dir = playerPos.x > bossPos.x ? 1f : -1f;
            transform.position = new Vector3(
                bossPos.x + dir * followSpeed * Time.deltaTime,
                bossPos.y, bossPos.z);
            _animator.SetBool(Moving, true);
        }
        else
        {
            _animator.SetBool(Moving, false);
        }
    }

    /// <summary>Called by EnterBossAreaScript when player enters boss area.</summary>
    public void WakeUp()
    {
        if (_state != BossState.Sleeping || _isDead) return;
        _state = BossState.Following;
        _attackTimer = 0f;
        // Immediately start facing and moving
        FacePlayer();
    }

    /// <summary>Called by animation event when missile launch animation finishes.</summary>
    public void LaunchMissile()
    {
        missileLauncherRockets.transform.position = new Vector3(
            transform.position.x + 3.3f,
            transform.position.y + 5.5f,
            transform.position.z);
        missileLauncherRockets.SetActive(true);
        SoundManager.Instance.ExplisionSound();

        _animator.SetBool(LaunchingMissiles, false);
        _attackTimer = 0f;
        _state = BossState.Following;
    }

    /// <summary>Called by OutletScript when player unplugs the boss.</summary>
    public void Unplug()
    {
        _isDead = true;
        _state = BossState.Sleeping;
        _animator.SetBool(Moving, false);
        _animator.SetBool(LaunchingMissiles, false);
        _animator.SetTrigger(PluggedOut);
    }
}
