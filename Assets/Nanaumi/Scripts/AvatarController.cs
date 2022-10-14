using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class AvatarController : DebugParent,IPunObservable
{
    #region enum

    #endregion

    #region const
    private const float COMPLETION_DISTANCE_TIME = 0.2f; // �⊮�ɂ����鎞�ԊԊu
    #endregion

    #region public property

    #endregion

    #region private property
    //[SerializeField] private GameObject prefab;
    //private int score = 0;

    /* ���W�����p */
    private Vector3 _p1 = Vector3.zero;
    private Vector3 _p2 = Vector3.zero;
    /* ��]�����p */
    //private Vector3 _r1 = Vector3.zero;
    //private Vector3 _r2 = Vector3.zero;
    private float _elapsedDeltaTime = 0f;
    #endregion

    #region event
    public virtual void Start()
    {
        _p1 = this.transform.position;
        _p2 = _p1;

        //_r1 = this.transform.localEulerAngles;
        //_r2 = _r1;

        _elapsedDeltaTime = Time.deltaTime;
    }

    public virtual void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            if (photonView.IsMine)
            {
                // �����q�@
                /* ���g�̃l�b�g���[�N�I�u�W�F�N�g�́A���t���[���̈ړ��ʂƌo�ߎ��Ԃ��L�^���� */
                _p1 = _p2;
                _p2 = this.transform.position;

                //_r1 = _r2;
                //_r2 = this.transform.localEulerAngles;

                _elapsedDeltaTime = Time.deltaTime;

                //Init();
            }
            else
            {
                _elapsedDeltaTime += Time.deltaTime;
                this.transform.position = Vector3.Lerp(_p1, _p2, _elapsedDeltaTime / COMPLETION_DISTANCE_TIME);
                //this.transform.localEulerAngles = Vector3.Lerp(_r1, _r2, _elapsedDeltaTime / COMPLETION_DISTANCE_TIME);
            }
        }
    }

    //private void Init()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        //Instantiate(prefab,this.transform);
    //        PhotonNetwork.LocalPlayer.SetScore(score++);
    //    }
    //}

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ���`�⊮
            /* ���g�̏��𑗐M���� */
            stream.SendNext(this.transform.position);
            //stream.SendNext(this.transform.localEulerAngles);

            // �����q�@
            /* ���t���[���̈ړ��ʂƌo�ߎ��Ԃ���A�b�������߂đ��M���� */
            stream.SendNext((_p2 - _p1) / _elapsedDeltaTime);
            //stream.SendNext((_r2 - _r1) / _elapsedDeltaTime);
        }
        else
        {
            /* ���v���C���[�̏�����M���� */
            //this.transform.position = (Vector3)stream.ReceiveNext();

            //// ���`�⊮
            ///* ��M���̍��W���A�⊮�̊J�n���W�ɂ��� */
            //_p1 = this.transform.position;
            ///* ��M�������W���A�⊮�̏I�����W�ɂ��� */
            //_p2 = (Vector3)stream.ReceiveNext();
            ///* �o�ߎ��Ԃ̃��Z�b�g */
            //_elapsedDeltaTime = 0f;

            var networkPosition = (Vector3)stream.ReceiveNext();
            var networkVelocity = (Vector3)stream.ReceiveNext();
            if (float.IsNaN(networkVelocity.x)) { networkVelocity = Vector3.one; } // ��O����
            //var networkEulerAngles = (Vector3)stream.ReceiveNext();
            //var networkAngulerVelocity = (Vector3)stream.ReceiveNext();
            var lag = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - info.SentServerTimestamp) / 1000f);

            /* ��M���̍��W���A�⊮�̊J�n���W�ɂ��� */
            _p1 = this.transform.position;
            /* ���ݎ����ɂ�����\�����W���A�⊮�̏I�����W�ɂ��� */
            _p2 = networkPosition + networkVelocity * lag;

            //_r1 = this.transform.localEulerAngles;
            //_r2 = networkEulerAngles * lag;

            /* �o�ߎ��Ԃ̃��Z�b�g */
            _elapsedDeltaTime = 0f;
        }
    }
    #endregion
}
