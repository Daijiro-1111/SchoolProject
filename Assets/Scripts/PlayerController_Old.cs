using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController_Old : AvatarController
{
    private GameObject _thisPlayer;
    private Rigidbody _rb;
    private Animator _animator;
    [SerializeField] private GameObject _punch;

    // �ړ��X�s�[�h
    private const float MOVE_SPEED = 10f;
    // �W�����v�����
    private const float JUMP_POWER = 9f;
    // �p���`�̗�
    private const float PUNCH_POWER = 10f;
    // �p���`���Ă��鎞��
    private const float PUNCH_TIME = 0.4f;
    // �p���`���󂯂ĉ����o����鎞��
    private const float PUNCH_PUSH_TIME = 0.3f;
    // �_���[�W���󂯂Ă��畜�A����܂ł̎���
    private const float STUN_TIME = 0.7f;
    // Ray�̒���
    private const float RAY_DISTANCE = 1.0f;
    // Ray�̈ʒu
    private readonly Vector3 RAY_POSITION = new Vector3(0, 1, 0);
    // BoxCast�̑傫��
    private const float BOX_CAST_SIZE = 0.9f;
    // ��]�̓K�����x
    private const float APPRY_SPEED = 0.2f;

    // �A�j���[�V�����̃p�����[�^
    public const string ANIM_PARAM = "PlayerAnimParam";
    #region �A�j���[�V�����p�����[�^�̔ԍ�
    public const int IDLE_NUM = 0;
    public const int RUN_NUM = 1;
    public const int JUMP_NUM = 2;
    public const int PUNCH_NUM = 3;
    public const int DAMAGE_NUM = 4;
    #endregion

    [SerializeField] private float _axisX;
    [SerializeField] private float _axisY;
    [SerializeField] private float _axisZ;
    [SerializeField] private RaycastHit _hit;
    [SerializeField] private bool _isGround;
    [SerializeField] private bool _isPunch;
    [SerializeField] private bool _isDamage;
    private Vector3 _punchDefaultPos;

    private const float JUMP_VELOCITY_RATE = 0.5f;
    [SerializeField] private float _lastPosY;
    // �������Ƀ_���[�W���󂯂鍂��
    private const float FALL_DAMAGE_DISTANCE = 5;

    [SerializeField] private Vector3 _gimicVelocity;

    public override void Start()
    {
        base.Start();

        _thisPlayer = this.gameObject;
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _punchDefaultPos = _punch.transform.localPosition;
    }

    public override void Update()
    {
        base.Update();

        if (photonView.IsMine)
        {
            if (!_isDamage)
            {
                if (!_isPunch)
                {
                    _axisX = Input.GetAxis("Horizontal") * MOVE_SPEED;
                    _axisZ = Input.GetAxis("Vertical") * MOVE_SPEED;
                }

                // �W�����v
                if (Input.GetKeyDown(KeyCode.Space) && _isGround)
                {
                    Jump();
                }

                // �p���`
                if (Input.GetMouseButtonDown(0) && !_isPunch)
                {
                    photonView.RPC("PlayerPunch", RpcTarget.All, PhotonNetwork.LocalPlayer.UserId);

                    StartCoroutine(Punch());
                }
            }
            return;
        }

        if (!PhotonNetwork.InRoom)
        {
            if (!_isDamage)
            {
                if (!_isPunch)
                {
                    _axisX = Input.GetAxis("Horizontal") * MOVE_SPEED;
                    _axisZ = Input.GetAxis("Vertical") * MOVE_SPEED;
                }

                // �W�����v
                if (Input.GetKeyDown(KeyCode.Space) && _isGround)
                {
                    Jump();
                }

                // �p���`
                if (Input.GetMouseButtonDown(0) && !_isPunch)
                {
                    PlayerPunch("");
                }
            }
            return;
        }

        if (!photonView.IsMine)
        {
            if (_isPunch)
            {
                StartCoroutine(Punch());
            }
        }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine || !PhotonNetwork.InRoom)
        {
            if (!_isDamage)
            {
                // �J�����̐��ʂ�O���Ƃ��Ĉړ�����
                Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
                Vector3 moveForward = cameraForward * _axisZ + Camera.main.transform.right * _axisX;

                //Vector3 all = moveForward * MOVE_SPEED + _gimicVelocity;

                _rb.velocity = moveForward + new Vector3(0, _rb.velocity.y, 0);

                // ������]
                Vector3 diff = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
                if (diff.magnitude > 0.01f)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                        Quaternion.LookRotation(diff), APPRY_SPEED);
                }

                if (!_isGround)
                {
                    // �W�����v���Ɉړ������։����͂𒲐�
                    _rb.velocity = new Vector3(_rb.velocity.x * JUMP_VELOCITY_RATE, _rb.velocity.y, _rb.velocity.z * JUMP_VELOCITY_RATE);
                }
                else
                {
                    // �~�܂��Ă�Ƃ�
                    if (_rb.velocity == Vector3.zero)
                    {
                        _animator.SetInteger(ANIM_PARAM, IDLE_NUM);
                    }
                    // �����Ă�Ƃ�
                    else
                    {
                        _animator.SetInteger(ANIM_PARAM, RUN_NUM);
                    }

                    // �����_���[�W
                    if (_lastPosY - transform.position.y > FALL_DAMAGE_DISTANCE) { StartCoroutine(Damage(null)); }
                    _lastPosY = transform.position.y;
                }
            }

            // ���n����
            _isGround = Physics.BoxCast(transform.position + RAY_POSITION, Vector3.one * BOX_CAST_SIZE, -transform.up, out _hit, Quaternion.identity, RAY_DISTANCE);
        }
    }

    /// <summary>
    /// Ray��`��
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, -transform.up * _hit.distance);
        Gizmos.DrawWireCube(transform.position + -transform.up * _hit.distance, Vector3.one * BOX_CAST_SIZE);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Punch") && !_isDamage && other.transform.parent != _thisPlayer)
        {
            StartCoroutine(Damage(other));
        }
    }

    [PunRPC]
    private void PlayerPunch(string id)
    {
        _isPunch = true;

        OutputDebug($"attack:{id}");
    }

    private IEnumerator Punch()
    {
        _animator.SetInteger(ANIM_PARAM, PUNCH_NUM);
        _punch.transform.localPosition = _punchDefaultPos;
        _punch.SetActive(true);
        Stop();

        yield return new WaitForSeconds(PUNCH_TIME);

        PunchFinish();
        photonView.RPC("PunchFin", RpcTarget.All);
    }

    /// <summary>
    /// �p���`���I�������鏈��
    /// </summary>
    private void PunchFinish()
    {
        _punch.transform.position = _punchDefaultPos;
        _punch.SetActive(false);
        _animator.SetInteger(ANIM_PARAM, IDLE_NUM);
    }

    [PunRPC]
    private void PunchFin()
    {
        _isPunch = false;
    }

    private IEnumerator Damage(Collider other)
    {
        if (_isPunch)
        {
            photonView.RPC("PunchFin", RpcTarget.All);
            PunchFinish();
        }
        _animator.SetInteger(ANIM_PARAM, DAMAGE_NUM);
        _isDamage = true;
        Stop();
        if (other != null)
        {
            _rb.AddForce(other.transform.forward * PUNCH_POWER, ForceMode.VelocityChange);
        }

        yield return new WaitForSeconds(PUNCH_PUSH_TIME);
        Stop();
        yield return new WaitForSeconds(STUN_TIME);

        _isDamage = false;
        _animator.SetInteger(ANIM_PARAM, IDLE_NUM);
        yield break;
    }

    private void Stop()
    {
        _axisX = 0;
        _axisZ = 0;

        if (!_isPunch) { _rb.velocity = Vector3.zero; }
    }

    public void AllStop()
    {
        _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
        _animator.SetInteger(ANIM_PARAM, IDLE_NUM);
    }

    private void Jump()
    {
        _animator.SetInteger(ANIM_PARAM, JUMP_NUM);
        _rb.AddForce(transform.up * JUMP_POWER, ForceMode.VelocityChange);
    }

    /*public void Gimic(Vector3 gimicVelo, float time)
    {

        StartCoroutine(GimicVelocityTime(gimicVelo, time));

    }
    public IEnumerator GimicVelocityTime(Vector3 gimicVelo, float time)
    {
        _gimicVelocity = gimicVelo;
        yield return new WaitForSeconds(time);
        _gimicVelocity = Vector3.zero;
        _rb.velocity = Vector3.zero;
        yield break;
    }*/
}
