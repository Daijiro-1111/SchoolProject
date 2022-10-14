using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private GameObject _camera;
    private GameObject _player;
    private readonly Vector3 CAMERA_POS = new Vector3(0.0f, 5.0f, -9.1f);
    private const float CAMERA_ROTATE_SPEED = 2.0f;
    [SerializeField] private Vector3 _playerLastPos;

    [SerializeField] private bool _isCameraMoveStart;

    [SerializeField]private float _rotateV;
    private const float MIN_ANGLE_V = -30;
    private const float MAX_ANGLE_V = 10;

    private bool _isPlayerMove = false;

    public bool isPlayerMove
    {
        get { return _isPlayerMove = false; }
        set { _isPlayerMove = value; }
    }

    void Start()
    {
        _camera = Camera.main.gameObject;
    }

    void FixedUpdate()
    {
        if (!_isPlayerMove) { return; }
        else
        {
            if(_player == null)
            {
                _player = GameObject.FindWithTag("Player");

                _camera.transform.position = _player.transform.position + CAMERA_POS;
                _camera.transform.rotation = Quaternion.Euler(20, 0, 0);

                _playerLastPos = _player.transform.position;
            }
        }

        _camera.transform.position += _player.transform.position - _playerLastPos;
        _playerLastPos = _player.transform.position;

        /*float angle = Input.GetAxis("Mouse X") * CAMERA_ROTATE_SPEED;
        _camera.transform.RotateAround(_player.transform.position, Vector3.up, angle);*/

        Vector3 angle = new Vector3(Input.GetAxis("Mouse X") * CAMERA_ROTATE_SPEED,
            Input.GetAxis("Mouse Y") * CAMERA_ROTATE_SPEED, 0);

        float deltaAngleV = Input.GetAxis("Mouse Y") * CAMERA_ROTATE_SPEED;

        _rotateV += deltaAngleV;

        float clampV = Mathf.Clamp(_rotateV, MIN_ANGLE_V, MAX_ANGLE_V);

        float overV = _rotateV - clampV;

        deltaAngleV -= overV;

        _rotateV = clampV;

        if (_isCameraMoveStart)
        {
            _camera.transform.RotateAround(_player.transform.position, Vector3.up, angle.x);
            _camera.transform.RotateAround(_player.transform.position, -_camera.transform.right, deltaAngleV);
        }
    }
}
