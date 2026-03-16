using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingMissile : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float _speed = 9f;
    [SerializeField] private float _rotateSpeed = 200f;
    [SerializeField] private GameObject explosionPrefab;
    private static readonly int Close = Animator.StringToHash("Hit");

    private Animator animator;
    private Rigidbody2D _rigidbody2D;
    private Transform _target;
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        //animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // if destroied wait for the blow up animation and then destroy
        if (_target == null)
        {
            // stop the missile
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0;

            return;
        }

        Vector2 direction = (Vector2)_target.position - _rigidbody2D.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        _rigidbody2D.angularVelocity = -rotateAmount * _rotateSpeed;
        _rigidbody2D.velocity = transform.up * _speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 position = transform.position;
            Destroy(gameObject);
            SoundManager.Instance.ExplisionSound();
            Instantiate(explosionPrefab, position, Quaternion.identity);
        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            //save the position of the missile
            Vector3 position = transform.position;
            // make the position to the tip of the missile
            //position.y += 5.5f;

            Destroy(gameObject);
            //animator.SetTrigger("Hit");
            Debug.Log("Hit the ground");


            SoundManager.Instance.ExplisionSound();
            //explosionPrefab.GetComponent<SpriteRenderer>().sortingLayerName = "Projectiles";

            Instantiate(explosionPrefab, position, Quaternion.identity);
            Debug.Log("Explosion prefab instantiated at: " + position);
            //set sorting layer to the same as the missile
            //explosionPrefab.Animator.SetTrigger("Hit");
            // wait for the blow up animation and then destroy




        }
    }

}
