using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeelOverFloor : MonoBehaviour
{ 
    private float _minRotation = 0;
    private const float MAX_ROTATION = 20;

    void Start()
    {
        _minRotation = 360 - MAX_ROTATION;
    }

    void FixedUpdate()
    {
        Vector3 rocalAngle = transform.localEulerAngles;

        // ‰ñ“]§ŒÀ
        if (rocalAngle.x > MAX_ROTATION && rocalAngle.x < 180) { rocalAngle.x = MAX_ROTATION; }
        if (rocalAngle.x < _minRotation && rocalAngle.x > 180) { rocalAngle.x = _minRotation; }

        rocalAngle.y = 0;

        if (rocalAngle.z > MAX_ROTATION && rocalAngle.z < 180) { rocalAngle.z = MAX_ROTATION; }
        if (rocalAngle.z < _minRotation && rocalAngle.z > 180) { rocalAngle.z = _minRotation; }
        
        transform.localEulerAngles = rocalAngle;
    }
}
