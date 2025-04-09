using UnityEngine;

namespace InteractableItemsScripts
{
    public class PowerOutletAnimator : MonoBehaviour
    {
        private IPowerOutlet _outlet;
        private Animator _animator;
        private static readonly int PluggedIn = Animator.StringToHash("PluggedIn");

        void Awake() => _outlet = GetComponentInParent<IPowerOutlet>();
        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            _animator.SetBool(PluggedIn, _outlet.PluggedIn);
        }
    }
}
