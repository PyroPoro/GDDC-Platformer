using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveController : MonoBehaviour {
    [SerializeField] float shockwaveDuration;
    [SerializeField] AnimationCurve shockwaveDistance;
    Material mat;
    void Start() {
        mat = GetComponent<SpriteRenderer>().material;
        StartCoroutine(Shockwave());
    }

    IEnumerator Shockwave() {
        float timer = 0;
        float disFromCenter = -0.1f;
        while (timer < shockwaveDuration) {
            disFromCenter = Mathf.Lerp(-0.1f, 1f, shockwaveDistance.Evaluate(timer / shockwaveDuration));
            mat.SetFloat("_WaveDistanceFromCenter", disFromCenter);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
