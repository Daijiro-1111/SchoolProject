using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

// :ToDo 弾・ステージなどの同期 同期の優先順位の確立 タイトル画面を仕様通りに制作
//       Playerスキンの引継ぎ
public class GameManager : DebugParent
{
    private GameObject[] _cannons;
    private CameraController _cameraController;
    [SerializeField] private Text _startText;

    /* Add 七海 */
    #region const
    private const int FINISH_TIME = 120;
    #endregion

    #region private property
    // :ToDo アバターの引継ぎをして、このシーンのインスタンスでもそれが反映されるようにする
    [SerializeField] private string playerObjName = "Player";
    List<PlayerController> _players = new List<PlayerController>();
    [SerializeField] private Transform _playersParentObj = null;
    private PlayerController _playerController = null;

    [SerializeField] private GameObject startButtonObj = null;

    private int _goalCount = 0;
    private bool _goalActive = false;
    [SerializeField] private GameObject ui = null;

    [SerializeField] private string backSceneName = "TitleLobby";

    private bool _isStartReady = false;
    private bool _isStart = false;
    #endregion
    /* ここまで */

    void Start()
    {
        /* Add 七海 */
        startButtonObj.SetActive(false);
        ui.SetActive(false);
        _isStartReady = false;
        _goalCount = 0;
        _goalActive = false;

        PhotonNetwork.IsMessageQueueRunning = true;

        PhotonNetwork.ConnectUsingSettings();

        /* ランダムな座標に自身のアバター(ネットワークオブジェクト)を生成する */
        var position = new Vector3(Random.Range(-3f, 3f), 0f);
        GameObject obj = PhotonNetwork.Instantiate(playerObjName, position, Quaternion.identity);

        _playerController = obj.GetComponent<PlayerController>();
        _playerController.enabled = false;
        /* ここまで */

        List<PlayerController> _players = new List<PlayerController>();

        //_players = GameObject.FindGameObjectsWithTag(TagData.PLAYER_TAG);
        _cannons = GameObject.FindGameObjectsWithTag(TagData.CANNON_TAG);
        for (int i = 0; i < _cannons.Length; i++)
        {
            _cannons[i].transform.GetChild(0).GetComponent<CandyCannon>().enabled = false;
        }
        _cameraController = Camera.main.GetComponent<CameraController>();
        _cameraController.isPlayerMove = true;
    }

    /* Add 七海 */
    private void Update()
    {
        /* ルームに参加していない場合は更新しない */
        if (!PhotonNetwork.InRoom) { return; }

        if (!_isStartReady)
        {
            /* 部屋の人数がそろったらプレイヤーをリストに追加し、開始できるようにする */
            if (_playersParentObj.childCount != PhotonNetwork.CurrentRoom.MaxPlayers) { return; }
            _isStartReady = true;

            /* 全員そろえばプレイヤーリストに代入 */
            for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
            {
                _players.Add(_playersParentObj.GetChild(i).GetComponent<PlayerController>());
                _players[i].enabled = false;
            }

            /* マスタークライアントのみ開始できる */
            if (PhotonNetwork.IsMasterClient)
            {
                startButtonObj.SetActive(true);
            }
        }

        /* ゲームの開始時刻が設定されていない場合は更新しない */
        if (!PhotonNetwork.CurrentRoom.TryGetStartTime(out int timestamp)) { return; }
        if(timestamp == 0) { return; }

        if (!_isStart)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                _players[i].enabled = true;
            }
            for (int i = 0; i < _cannons.Length; i++)
            {
                _cannons[i].transform.GetChild(0).GetComponent<CandyCannon>().enabled = true;
            }

            _cameraController.MouseCameraMoveStart();
            _isStart = true;
        }

        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].isGoal)
            {
                _goalCount++;
            }
        }

        float elapsedTime = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);

        if (_goalCount >= PhotonNetwork.CurrentRoom.MaxPlayers || elapsedTime >= FINISH_TIME)
        {
            photonView.RPC("GoalorTimeup", RpcTarget.All);

            _playerController.AllStop();
            _playerController.enabled = false;
        }
        else
        {
            _goalCount = 0;
        }

        if (!_goalActive) { return; }
        ui.SetActive(true);
    }

    /// <summary>
    /// ゲーム開始イベント
    /// </summary>
    public void OnGameStart()
    {
        startButtonObj.SetActive(false);
        photonView.RPC("GameStart",RpcTarget.All);
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    [PunRPC]
    private void GoalorTimeup()
    {
        _goalActive = true;
    }

    /// <summary>
    /// ゲーム終了後にルームに戻るイベント
    /// </summary>
    public void OnBackRoom()
    {
        PhotonNetwork.CurrentRoom.SetStartTime(0);

        PhotonNetwork.IsMessageQueueRunning = false;

        SceneManager.LoadSceneAsync(backSceneName);
    }

    //public override void OnConnectedToMaster()
    //{
    //    base.OnConnectedToMaster();

    //    /* ルームオプションの基本設定 */
    //    RoomOptions roomOptions = new RoomOptions
    //    {
    //        MaxPlayers = (byte)1, // 部屋の参加最大人数
    //    };

    //    PhotonNetwork.JoinOrCreateRoom("Room", roomOptions, TypedLobby.Default);
    //}

    //public override void OnJoinedRoom()
    //{
    //    base.OnJoinedRoom();

    //    /* ランダムな座標に自身のアバター(ネットワークオブジェクト)を生成する */
    //    var position = new Vector3(Random.Range(-3f, 3f), 0f);
    //    GameObject obj = PhotonNetwork.Instantiate(playerObjName, position, Quaternion.identity);

    //    _playerController = obj.GetComponent<PlayerController>();
    //    _cameraController.isPlayerMove = true;
    //}
    /* ここまで */

    [PunRPC]
    private IEnumerator GameStart()
    {
        //for (int i = 0; i < _players.Count; i++)
        //{
        //    _players[i].GetComponent<PlayerController>().enabled = false;
        //}
        //for (int i = 0; i < _cannons.Length; i++)
        //{
        //    _cannons[i].transform.GetChild(0).GetComponent<Cannon>().enabled = false;
        //}

        // :ToDo 全員のスタートタイミングを合わせる
        yield return new WaitForSeconds(1);
        _startText.text = "よーい";
        yield return new WaitForSeconds(2);
        _startText.text = "どん!";
        yield return new WaitForSeconds(1);
        _startText.gameObject.SetActive(false);

        //photonView.RPC("Started",RpcTarget.All);


        /* Add 七海 */
        PhotonNetwork.CurrentRoom.SetStartTime(PhotonNetwork.ServerTimestamp);
        /* ここまで */
    }

    [PunRPC]
    private void Started()
    {
        _isStart = true;
    }
}
