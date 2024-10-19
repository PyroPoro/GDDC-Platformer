using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingHinge : MonoBehaviour {
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Rigidbody2D hinge;
    [SerializeField] LineRenderer lr;
    Color activatedColor;
    Color deactivatedColor;
    Transform attachedObject;

    void Start() {
        activatedColor = sr.color;
        deactivatedColor = new(activatedColor.r, activatedColor.g, activatedColor.b, 0.5f);
        SetActive(false);
        lr.enabled = false;
    }
    void Update() {
        if (attachedObject != null) {
            UpdateLinePosition();
        }
    }

    public void SetActive(bool isActive) {
        sr.color = isActive ? activatedColor : deactivatedColor;
    }

    public Rigidbody2D GetHinge() {
        return hinge;
    }

    public void SetAttachedObject(Transform tr) {
        attachedObject = tr;
        lr.SetPositions(new Vector3[] { transform.position, attachedObject.position });
        lr.enabled = true;
    }

    public void DisconnectObject() {
        attachedObject = null;
        lr.SetPositions(new Vector3[0]);
        lr.enabled = false;
    }


    void UpdateLinePosition() {
        lr.SetPositions(new Vector3[] { transform.position, attachedObject.position });
    }
}
