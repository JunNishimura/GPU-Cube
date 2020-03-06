using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 LookTargetPos;
    public Vector2 Radius = new Vector2(150, 200);
    public float UpDownSpeed = 1f;
    public float AroundSpeed = 1f;

    private float elapsedTime = 0f;

    private void Update()
    {
        float theta  = elapsedTime * UpDownSpeed * Mathf.Deg2Rad % Mathf.PI * 2.0f;
        float phi    = elapsedTime * AroundSpeed * Mathf.Deg2Rad;
        float radius = Mathf.PingPong(elapsedTime, Radius.y - Radius.x + 0.01f) + Radius.x;
        //if (theta > Mathf.PI / 2 || theta > Mathf.PI / 2 * 3)
        //{
        //    phi += Mathf.PI;
        //}
        Vector3 newPos = new Vector3
        (
            radius * Mathf.Cos(theta) * Mathf.Sin(phi),
            radius * Mathf.Sin(theta),
            radius * Mathf.Cos(theta) * Mathf.Cos(phi)
        );
        transform.position = newPos;
        transform.LookAt(LookTargetPos);

        elapsedTime += Time.deltaTime;
    }
}
