using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject missileLauncherRockets;
    [SerializeField] Animator _animator;
    [SerializeField] private GameObject player;
    private float time_passed = 0f;
    private bool _launching = false;
    private static readonly int Moving = Animator.StringToHash("Moving");
    private static readonly int TurnedLeft = Animator.StringToHash("TurnedLeft");
    private static readonly int MovingRight = Animator.StringToHash("MovingRight");
    private static readonly int LaunchingMissiles = Animator.StringToHash("LaunchingMissiles");

    // Update is called once per frame
    private void Update()
    {
        // if player is far away follow but dont change y position
        
        if (Vector2.Distance(transform.position, player.transform.position) > 3 && !_launching)
        {
            var bossPosition = transform.position;
            var playerPosition = player.transform.position;
            Vector3 nextPos = Vector2.MoveTowards(bossPosition, playerPosition, 5 * Time.deltaTime);
            // only save x position
            bossPosition = new Vector3(nextPos.x, bossPosition.y, bossPosition.z);
            transform.position = bossPosition;
            _animator.SetBool(Moving, true);
            if ((bossPosition.x - playerPosition.x) < 0)
            {
                // start by turning right and if the animation is finished then set moving right to true
                if (time_passed > 2)
                {
                    _launching = true;
                    _animator.SetBool(LaunchingMissiles, true);
                }
                else
                {
                    _animator.SetBool(TurnedLeft, false);
                    _animator.SetBool(MovingRight, true);
                     time_passed += Time.deltaTime;
                }
                
            }
            else if ((bossPosition.x - playerPosition.x) > 0)
            {
               
                if(time_passed > 0) time_passed -= Time.deltaTime;
                _animator.SetBool(MovingRight, false);
                _animator.SetBool(TurnedLeft, true);
                
            }
        }
        else
        {
            _animator.SetBool(Moving, false);
        }
    }
    
    public void LaunchMissile() // should be called from animation event
    {
        missileLauncherRockets.SetActive(true);
        SoundManager.Instance.ExplisionSound();
        _launching = false;
    }
}
