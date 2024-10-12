using UnityEngine;


public class CameraManager : MonoBehaviour {
    [SerializeField] Transform cam;
    ACameraZone currentCameraZone;
    void OnTriggerEnter2D(Collider2D col){
        if(col.CompareTag("CameraZone")){
            ACameraZone cameraZone = col.GetComponent<ACameraZone>();
            if(currentCameraZone != cameraZone) {
                currentCameraZone = cameraZone;
            }
            currentCameraZone.OnEnterCameraZone(cam);
        }
    }

}