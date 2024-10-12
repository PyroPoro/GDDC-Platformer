using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    bool isIgnored = false;

    public void DisableCollision(Collider2D other){
        if(isIgnored) return;
        Physics2D.IgnoreCollision(other, GetComponent<Collider2D>(), true);
        isIgnored = true;
        StartCoroutine(EnableCollision(other));
    }

    IEnumerator EnableCollision(Collider2D other) {
        yield return new WaitForSeconds(0.1f);
        Physics2D.IgnoreCollision(other, GetComponent<Collider2D>(), false);
        isIgnored = false;
    }
}
