using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignTextBubble : MonoBehaviour {
    [SerializeField] SpriteRenderer topBubble;
    [SerializeField] SpriteRenderer botBubble;
    [SerializeField] TMP_Text text;
    [SerializeField] float fadeDuration;

    IEnumerator textBubbleToggle;

    public void ToggleTextBubble(bool isShow) {
        if (textBubbleToggle != null) {
            StopCoroutine(textBubbleToggle);
        }
        if (isShow) {
            textBubbleToggle = ShowTextBubble();
        } else {
            textBubbleToggle = HideTextBubble();
        }
        StartCoroutine(textBubbleToggle);
    }

    void UpdateBubbleAlpha(float a) {
        Color c = topBubble.color;
        topBubble.color = new Color(c.r, c.g, c.b, a);
        c = botBubble.color;
        botBubble.color = new Color(c.r, c.g, c.b, a);
        c = text.color;
        text.color = new Color(c.r, c.g, c.b, a);
    }
    IEnumerator ShowTextBubble() {
        float timer = 0;
        float startA = topBubble.color.a;
        while (timer < fadeDuration) {
            float a = Mathf.Lerp(startA, 1, timer / fadeDuration);
            UpdateBubbleAlpha(a);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        UpdateBubbleAlpha(1);
    }

    IEnumerator HideTextBubble() {
        float timer = 0;
        float startA = topBubble.color.a;
        while (timer < fadeDuration) {
            float a = Mathf.Lerp(startA, 0, timer / fadeDuration);
            UpdateBubbleAlpha(a);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        UpdateBubbleAlpha(0);
    }
}
