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
    [Header("MISSILE LAUNCH OFFSETS")]
    [SerializeField] private float launchOffsetXRight = 3.3f;
    [SerializeField] private float launchOffsetXLeft = 2.05f;
    [SerializeField] private float launchOffsetY = 5.5f;

    private float _attackTimer = 0f;
    private bool _isDead = false;
    private bool _facingRight = false;
    private float _debugLogTimer = 0f;

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
                    Debug.Log($"[BOSS] Entering Launching state: _facingRight={_facingRight} -> playing {(_facingRight ? RocketLaunchRightState : RocketLaunchLeftState)}");
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
        var playerIsRight = player.transform.position.x > transform.position.x;
        if (playerIsRight != _facingRight)
            Debug.Log($"[BOSS] FacePlayer changed: _facingRight {_facingRight} -> {playerIsRight} | playerX={player.transform.position.x:F2} bossX={transform.position.x:F2}");
        _facingRight = playerIsRight;
        animator.SetBool(TurnedLeft, !_facingRight);
    }

    private string GetAnimatorStateName()
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("IdleLeft")) return "IdleLeft";
        if (info.IsName("IdleRight")) return "IdleRight";
        if (info.IsName("WalkLeft")) return "WalkLeft";
        if (info.IsName("WalkRight")) return "WalkRight";
        if (info.IsName("TurnRight")) return "TurnRight";
        if (info.IsName("TurnLeft")) return "TurnLeft";
        if (info.IsName("RocketLaunchRight")) return "RocketLaunchRight";
        if (info.IsName("RocketLaunchLeft")) return "RocketLaunchLeft";
        if (info.IsName("ShutDownLeft")) return "ShutDownLeft";
        if (info.IsName("ShutDownRight")) return "ShutDownRight";
        if (info.IsName("Sleep")) return "Sleep";
        if (info.IsName("rsleep")) return "rsleep";
        return $"UNKNOWN(hash:{info.fullPathHash})";
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

            // Log every 0.5s to avoid spam
            _debugLogTimer += Time.deltaTime;
            if (_debugLogTimer >= 0.5f)
            {
                _debugLogTimer = 0f;
                var inTransition = animator.IsInTransition(0);
                var nextState = inTransition ? animator.GetNextAnimatorStateInfo(0) : default;
                Debug.Log($"[BOSS] State={GetAnimatorStateName()} inTransition={inTransition} isBusy={isBusy} " +
                    $"_facingRight={_facingRight} moveRight={moveRight} dist={dist:F2} " +
                    $"params: TurnedLeft={animator.GetBool(TurnedLeft)} MovingR={animator.GetBool(MovingRight)} MovingL={animator.GetBool(MovingLeft)} " +
                    $"bossX={bossPos.x:F2} playerX={playerPos.x:F2}");
            }

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

    // TODO: launchOffsetXLeft is misaligned — when boss faces left, missiles don't line up
    //       correctly with the boss sprite. Right side (launchOffsetXRight) works fine.
    //       The left offset likely needs to account for the boss sprite's asymmetric pivot.
    /// <summary>Called by animation event when missile launch animation finishes.</summary>
    public void LaunchMissile()
    {
        if (_isDead) return;
        var xOffset = _facingRight ? launchOffsetXRight : launchOffsetXLeft;
        Debug.Log($"[BOSS] LaunchMissile: _facingRight={_facingRight} xOffset={xOffset} (R={launchOffsetXRight}, L={launchOffsetXLeft}) bossPos={transform.position} playerPos={player.transform.position}");
        missileLauncherRockets.transform.position = new Vector3(
            transform.position.x + xOffset,
            transform.position.y + launchOffsetY,
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
