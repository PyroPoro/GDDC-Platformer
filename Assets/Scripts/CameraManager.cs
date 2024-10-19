using System.Collections;
using UnityEngine;


public class CameraManager : MonoBehaviour {
    [SerializeField] Transform cam;
    [SerializeField] PlayerMovement pm;
    [SerializeField] float cameraShakeStrength;
    [SerializeField] int cameraShakeNumFrames;
    [SerializeField] float cameraShakeTimeInterval;
    ACameraZone currentCameraZone;

    void Start(){
        pm.OnDashStart += DashCameraShake;
    }

    void OnTriggerEnter2D(Collider2D col){
        if(col.CompareTag("CameraZone")){
            ACameraZone cameraZone = col.GetComponent<ACameraZone>();
            if(currentCameraZone != cameraZone) {
                currentCameraZone = cameraZone;
            }
            currentCameraZone.OnEnterCameraZone(cam);
        }
    }

    void DashCameraShake(){
        StartCoroutine(ShakeCamera(cameraShakeNumFrames));
    }

    IEnumerator ShakeCamera(int numFrames){
        Vector3 startPos = cam.position;
        for(int i = 0; i < numFrames; i++) {
            float angle = Random.Range(0f, 360f);
            float dis = Random.Range(0.5f,1f) * cameraShakeStrength;
            Vector3 offset = Quaternion.Euler(0,0,angle) * Vector3.right * dis;
            cam.position = startPos + offset;
            yield return new WaitForSeconds(cameraShakeTimeInterval);
        }
        cam.position = startPos;
    }
}