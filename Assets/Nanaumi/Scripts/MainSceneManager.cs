using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MainSceneManager : DebugParent
{
    #region enum

    #endregion

    #region const
    private int FINISH_TIME = 10;
    #endregion

    #region public property

    #endregion

    #region private property
    [SerializeField] private string playerObjName = "Player";
    private PlayerController _playerController = null;
    [SerializeField] private CameraMove cameraMove = null;

    [SerializeField] private GameObject startButtonObj = null;

    private bool _goalActive = false;
    [SerializeField] private GameObject ui = null;

    [SerializeField] private string backSceneName = "TitleLobby";
    #endregion

    #region public method

    #endregion

    #region private method

    #endregion

    #region event
    private void Start()
    {
        startButtonObj.SetActive(false);
        ui.SetActive(false);

        PhotonNetwork.IsMessageQueueRunning = true;

        /* ランダムな座標に自身のアバター(ネットワークオブジェクト)を生成する */
        var position = new Vector3(Random.Range(-3f, 3f), 0f);
        GameObject obj = PhotonNetwork.Instantiate(playerObjName, position, Quaternion.identity);

        _playerController = obj.GetComponent<PlayerController>();
        cameraMove.isPlayerMove = true;

        /* マスタークライアントのみゲーム開始ができるようにする */
        if (PhotonNetwork.IsMasterClient)
        {
            startButtonObj.SetActive(true);
        }
    }

    private void Update()
    {
        /* ルームに参加していない場合は更新しない */
        if (!PhotonNetwork.InRoom) { return; }
        /* ゲームの開始時刻が設定されていない場合は更新しない */
        if (!PhotonNetwork.CurrentRoom.TryGetStartTime(out int timestamp)) { return; }

        float elapsedTime = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);

        if (elapsedTime >= FINISH_TIME)
        {
            photonView.RPC("GoalorTimeup", RpcTarget.All);
        }

        if (!_goalActive) { return; }
        ui.SetActive(true);
    }

    [PunRPC]
    private void GoalorTimeup()
    {
        _playerController.AllStop();
        _playerController.enabled = false;

        _goalActive = true;
    }

    public void OnGameStart()
    {
        PhotonNetwork.CurrentRoom.SetStartTime(PhotonNetwork.ServerTimestamp);
        startButtonObj.SetActive(false);
    }

    public void OnBackRoom()
    {
        PhotonNetwork.IsMessageQueueRunning = false;

        SceneManager.LoadSceneAsync(backSceneName);
    }
    #endregion

    #region Debug
    //private void Start()
    //{
    //    startButtonObj.SetActive(false);
    //    ui.SetActive(false);

    //    PhotonNetwork.NickName = "Player";

    //    PhotonNetwork.ConnectUsingSettings();
    //}

    //private void Update()
    //{
    //    /* ルームに参加していない場合は更新しない */
    //    if (!PhotonNetwork.InRoom) { return; }
    //    /* ゲームの開始時刻が設定されていない場合は更新しない */
    //    if (!PhotonNetwork.CurrentRoom.TryGetStartTime(out int timestamp)) { return; }

    //    float elapsedTime = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);

    //    if (elapsedTime >= FINISH_TIME)
    //    {
    //        photonView.RPC("GoalorTimeup", RpcTarget.All);
    //    }

    //    if (!_goalActive) { return; }
    //    ui.SetActive(true);
    //}

    //public override void OnConnectedToMaster()
    //{
    //    base.OnConnectedToMaster();

    //    PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    //}

    //public override void OnJoinedRoom()
    //{
    //    base.OnJoinedRoom();

    //    /* ランダムな座標に自身のアバター(ネットワークオブジェクト)を生成する */
    //    var position = new Vector3(Random.Range(-3f, 3f), 0f);
    //    GameObject obj = PhotonNetwork.Instantiate(playerObjName, position, Quaternion.identity);

    //    _playerController = obj.GetComponent<PlayerController>();
    //    cameraMove.isPlayerMove = true;

    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        startButtonObj.SetActive(true);
    //    }
    //}
    #endregion
}