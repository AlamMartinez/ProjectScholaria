using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager
{
    private Camera camera;
    private Transform focusTransform;
    private Transform cameraPivot;
    public int translateSpeed = 10;
    public int rotateSpeed = 10;
    public int zoomSpeed = 10;
    public float cameraDistance = 10;


    public CameraManager(Transform focusTransform, Transform cameraPivot)
    {
        camera = Camera.main;
        this.focusTransform = focusTransform;
        this.cameraPivot = cameraPivot;
    }

    public void TranslateCamera(float x, float y)
    {
        focusTransform.Translate(new Vector3(x * translateSpeed * Time.deltaTime, 0, y * translateSpeed * Time.deltaTime));
    }

    public void PanCamera(float a)
    {
        Vector3 angles = focusTransform.eulerAngles;
        angles.y += a * rotateSpeed * Time.deltaTime;
        focusTransform.eulerAngles = angles;
    }

    public void TiltCamera(float a)
    {
        Vector3 angles = focusTransform.eulerAngles;
        angles = cameraPivot.eulerAngles;
        angles.x += -1 * a * rotateSpeed * Time.deltaTime;
        angles.x = Mathf.Clamp(angles.x, 275, 355);
        cameraPivot.eulerAngles = angles;
    }

    public void ZoomCamera(float a)
    {
        if ((cameraDistance > 1 && a > 0) || (cameraDistance <= 50 && a < 0))
        {
            camera.GetComponent<Transform>().Translate(new Vector3(0, 0, a * zoomSpeed * Time.deltaTime));
            cameraDistance -= a * zoomSpeed * Time.deltaTime;
        }
    }
}
