using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OutBullet : DebugParent
{
    #region enum

    #endregion

    #region const
    private const float SHOT_DISTANCE = 1f;
    #endregion

    #region public property

    #endregion

    #region private property
    private GameObject _bulletObj = null;

    private Vector3 _origin = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;

    private float _shotDeltaTime = 0;
    #endregion

    #region public method

    #endregion

    #region private method

    #endregion

    #region event
    private void Start()
    {
        _bulletObj = (GameObject)Resources.Load("Bullet");
    }

    private void Update()
    {
        /* ルームに参加していない場合は更新しない */
        if (!PhotonNetwork.InRoom) { return; }
        /* ゲームの開始時刻が設定されていない場合は更新しない */
        if (!PhotonNetwork.CurrentRoom.TryGetStartTime(out int timestamp)) { return; }

        _shotDeltaTime += Time.deltaTime;
        if (_shotDeltaTime > SHOT_DISTANCE)
        {
            photonView.RPC("FireBullet", RpcTarget.All);
            _shotDeltaTime = 0;
        }
    }

    [PunRPC]
    private void FireBullet()
    {
        Instantiate(_bulletObj, this.transform.position-Vector3.right*2, this.transform.rotation, this.transform);
    }
    #endregion
}