using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject missileLauncherRockets;
    public GameObject player;
    [SerializeField] Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private static readonly int Moving = Animator.StringToHash("Moving");
    private static readonly int Moveing = Animator.StringToHash("Moveing");

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // change this 
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        // if player is far away follow but dont change y position
        
        if (Vector2.Distance(transform.position, player.transform.position) > 7)
        {
            var bossPosition = transform.position;
            var playerPosition = player.transform.position;
            Vector3 nextPos = Vector2.MoveTowards(bossPosition, playerPosition, 5 * Time.deltaTime);
            // only save x position
            bossPosition = new Vector3(nextPos.x, bossPosition.y, bossPosition.z);
            transform.position = bossPosition;
            if ((bossPosition.x - playerPosition.x) < 0)
            {
                _animator.SetBool("MovingRight", true);
                _animator.SetBool("TurnedLeft", false);

            }
            else if ((bossPosition.x - playerPosition.x) > 0)
            {
                _animator.SetBool("MovingRight", false);
                _animator.SetBool("TurnedLeft", true);
            }
            
            _animator.SetBool(Moving, true);
        }
        else
        {
            _animator.SetBool(Moving, false);

            _animator.SetBool("LaunchingMissiles", true); // delegate animation logic in the future
        }


    }
    
    public void LaunchMissile()
    {
        missileLauncherRockets.SetActive(true);
        SoundManager.Instance.ExplisionSound();
    }
}
