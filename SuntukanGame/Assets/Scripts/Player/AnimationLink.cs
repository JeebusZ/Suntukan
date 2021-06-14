using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AnimationLink : MonoBehaviour
{
    private PlayerController pControl;

    private void Awake()
    {
        if(pControl == null)
        {
            pControl = GetComponentInParent<PlayerController>();
        }
    }

    public void RestartAttack()
    {
        pControl.ResetPlayerViaAnimation();
    }
}
