using UnityEngine;

namespace EnemyScripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class HomingMissile : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _rotateSpeed = 200f;
        private Rigidbody2D _rigidbody2D;
        private Transform _target;
        void Start()
        {
        
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
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
                Explode();
            } else if (other.gameObject.CompareTag("Ground"))
            {
                Explode();
            }
        }
    
        private void Explode()
        {
            // play missile explosion animation and destroy the missile
            gameObject.GetComponent<Animation>().Play();
            SoundManager.Instance.ExplisionSound();
            Destroy(gameObject);
        }
  
    }
}
