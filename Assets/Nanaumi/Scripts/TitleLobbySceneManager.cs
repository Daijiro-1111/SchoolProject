using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

// :ToDo 接続後にInputFieldの内容を部屋退出時/何かしらのエラーで部屋に入れなかった場合、クリアする
public class TitleLobbySceneManager : DebugParent
{
    #region enum
    /// <summary>
    /// 部屋の状態
    /// </summary>
    public enum RoomState
    {
        WAIT,
        PLAYING
    }

    public enum PlayerState
    {
        TITLE,
        NAME_INPUT,
        CONNECTING_MASTERSERVER,
        JOINING_LOBBY,
        ROOM_SELECT,
        PLAYERAVATER_SELECT,
        PLAYERAVATER_INSTANCE,
        CREATEROOM_INPUT,
        JOINROOM_INPUT,
        SEARCHROOM_INPUT,
        CONNECTING_ROOM,
        ON_ROOM
    }

    public enum PlayerAvaters
    {
        Player,
        Player1
    }
    #endregion

    #region const
    private const int MAX_PLAYERS = 1;
    #endregion

    #region public property

    #endregion

    #region private property
    [SerializeField] private string playerObjName = "Player";
    private PlayerController _playerController = null;
    [SerializeField] private CameraMove cameraMove = null;

    private PlayerState _playerState = PlayerState.TITLE;

    [SerializeField, Header("名前入力")] private InputField playerNameInput = null;

    [Header("部屋を作成する")]

    [SerializeField] private InputField createRoomNameInput = null;
    [SerializeField] private InputField createRoomPasswordInput = null;
    [SerializeField] private Toggle privateToggles = null;

    [Header("部屋に参加する")]

    [SerializeField] private InputField joinRoomNameInput = null;
    [SerializeField] private InputField joinRoomPasswordInput = null;

    [Header("部屋を検索する")]

    [SerializeField] private InputField searchRoomNameInput = null;
    [SerializeField] private InputField searchRoomPasswordInput = null;


    [SerializeField, Header("部屋の公開/非公開")] private bool _isRoomOpen = false;

    [SerializeField, Header("部屋の名前表示")] private Text roomnameText = null;

    [SerializeField, Header("パスワードがかかっている部屋かどうか")] private Text passwordLockText = null;

    [SerializeField, Header("部屋の人数表示")] private Text roomMemberCountText = null;


    [Header("")]
    [SerializeField] private GameObject titleIconsObj = null;
    [SerializeField] private GameObject nameInputObj = null;


    [SerializeField, Header("部屋を作成/部屋に参加/部屋を検索 の選択をする")] private GameObject selectRoomObj = null;
    [SerializeField, Header("アバター選択")] private GameObject avatersObj = null;
    [SerializeField, Header("部屋を作成する")] private GameObject createRoomObj = null;
    [SerializeField, Header("部屋に参加する")] private GameObject joinRoomObj = null;
    [SerializeField, Header("部屋を検索する")] private GameObject searchRoomObj = null;


    [SerializeField, Header("部屋から退出する")] private GameObject leftRoomObj = null;
    [SerializeField] private CanvasGroup canvasGroup = null;


    [SerializeField, Header("ゲームを開始できるときに押せる遷移のボタン")] private GameObject gameStartButtonObj = null;
    [SerializeField, Header("次に遷移するシーン名")] private string nextSceneName = "Main";
    #endregion

    #region public method
    /// <summary>
    /// Photonに接続しようとするとき
    /// </summary>
    public override void OnConnected()
    {
        base.OnConnected();
        SetPlayerName(playerNameInput.text);
    }

    /// <summary>
    /// マスターサーバーへの接続が成功したときに呼ばれるコールバック
    /// </summary>
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        _playerState = PlayerState.JOINING_LOBBY;
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// ロビーへの参加が成功したときに呼ばれるコールバック
    /// </summary>
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        OutputDebug("Join Lobby Sccess.");

        InstancePlayer(PlayerState.ROOM_SELECT);
    }

    /// <summary>
    /// 部屋作成が成功したときに呼ばれるコールバック
    /// </summary>
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        OutputDebug("Create Room Sccess.");
    }

    /// <summary>
    /// 部屋作成が失敗したときに呼ばれるコールバック
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        OutputDebug("Create Room Failed." + message);

        _playerState = PlayerState.ROOM_SELECT;
    }

    /// <summary>
    /// ゲームサーバーへの接続が成功したときに呼ばれるコールバック
    /// </summary>
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        OutputDebug("Join Room Sccess.");

        _playerState = PlayerState.ON_ROOM;

        Destroy(GameObject.FindGameObjectWithTag("Player"));

        /* ランダムな座標に自身のアバター(ネットワークオブジェクト)を生成する */
        var position = new Vector3(Random.Range(-3f, 3f), 0f); // :ToDo ランダムでなく一定の値から生成されていない場所に生成するようにする
        GameObject obj = PhotonNetwork.Instantiate(playerObjName, position, this.transform.rotation);

        _playerController = obj.GetComponent<PlayerController>();
        cameraMove.isPlayerMove = true;

        if (PhotonNetwork.IsMasterClient)
        {
            roomnameText.text = createRoomNameInput.text;
        }
        else
        {
            roomnameText.text = joinRoomNameInput.text;
        }

        // :ToDo 人数が減ったときに再度参加可能にする
        if (PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }

    /// <summary>
    /// ゲームサーバーへの接続が失敗したときに呼ばれるコールバック
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        OutputDebug("Join Room Failed."+ message);

        _playerState = PlayerState.ROOM_SELECT;
    }

    /// <summary>
    /// 他プレイヤーがルームへ参加したときに呼ばれるコールバック
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        OutputDebug($"{newPlayer.NickName}が参加しました。");
    }

    /// <summary>
    /// 他プレイヤーがルームから退出したときに呼ばれるコールバック
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        OutputDebug($"{otherPlayer.NickName}が退出しました。");
    }

    /// <summary>
    /// 自身がルームから退出したときに呼ばれるコールバック
    /// </summary>
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        OutputDebug("ルームから退出しました。");

        _playerState = PlayerState.JOINING_LOBBY;
    }
    #endregion

    #region private method
    /// <summary>
    /// PlayerStateに応じた処理の変更
    /// </summary>
    private void PlayerStateChanged()
    {
        switch (_playerState)
        {
            case PlayerState.TITLE:
                titleIconsObj.SetActive(true);
                canvasGroup.interactable = true;

                if (Input.anyKeyDown)
                {
                    _playerState = PlayerState.NAME_INPUT;
                }
                break;

            case PlayerState.NAME_INPUT:
                titleIconsObj.SetActive(false);
                nameInputObj.SetActive(true);
                break;

            case PlayerState.CONNECTING_MASTERSERVER:
                canvasGroup.interactable = false;
                break;

            case PlayerState.JOINING_LOBBY:
                canvasGroup.interactable = false;

                cameraMove.isPlayerMove = false;
                break;

            case PlayerState.ROOM_SELECT:
                titleIconsObj.SetActive(false);
                nameInputObj.SetActive(false);
                selectRoomObj.SetActive(true);
                createRoomObj.SetActive(false);
                joinRoomObj.SetActive(false);
                searchRoomObj.SetActive(false);
                leftRoomObj.SetActive(false);
                gameStartButtonObj.SetActive(false);
                canvasGroup.interactable = true;
                cameraMove.isPlayerMove = true;

                _playerController.enabled = true;
                break;

            case PlayerState.PLAYERAVATER_SELECT:
                selectRoomObj.SetActive(false);
                avatersObj.SetActive(true);

                _playerController.AllStop();
                _playerController.enabled = false;
                break;

            case PlayerState.PLAYERAVATER_INSTANCE:
                selectRoomObj.SetActive(true);
                avatersObj.SetActive(false);

                InstancePlayer(PlayerState.ROOM_SELECT);
                break;

            case PlayerState.CREATEROOM_INPUT:
                selectRoomObj.SetActive(false);
                createRoomObj.SetActive(true);
                searchRoomObj.SetActive(false);
                joinRoomObj.SetActive(false);

                _playerController.AllStop();
                _playerController.enabled = false;
                break;

            case PlayerState.JOINROOM_INPUT:
                selectRoomObj.SetActive(false);
                createRoomObj.SetActive(false);
                joinRoomObj.SetActive(true);
                searchRoomObj.SetActive(false);

                _playerController.AllStop();
                _playerController.enabled = false;
                break;

            case PlayerState.SEARCHROOM_INPUT:
                selectRoomObj.SetActive(false);
                createRoomObj.SetActive(false);
                joinRoomObj.SetActive(false);
                searchRoomObj.SetActive(true);

                _playerController.AllStop();
                _playerController.enabled = false;
                break;

            case PlayerState.CONNECTING_ROOM:
                canvasGroup.interactable = false;

                _playerController.AllStop();
                _playerController.enabled = false;
                break;

            case PlayerState.ON_ROOM:
                titleIconsObj.SetActive(false);
                nameInputObj.SetActive(false);
                selectRoomObj.SetActive(false);
                createRoomObj.SetActive(false);
                joinRoomObj.SetActive(false);
                searchRoomObj.SetActive(false);
                leftRoomObj.SetActive(true);
                canvasGroup.interactable = true;

                _playerController.enabled = true;

                roomMemberCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

                GameStart();
                break;

            default:
                titleIconsObj.SetActive(false);
                nameInputObj.SetActive(false);
                selectRoomObj.SetActive(false);
                createRoomObj.SetActive(false);
                joinRoomObj.SetActive(false);
                searchRoomObj.SetActive(false);
                leftRoomObj.SetActive(false);
                canvasGroup.interactable = false;
                gameStartButtonObj.SetActive(false);

                _playerController.AllStop();
                _playerController.enabled = false;
                break;
        }
    }

    /// <summary>
    /// Photonへの接続をする
    /// </summary>
    /// <param name="gameVersion">ゲームのバージョン情報</param>
    private void PhotonConnect(string gameVersion)
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = gameVersion;

            /* PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する */
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    /// <summary>
    /// プレイヤーネームの登録
    /// </summary>
    /// <param name="name"></param>
    private void SetPlayerName(string name)
    {
        if (PhotonNetwork.IsConnected)
        {
            /* プレイヤー自身の名前を"Player"にする */
            PhotonNetwork.NickName = name;
        }
    }

    /// <summary>
    /// プレイヤーをロビーにインスタンスする
    /// </summary>
    /// <param name="nextPlayerState">次に遷移するステート</param>
    private void InstancePlayer(PlayerState nextPlayerState)
    {
        _playerState = nextPlayerState;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }

        Vector3 position = new Vector3(Random.Range(-3f, 3f), 0f);
        GameObject gameobj = (GameObject)Resources.Load(playerObjName);
        GameObject obj = Instantiate(gameobj, position, gameobj.transform.rotation);

        _playerController = obj.GetComponent<PlayerController>();
        cameraMove.isPlayerMove = true;
    }

    /// <summary>
    /// パスワード付きの部屋を作成する
    /// </summary>
    private void CreateRoomSetProperty()
    {
        string roomName = createRoomNameInput.text;

        bool isRoomLock = false;
        if (!string.IsNullOrEmpty(createRoomPasswordInput.text))
        {
            isRoomLock = true;
            roomName += "_" + createRoomPasswordInput.text;
        }
        passwordLockText.text = $"{(isRoomLock?"": "un")}LockedRoom";

        /* ルームオプションの基本設定 */
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)MAX_PLAYERS,  // 部屋の参加最大人数
            IsVisible = privateToggles.isOn, // 公開するか
            IsOpen = _isRoomOpen             // 入室できるか
        };

        /* ルームオプションにカスタムプロパティを設定 */
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable
        {
            {"Room", createRoomNameInput.text},     // 部屋名
            {"Password", createRoomPasswordInput.text}, // パスワード
            {"isLock", isRoomLock}            // 部屋に鍵をかけるか
        };
        roomOptions.CustomRoomProperties = customRoomProperties;

        /* ロビーに公開するカスタムプロパティの指定 */
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "Room", "isLock" };

        if (PhotonNetwork.InLobby)
        {
            /* 部屋名の表示 */
            roomnameText.text = createRoomNameInput.text;

            // :ToDo 部屋名の重複を無くすor重複している場合のみカウントを増やすor重複した場合は作成できないようにする

            /* 部屋名(部屋名は指定の部屋名+パスワードで設定)に従ってルーム作成して参加する */
            PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
        }
    }

    /// <summary>
    /// 部屋に参加する
    /// </summary>
    /// <param name="isLock">パスワード付きかどうか</param>
    private void JoinRoom(bool isLock)
    {
        passwordLockText.text = $"{(isLock ? "" : "un")}LockedRoom";

        if (isLock)
        {
            PhotonNetwork.JoinRoom(joinRoomNameInput.text + "_" + joinRoomPasswordInput.text);
        }
        else
        {
            PhotonNetwork.JoinRoom(joinRoomNameInput.text);
        }
    }

    /// <summary>
    /// 部屋から退出する
    /// </summary>
    private void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    /// <summary>
    /// ゲームを開始できるかの判定
    /// </summary>
    private void GameStart()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }

        if (PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            gameStartButtonObj.SetActive(true);
        }
        else
        {
            gameStartButtonObj.SetActive(false);
        }
    }
    #endregion

    #region event
    public override void Awake()
    {
        base.Awake();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        nameInputObj.SetActive(false);
        selectRoomObj.SetActive(false);
        createRoomObj.SetActive(false);
        joinRoomObj.SetActive(false);
        searchRoomObj.SetActive(false);
        leftRoomObj.SetActive(false);
        titleIconsObj.SetActive(true);
        gameStartButtonObj.SetActive(false);

        if (PhotonNetwork.InLobby) { OnJoinedLobby(); }
        if (PhotonNetwork.InRoom) { OnJoinedRoom(); }
    }

    private void Update()
    {
        PlayerStateChanged();
    }

    /// <summary>
    /// 名前入力後にPhotonへ接続するイベント
    /// </summary>
    public void OnSubmit()
    {
        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            OutputDebug("PlayerName is null.");
            return;
        }

        _playerState = PlayerState.CONNECTING_MASTERSERVER;

        PhotonConnect("v1.0");
    }

    /// <summary>
    /// 「キャラクターの見た目を変更する」を選択したとき
    /// </summary>
    public void OnSelectPlayerAvater()
    {
        _playerState = PlayerState.PLAYERAVATER_SELECT;
    }

    /// <summary>
    /// キャラクターを選択したとき
    /// </summary>
    /// <param name="avaters">インスタンスするオブジェクトの名前</param>
    public void OnChangeAvater(GlobalDialogButton.PlayerAvater avaters)
    {
        playerObjName = avaters.ToString();
    }

    /// <summary>
    /// キャラクターの見た目を決定したとき
    /// </summary>
    public void OnDecidePlayerAvater()
    {
        _playerState = PlayerState.PLAYERAVATER_INSTANCE;
    }

    /// <summary>
    /// 「部屋作成をする」を選択したとき
    /// </summary>
    public void OnCreateRoomSelected()
    {
        _playerState = PlayerState.CREATEROOM_INPUT;
    }

    /// <summary>
    /// 「部屋に参加する」を選択したとき
    /// </summary>
    public void OnJoinRoomSelected()
    {
        _playerState = PlayerState.JOINROOM_INPUT;
    }

    /// <summary>
    /// 「部屋を検索する」を選択したとき
    /// </summary>
    public void OnSearchRoomSelected()
    {
        _playerState = PlayerState.SEARCHROOM_INPUT;
    }

    /// <summary>
    /// 部屋を作成・参加・検索の画面から「戻る」を選択したとき
    /// </summary>
    public void OnBackSelectRoomMode()
    {
        _playerState = PlayerState.ROOM_SELECT;
    }

    /// <summary>
    /// 部屋作成をするときのイベント
    /// </summary>
    public void OnCreateRoom()
    {
        if(string.IsNullOrEmpty(createRoomNameInput.text))
        {
            ErrorPopup("Roomname is null.");
            return;
        }

        _playerState = PlayerState.CONNECTING_ROOM;

        CreateRoomSetProperty();
    }

    /// <summary>
    /// 部屋参加するときのイベント
    /// </summary>
    public void OnJoinRoom()
    {
        if (string.IsNullOrEmpty(joinRoomNameInput.text))
        {
            ErrorPopup("Roomname is null.");
            return;
        }

        bool isLock = false;
        if (!string.IsNullOrEmpty(joinRoomPasswordInput.text))
        {
            isLock = true;
        }

        _playerState = PlayerState.CONNECTING_ROOM;

        JoinRoom(isLock);
    }

    /// <summary>
    /// 部屋退出するときのイベント
    /// </summary>
    public void OnLeaveRoom()
    {
        _playerState = PlayerState.CONNECTING_MASTERSERVER;

        LeaveRoom();
    }

    /// <summary>
    /// ゲームを開始したときのイベント
    /// </summary>
    public void OnStartGame()
    {
        PhotonNetwork.IsMessageQueueRunning = false;

        SceneManager.LoadSceneAsync(nextSceneName);
    }
    #endregion
}
