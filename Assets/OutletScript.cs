using UnityEngine;
using UnityEngine.Serialization;

public class OutletScript : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject player;
    [FormerlySerializedAs("_animator")] [SerializeField] private Animator animator;
    private static readonly int PluggedOut = Animator.StringToHash("PluggedOut");
    private bool _pluggedIn = true;
    private BossScript _bossScript;

    private void Start()
    {
        _bossScript = boss.GetComponent<BossScript>();
    }

    private void Update()
    {
        if (!(Vector2.Distance(transform.position, player.transform.position) < 1.5f) || !_pluggedIn) return;
        var interact = Input.GetKeyDown(KeyCode.E);
        if (MobileControls.Instance)
            interact = interact || MobileControls.Instance.InteractDown;
        if (!interact) return;
        animator.SetTrigger(PluggedOut);
        _pluggedIn = false;

        if (_bossScript) _bossScript.Unplug();
    }
}
