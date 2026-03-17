using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BossScript : MonoBehaviour
{
    [SerializeField] private GameObject missileLauncherRockets;
    [FormerlySerializedAs("_animator")] [SerializeField]
    private Animator animator;
    [SerializeField] private GameObject player;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float attackInterval = 10f;
    [SerializeField] private float followDistance = 1.5f;

    private float _attackTimer = 0f;
    private bool _isDead = false;
    private bool _facingRight = false;

    private enum BossState { Sleeping, Following, Launching }
    private BossState _state = BossState.Sleeping;

    public BossScript(GameObject missileLauncherRockets)
    {
        this.missileLauncherRockets = missileLauncherRockets;
    }

    private static readonly int MovingLeft = Animator.StringToHash("MovingLeft");
    private static readonly int TurnedLeft = Animator.StringToHash("TurnedLeft");
    private static readonly int MovingRight = Animator.StringToHash("MovingRight");
    private static readonly int LaunchingMissiles = Animator.StringToHash("LaunchingMissiles");
    private static readonly int PluggedOut = Animator.StringToHash("PluggedOut");
    private const string RocketLaunchLeftState = "Base Layer.RocketLaunchLeft";
    private const string RocketLaunchRightState = "Base Layer.RocketLaunchRight";

    private void Update()
    {
        if (_isDead) return;

        switch (_state)
        {
            case BossState.Sleeping:
                break;

            case BossState.Following:
                UpdateFacingAndMovement();
                _attackTimer += Time.deltaTime;
                if (_attackTimer >= attackInterval)
                {
                    _state = BossState.Launching;
                    StopMovementAnimation();
                    animator.SetBool(LaunchingMissiles, true);
                    animator.CrossFadeInFixedTime(_facingRight ? RocketLaunchRightState : RocketLaunchLeftState, 0.05f);
                }
                break;

            case BossState.Launching:
                // Waiting for animation event to call LaunchMissile()
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void FacePlayer()
    {
        var playerPos = player.transform.position;
        var bossPos = transform.position;
        var playerIsRight = playerPos.x > bossPos.x;

        if (playerIsRight == _facingRight) return;
        _facingRight = playerIsRight;
        animator.SetBool(TurnedLeft, !_facingRight);
    }

    private void FollowPlayer()
    {
        var bossPos = transform.position;
        var playerPos = player.transform.position;
        var dist = Mathf.Abs(bossPos.x - playerPos.x);

        if (dist > followDistance)
        {
            var moveRight = playerPos.x > bossPos.x;

            // Don't move physically while a turn or rocket launch animation is playing
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            var isBusy = stateInfo.IsName("TurnRight") || stateInfo.IsName("TurnLeft")
                || stateInfo.IsName("RocketLaunchRight") || stateInfo.IsName("RocketLaunchLeft");
            if (!isBusy)
            {
                var dir = moveRight ? 1f : -1f;
                transform.position = new Vector3(
                    bossPos.x + dir * followSpeed * Time.deltaTime,
                    bossPos.y, bossPos.z);
            }

            SetMovementDirection(moveRight);
        }
        else
        {
            StopMovementAnimation();
        }
    }

    private void UpdateFacingAndMovement()
    {
        FacePlayer();
        FollowPlayer();
    }

    /// <summary>Called by EnterBossAreaScript when player enters boss area.</summary>
    public void WakeUp()
    {
        if (_state != BossState.Sleeping || _isDead) return;
        _state = BossState.Following;
        _attackTimer = 0f;
        // Prime the animator on the activation frame so movement does not wait for the next Update.
        UpdateFacingAndMovement();
    }

    /// <summary>Called by animation event when missile launch animation finishes.</summary>
    public void LaunchMissile()
    {
        if (_isDead) return;
        missileLauncherRockets.transform.position = new Vector3(
            transform.position.x + 3.3f,
            transform.position.y + 5.5f,
            transform.position.z);
        missileLauncherRockets.SetActive(true);
        SoundManager.Instance.ExplisionSound();

        animator.SetBool(LaunchingMissiles, false);
        _attackTimer = 0f;
        _state = BossState.Following;
        UpdateFacingAndMovement();
    }

    /// <summary>Called by OutletScript when player unplugs the boss.</summary>
    public void Unplug()
    {
        _isDead = true;
        _state = BossState.Sleeping;
        StopMovementAnimation();
        animator.SetBool(LaunchingMissiles, false);
        animator.SetTrigger(PluggedOut);
        missileLauncherRockets.SetActive(false);
    }

    private void SetMovementDirection(bool movingRight)
    {
        animator.SetBool(MovingRight, movingRight);
        animator.SetBool(MovingLeft, !movingRight);
    }

    private void StopMovementAnimation()
    {
        animator.SetBool(MovingLeft, false);
        animator.SetBool(MovingRight, false);
    }
}
