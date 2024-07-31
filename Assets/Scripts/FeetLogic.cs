using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetLogic : MonoBehaviour
{
    public PlayerLogic1 playerLogic1;

    private void Start()
    {
        if (playerLogic1 == null)
        {
            playerLogic1 = GetComponentInParent<PlayerLogic1>();
            if (playerLogic1 == null)
            {
                Debug.LogError("PlayerLogic1 component not found in parent.");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (playerLogic1 != null && playerLogic1.photonView.IsMine)
        {
            playerLogic1.puedoSaltar = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerLogic1 != null && playerLogic1.photonView.IsMine)
        {
            playerLogic1.puedoSaltar = false;
        }
    }
}
