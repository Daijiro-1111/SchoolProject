using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class AvatarController : DebugParent,IPunObservable
{
    #region enum

    #endregion

    #region const
    private const float COMPLETION_DISTANCE_TIME = 0.2f; // 補完にかける時間間隔
    #endregion

    #region public property

    #endregion

    #region private property
    //[SerializeField] private GameObject prefab;
    //private int score = 0;

    /* 座標同期用 */
    private Vector3 _p1 = Vector3.zero;
    private Vector3 _p2 = Vector3.zero;
    /* 回転同期用 */
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
                // 推測航法
                /* 自身のネットワークオブジェクトは、毎フレームの移動量と経過時間を記録する */
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
            // 線形補完
            /* 自身の情報を送信する */
            stream.SendNext(this.transform.position);
            //stream.SendNext(this.transform.localEulerAngles);

            // 推測航法
            /* 毎フレームの移動量と経過時間から、秒速を求めて送信する */
            stream.SendNext((_p2 - _p1) / _elapsedDeltaTime);
            //stream.SendNext((_r2 - _r1) / _elapsedDeltaTime);
        }
        else
        {
            /* 他プレイヤーの情報を受信する */
            //this.transform.position = (Vector3)stream.ReceiveNext();

            //// 線形補完
            ///* 受信時の座標を、補完の開始座標にする */
            //_p1 = this.transform.position;
            ///* 受信した座標を、補完の終了座標にする */
            //_p2 = (Vector3)stream.ReceiveNext();
            ///* 経過時間のリセット */
            //_elapsedDeltaTime = 0f;

            var networkPosition = (Vector3)stream.ReceiveNext();
            var networkVelocity = (Vector3)stream.ReceiveNext();
            if (float.IsNaN(networkVelocity.x)) { networkVelocity = Vector3.one; } // 例外処理
            //var networkEulerAngles = (Vector3)stream.ReceiveNext();
            //var networkAngulerVelocity = (Vector3)stream.ReceiveNext();
            var lag = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - info.SentServerTimestamp) / 1000f);

            /* 受信時の座標を、補完の開始座標にする */
            _p1 = this.transform.position;
            /* 現在時刻における予測座標を、補完の終了座標にする */
            _p2 = networkPosition + networkVelocity * lag;

            //_r1 = this.transform.localEulerAngles;
            //_r2 = networkEulerAngles * lag;

            /* 経過時間のリセット */
            _elapsedDeltaTime = 0f;
        }
    }
    #endregion
}
