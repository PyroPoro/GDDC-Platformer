using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACameraZone : MonoBehaviour {
    public float cameraTransitionDuration;
    IEnumerator cameraLerpCoroutine;
    public float cameraSmoothTime;
    public void OnEnterCameraZone(Transform camera) {
        if (cameraLerpCoroutine != null) {
            StopCoroutine(cameraLerpCoroutine);
        }
        cameraLerpCoroutine = MoveCamera(transform.position, camera);
        StartCoroutine(cameraLerpCoroutine);
    }
    public void UpdateCameraPosition() {

    }

    IEnumerator MoveCamera(Vector3 targetPos, Transform cam) {
        float timer = 0;
        Vector3 velocity = Vector3.zero;
        while (timer < cameraTransitionDuration) {
            cam.transform.position = Vector3.SmoothDamp(cam.position, targetPos, ref velocity, cameraSmoothTime);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
