using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillWall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.TryGetComponent<PlayerController>(out PlayerController player)) {
            player.Die();
        }
    }
}
