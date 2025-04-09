using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutletScript : MonoBehaviour
{
    // has access to boss and player
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject player;
    [SerializeField] Animator _animator;
    private static readonly int PluggedOut = Animator.StringToHash("PluggedOut");
    private static bool _pluggedIn = true;

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < 1 && _pluggedIn)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                _animator.SetTrigger(PluggedOut);
                boss.GetComponent<Animator>().SetTrigger(PluggedOut);
                _pluggedIn = false;
            }

        }

    }
}