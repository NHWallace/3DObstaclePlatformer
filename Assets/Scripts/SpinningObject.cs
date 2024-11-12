using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningObject : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeeds = Vector3.zero;

    private void Update() {
        transform.Rotate(rotationSpeeds * Time.deltaTime);
    }
}
