using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovingPlatform : MonoBehaviour {
    [SerializeField] private Transform platform;
    [SerializeField] private GameObject[] pointsToMoveTo;
    [SerializeField] private float moveSpeed = 2f;
    private Vector3[] positionsToMoveTo;
    private int nextPositionIndex = 0;
    private Vector3 nextPosition;
    private Vector3 moveVelocity;


    private void Start() {
        positionsToMoveTo = new Vector3[pointsToMoveTo.Length];
        for (int i = 0; i < pointsToMoveTo.Length; i++) {
            positionsToMoveTo[i] = pointsToMoveTo[i].transform.position;
        }
    }

    void FixedUpdate() {
        // Vector3 cannot be null, default value is instead Vector3.zero
        if (nextPosition == Vector3.zero) {
            nextPosition = positionsToMoveTo[0];
        }

        if (positionsToMoveTo.Length < 2) {
            Debug.Log("Not enough positions have been set for moving platform with name " + platform.name);
        }

        Vector3 moveDir = (nextPosition - platform.position).normalized;
        moveVelocity = moveDir * moveSpeed;

        platform.position += moveVelocity * Time.deltaTime;

        if (Vector3.Distance(platform.position, nextPosition) <= 0.1) {
            // move to the next point, or towards the starting point if there are no more points in the list
            nextPositionIndex = (nextPositionIndex + 1 == positionsToMoveTo.Length) ? 0 : nextPositionIndex + 1;
            nextPosition = positionsToMoveTo[nextPositionIndex];
        }
    }

    void OnTriggerStay(Collider col) {
        if (col.gameObject.GetComponent<PlayerController>() != null) {
            col.transform.parent = this.transform;
        }
    }

    void OnTriggerExit(Collider col) {
        col.transform.parent = null;
    }

}
