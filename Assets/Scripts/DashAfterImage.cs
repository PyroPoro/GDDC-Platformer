using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAfterImage : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] float duration;
    Color c;
    public void Initialize(Sprite sprite, Color color){
        sr.sprite = sprite;
        c = color;
        StartCoroutine(LerpAlpha());
    }

    IEnumerator LerpAlpha(){
        float timer = 0;
        float startAlpha = c.a / 3f;
        while (timer < duration){
            Color newC = new(c.r, c.g, c.b, Mathf.Lerp(startAlpha, 0, timer/duration));
            sr.color = newC;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
