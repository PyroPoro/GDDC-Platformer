using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    //[SerializeField] List<GameObject> backgrounds = new();
    [SerializeField] Transform cam;
    [SerializeField] float scrollingSpeed;
    [SerializeField] float backgroundWidth;
    Vector3 centerPosition;
    Vector3 camStartPos;
    Vector3 camDiff;
    float camPrev;
    void Start()
    {
        camPrev = cam.position.x;
        centerPosition = transform.position;
        camStartPos = cam.position;
    }

    void Update()
    {
        camDiff = cam.position - camStartPos;
        transform.position = camDiff * scrollingSpeed + centerPosition;
        float diff = cam.position.x * (1 - scrollingSpeed);

        if(diff > centerPosition.x + (backgroundWidth / 2)) {
            centerPosition += new Vector3(backgroundWidth,0 ,0);
        } else if (diff < centerPosition.x - (backgroundWidth / 2)) {
            centerPosition -= new Vector3(backgroundWidth,0 ,0);
        }
    }
}
