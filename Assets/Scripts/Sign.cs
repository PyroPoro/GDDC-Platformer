using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour {
    [SerializeField] SignTextBubble textBubble;
    [SerializeField] float bobHeight;
    [SerializeField] float bobSpeed;
    bool isPlayerNearby;
    float timer = 0;
    Vector3 startPos;
    void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("Player")) {
            timer = 0;
            isPlayerNearby = true;
            textBubble.ToggleTextBubble(true);
        }
    }
    void OnTriggerExit2D(Collider2D col) {
        if (col.CompareTag("Player")) {
            isPlayerNearby = false;
            textBubble.ToggleTextBubble(false);
        }
    }

    void Start() {
        startPos = textBubble.transform.position;
    }

    void Update() {
        if (isPlayerNearby) {
            timer += Time.deltaTime * bobSpeed;
            textBubble.transform.position = startPos + new Vector3(0, Mathf.Sin(timer), 0) * bobHeight;
        }
    }
}
