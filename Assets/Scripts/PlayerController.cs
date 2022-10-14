using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// �v���C���[�p
/// </summary>
public class PlayerController : AvatarController
{
    private GameObject _thisPlayer;
    private Rigidbody _rb;
    private Animator _animator;
    [SerializeField] private GameObject _punch;

    // �ړ��X�s�[�h
    private const float MOVE_SPEED = 7.0f;
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
    private readonly Vector3 RAY_POSITION = new Vector3(0, 1.5f, 0);
    // BoxCast�̑傫��
    private const float BOX_CAST_SIZE = 0.9f;
    // ��]�̓K�����x
    private const float APPRY_SPEED = 0.2f;

    // �A�j���[�V�����̃p�����[�^
    private const string ANIM_PARAM = "PlayerAnimParam";
    #region �A�j���[�V�����p�����[�^�̔ԍ�
    private const int IDLE_NUM = 0;
    private const int RUN_NUM = 1;
    private const int JUMP_NUM = 2;
    private const int PUNCH_NUM = 3;
    private const int DAMAGE_NUM = 4;
    private const int RECOVER_NUM = 5;
    #endregion
    private const string ANIM_ISGROUND = "isGround";

    [SerializeField] private float _axisX;
    [SerializeField] private float _axisZ;
    [SerializeField] private RaycastHit _hit;
    [SerializeField] private bool _isGround;
    [SerializeField] private bool _isPunch;
    [SerializeField] private bool _isDamage;
    private Vector3 _punchPos;
    private Vector3 _respawnPos;

    private const float JUMP_VELOCITY_RATE = 0.5f;
    [SerializeField] private float _lastPosY;
    // �������Ƀ_���[�W���󂯂鍂��
    private const float FALL_DAMAGE_DISTANCE = 5;

    [SerializeField] private Vector3 _gimicVelocity;

    #region ��Q������
    private int _areaCount = 1;
    // �L�����f�B�̃��C���[�̂ݖ���(BoxCast�Ɏg�p)
    private LayerMask _layerMask = ~(1 << 8);
    #endregion

    private bool _isGoal = false;

    public bool isGoal
    {
        get { return _isGoal; }
        set { _isGoal = value; }
    }

    public override void Start()
    {
        base.Start();

        _isGoal = false;

        _thisPlayer = this.gameObject;
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _punchPos = _punch.transform.localPosition;
        _respawnPos = transform.position;
    }

    public override void Update()
    {
        if (photonView.IsMine)
        {
            base.Update();
            if (!_isDamage)
            {
                if (!_isPunch)
                {
                    _axisX = Input.GetAxis(InputNameData.INPUT_HORIZONTAL);
                    _axisZ = Input.GetAxis(InputNameData.INPUT_VERTICAL);
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
                    _axisX = Input.GetAxis(InputNameData.INPUT_HORIZONTAL);
                    _axisZ = Input.GetAxis(InputNameData.INPUT_VERTICAL);
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
            base.Update();
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
                Vector3 moveForward = Vector3.zero;
                if (_gimicVelocity == Vector3.zero) { moveForward = cameraForward * _axisZ + Camera.main.transform.right * _axisX; }
                else { moveForward = _gimicVelocity + Camera.main.transform.right * _axisX; }

                Vector3 horizontalVelocity = moveForward * MOVE_SPEED;

                _rb.velocity = horizontalVelocity + new Vector3(0, _rb.velocity.y, 0);

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
                    if (_lastPosY - transform.position.y > FALL_DAMAGE_DISTANCE) { StartCoroutine(Damage(Vector3.zero)); }
                    _lastPosY = transform.position.y;
                }
            }

            // ���n����
            _isGround = Physics.BoxCast(transform.position + RAY_POSITION, Vector3.one * BOX_CAST_SIZE, -transform.up, out _hit, Quaternion.identity, RAY_DISTANCE, _layerMask);
            _animator.SetBool(ANIM_ISGROUND, _isGround);
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
        if (!_isDamage && other.transform.parent != _thisPlayer)
        {
            // �p���`�����������
            if (other.CompareTag(TagData.PUNCH_TAG))
            {
                StartCoroutine(Damage(other.transform.forward));
            }
        }

        // �G���A�O�ɍs������
        if (other.transform.CompareTag(TagData.DEATH_AREA_TAG))
        {
            transform.position = _respawnPos;
            Stop();
            _isDamage = true;
            _animator.SetInteger(ANIM_PARAM, RECOVER_NUM);
        }
        // ��񃊃X�|�[���n�_�ɍs������
        else if (other.transform.CompareTag(TagData.SECOND_RESPAWN_AREA) && _areaCount != 2)
        {
            _respawnPos = transform.position;
            _areaCount++;
        }

        // �S�[�������Ƃ�
        if (other.transform.CompareTag(TagData.GOAL_AREA) && _areaCount == 2)
        {
            _isGoal = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!_isDamage)
        {
            // �L�����f�B�ɓ���������
            if (collision.transform.CompareTag(TagData.CANDY_TAG))
            {
                StartCoroutine(Damage(collision.transform.GetComponent<Rigidbody>().velocity.normalized));
            }
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
        _punch.transform.localPosition = _punchPos;
        _punch.SetActive(true);
        Stop();

        yield return new WaitForSeconds(PUNCH_TIME);

        PunchFinish();
        photonView.RPC("PunchFin", RpcTarget.All);
    }

    [PunRPC]
    private void PunchFin()
    {
        _isPunch = false;
    }
    /// <summary>
    /// �p���`���I�������鏈��
    /// </summary>
    private void PunchFinish()
    {
        _punch.transform.position = _punchPos;
        _punch.SetActive(false);
        if (!_isDamage) { _animator.SetInteger(ANIM_PARAM, IDLE_NUM); }
    }

    private IEnumerator Damage(Vector3 forward)
    {
        if (_isPunch)
        {
            photonView.RPC("PunchFin", RpcTarget.All);
            PunchFinish();
        }
        _animator.SetInteger(ANIM_PARAM, DAMAGE_NUM);
        _isDamage = true;
        Stop();

        if (forward != null)
        {
            // �m�b�N�o�b�N
            _rb.AddForce(forward * PUNCH_POWER, ForceMode.VelocityChange);
        }

        yield return new WaitForSeconds(PUNCH_PUSH_TIME);

        Stop();
        //yield return new WaitForSeconds(STUN_TIME);
        yield break;
    }

    private void DamageFinish()
    {
        _animator.SetInteger(ANIM_PARAM, IDLE_NUM);
        _isDamage = false;
    }

    public void Stop()
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

    public void Gimic(float gimicPowerZ, float gimicPowerY, float time)
    {
        StartCoroutine(BeBlownAwayGimicTime(gimicPowerZ, gimicPowerY, time));
    }
    /// <summary>
    /// �M�~�b�N�ɂ���Ăӂ��Ƃ΂���鎞��
    /// </summary>
    /// <param name="gimicPowerZ">Z�����ɗ^�����</param>
    /// <param name="gimicPowerY">Y�����ɗ^�����</param>
    /// <param name="time">�͂�^�������鎞��</param>
    /// <returns></returns>
    public IEnumerator BeBlownAwayGimicTime(float gimicPowerZ, float gimicPowerY, float time)
    {
        if (_axisZ < 0) { gimicPowerZ = -gimicPowerZ; }
        _gimicVelocity = Vector3.forward * gimicPowerZ;
        _rb.AddForce(transform.up * gimicPowerY, ForceMode.VelocityChange);

        yield return new WaitForSeconds(time);

        _gimicVelocity = Vector3.zero;
        _rb.velocity = Vector3.zero;
        yield break;
    }
}