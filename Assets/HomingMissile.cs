using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingMissile : MonoBehaviour
{
    // Start is called before the first frame update
    [FormerlySerializedAs("_speed")] [SerializeField] private float speed = 9f;
    [FormerlySerializedAs("_rotateSpeed")] [SerializeField] private float rotateSpeed = 120f;
    [SerializeField] private GameObject explosionPrefab;
    private static readonly int Close = Animator.StringToHash("Hit");

    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private Transform _target;
    private float _lifetime;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _lifetime = 0f;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // Self-destruct after 10 seconds to avoid stuck missiles
        _lifetime += Time.fixedDeltaTime;
        if (_lifetime > 10f)
        {
            Destroy(gameObject);
            return;
        }

        if (!_target)
        {
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0;
            return;
        }

        var direction = (Vector2)_target.position - _rigidbody2D.position;
        direction.Normalize();
        var rotateAmount = Vector3.Cross(direction, transform.up).z;
        _rigidbody2D.angularVelocity = -rotateAmount * rotateSpeed;
        _rigidbody2D.velocity = transform.up * speed;
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
            var position = transform.position;
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
