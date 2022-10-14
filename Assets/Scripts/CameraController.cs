using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject _player;

    private readonly Vector3 CAMERA_POS = new Vector3(0.0f, 5.0f, -9.1f);
    private readonly Quaternion CAMERA_DEFAULT_ANGLE = Quaternion.Euler(20, 0, 0);
    private const float CAMERA_ROTATE_SPEED = 2.0f;
    private const float MIN_ANGLE_V = -30;
    private const float MAX_ANGLE_V = 10;

    [SerializeField] private Vector3 _playerLastPos;
    [SerializeField] private bool _isMouseCameraMove;
    [SerializeField] private float _rotateV;

    private bool _isPlayerMove = false;

    public bool isPlayerMove
    {
        get { return _isPlayerMove = false; }
        set { _isPlayerMove = value; }
    }

    void Start()
    {
        _player = GameObject.FindWithTag(TagData.PLAYER_TAG);
    }

    void FixedUpdate()
    {
        if (!_isPlayerMove) { return; }

        if (_player == null)
        {
            _player = GameObject.FindWithTag(TagData.PLAYER_TAG);

            transform.position = _player.transform.position + CAMERA_POS;
            transform.rotation = CAMERA_DEFAULT_ANGLE;

            _playerLastPos = _player.transform.position;
        }

        // プレイヤー位置を追跡
        transform.position += _player.transform.position - _playerLastPos;
        _playerLastPos = _player.transform.position;

        // マウスの動きを取得
        Vector3 angle = new Vector3(Input.GetAxis(InputNameData.INPUT_MOUSE_X) * CAMERA_ROTATE_SPEED,
            Input.GetAxis(InputNameData.INPUT_MOUSE_Y) * CAMERA_ROTATE_SPEED, 0);

        float deltaAngleV = Input.GetAxis(InputNameData.INPUT_MOUSE_Y) * CAMERA_ROTATE_SPEED;

        _rotateV += deltaAngleV;

        // 角度制限
        float clampV = Mathf.Clamp(_rotateV, MIN_ANGLE_V, MAX_ANGLE_V);

        float overV = _rotateV - clampV;

        deltaAngleV -= overV;

        _rotateV = clampV;

        if (_isMouseCameraMove)
        {
            transform.RotateAround(_player.transform.position, Vector3.up, angle.x);
            transform.RotateAround(_player.transform.position, -transform.right, deltaAngleV);
        }
    }

    /// <summary>
    /// マウスでカメラを動かせるようにする
    /// </summary>
    public void MouseCameraMoveStart()
    {
        _isMouseCameraMove = true;
    }
}
