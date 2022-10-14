using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

// :ToDo �ڑ����InputField�̓��e�𕔉��ޏo��/��������̃G���[�ŕ����ɓ���Ȃ������ꍇ�A�N���A����
public class TitleLobbySceneManager : DebugParent
{
    #region enum
    /// <summary>
    /// �����̏��
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

    [SerializeField, Header("���O����")] private InputField playerNameInput = null;

    [Header("�������쐬����")]

    [SerializeField] private InputField createRoomNameInput = null;
    [SerializeField] private InputField createRoomPasswordInput = null;
    [SerializeField] private Toggle privateToggles = null;

    [Header("�����ɎQ������")]

    [SerializeField] private InputField joinRoomNameInput = null;
    [SerializeField] private InputField joinRoomPasswordInput = null;

    [Header("��������������")]

    [SerializeField] private InputField searchRoomNameInput = null;
    [SerializeField] private InputField searchRoomPasswordInput = null;


    [SerializeField, Header("�����̌��J/����J")] private bool _isRoomOpen = false;

    [SerializeField, Header("�����̖��O�\��")] private Text roomnameText = null;

    [SerializeField, Header("�p�X���[�h���������Ă��镔�����ǂ���")] private Text passwordLockText = null;

    [SerializeField, Header("�����̐l���\��")] private Text roomMemberCountText = null;


    [Header("")]
    [SerializeField] private GameObject titleIconsObj = null;
    [SerializeField] private GameObject nameInputObj = null;


    [SerializeField, Header("�������쐬/�����ɎQ��/���������� �̑I��������")] private GameObject selectRoomObj = null;
    [SerializeField, Header("�A�o�^�[�I��")] private GameObject avatersObj = null;
    [SerializeField, Header("�������쐬����")] private GameObject createRoomObj = null;
    [SerializeField, Header("�����ɎQ������")] private GameObject joinRoomObj = null;
    [SerializeField, Header("��������������")] private GameObject searchRoomObj = null;


    [SerializeField, Header("��������ޏo����")] private GameObject leftRoomObj = null;
    [SerializeField] private CanvasGroup canvasGroup = null;


    [SerializeField, Header("�Q�[�����J�n�ł���Ƃ��ɉ�����J�ڂ̃{�^��")] private GameObject gameStartButtonObj = null;
    [SerializeField, Header("���ɑJ�ڂ���V�[����")] private string nextSceneName = "Main";
    #endregion

    #region public method
    /// <summary>
    /// Photon�ɐڑ����悤�Ƃ���Ƃ�
    /// </summary>
    public override void OnConnected()
    {
        base.OnConnected();
        SetPlayerName(playerNameInput.text);
    }

    /// <summary>
    /// �}�X�^�[�T�[�o�[�ւ̐ڑ������������Ƃ��ɌĂ΂��R�[���o�b�N
    /// </summary>
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        _playerState = PlayerState.JOINING_LOBBY;
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// ���r�[�ւ̎Q�������������Ƃ��ɌĂ΂��R�[���o�b�N
    /// </summary>
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        OutputDebug("Join Lobby Sccess.");

        InstancePlayer(PlayerState.ROOM_SELECT);
    }

    /// <summary>
    /// �����쐬�����������Ƃ��ɌĂ΂��R�[���o�b�N
    /// </summary>
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        OutputDebug("Create Room Sccess.");
    }

    /// <summary>
    /// �����쐬�����s�����Ƃ��ɌĂ΂��R�[���o�b�N
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
    /// �Q�[���T�[�o�[�ւ̐ڑ������������Ƃ��ɌĂ΂��R�[���o�b�N
    /// </summary>
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        OutputDebug("Join Room Sccess.");

        _playerState = PlayerState.ON_ROOM;

        Destroy(GameObject.FindGameObjectWithTag("Player"));

        /* �����_���ȍ��W�Ɏ��g�̃A�o�^�[(�l�b�g���[�N�I�u�W�F�N�g)�𐶐����� */
        var position = new Vector3(Random.Range(-3f, 3f), 0f); // :ToDo �����_���łȂ����̒l���琶������Ă��Ȃ��ꏊ�ɐ�������悤�ɂ���
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

        // :ToDo �l�����������Ƃ��ɍēx�Q���\�ɂ���
        if (PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }

    /// <summary>
    /// �Q�[���T�[�o�[�ւ̐ڑ������s�����Ƃ��ɌĂ΂��R�[���o�b�N
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
    /// ���v���C���[�����[���֎Q�������Ƃ��ɌĂ΂��R�[���o�b�N
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        OutputDebug($"{newPlayer.NickName}���Q�����܂����B");
    }

    /// <summary>
    /// ���v���C���[�����[������ޏo�����Ƃ��ɌĂ΂��R�[���o�b�N
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        OutputDebug($"{otherPlayer.NickName}���ޏo���܂����B");
    }

    /// <summary>
    /// ���g�����[������ޏo�����Ƃ��ɌĂ΂��R�[���o�b�N
    /// </summary>
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        OutputDebug("���[������ޏo���܂����B");

        _playerState = PlayerState.JOINING_LOBBY;
    }
    #endregion

    #region private method
    /// <summary>
    /// PlayerState�ɉ����������̕ύX
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
    /// Photon�ւ̐ڑ�������
    /// </summary>
    /// <param name="gameVersion">�Q�[���̃o�[�W�������</param>
    private void PhotonConnect(string gameVersion)
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = gameVersion;

            /* PhotonServerSettings�̐ݒ���e���g���ă}�X�^�[�T�[�o�[�֐ڑ����� */
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    /// <summary>
    /// �v���C���[�l�[���̓o�^
    /// </summary>
    /// <param name="name"></param>
    private void SetPlayerName(string name)
    {
        if (PhotonNetwork.IsConnected)
        {
            /* �v���C���[���g�̖��O��"Player"�ɂ��� */
            PhotonNetwork.NickName = name;
        }
    }

    /// <summary>
    /// �v���C���[�����r�[�ɃC���X�^���X����
    /// </summary>
    /// <param name="nextPlayerState">���ɑJ�ڂ���X�e�[�g</param>
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
    /// �p�X���[�h�t���̕������쐬����
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

        /* ���[���I�v�V�����̊�{�ݒ� */
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)MAX_PLAYERS,  // �����̎Q���ő�l��
            IsVisible = privateToggles.isOn, // ���J���邩
            IsOpen = _isRoomOpen             // �����ł��邩
        };

        /* ���[���I�v�V�����ɃJ�X�^���v���p�e�B��ݒ� */
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable
        {
            {"Room", createRoomNameInput.text},     // ������
            {"Password", createRoomPasswordInput.text}, // �p�X���[�h
            {"isLock", isRoomLock}            // �����Ɍ��������邩
        };
        roomOptions.CustomRoomProperties = customRoomProperties;

        /* ���r�[�Ɍ��J����J�X�^���v���p�e�B�̎w�� */
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "Room", "isLock" };

        if (PhotonNetwork.InLobby)
        {
            /* �������̕\�� */
            roomnameText.text = createRoomNameInput.text;

            // :ToDo �������̏d���𖳂���or�d�����Ă���ꍇ�̂݃J�E���g�𑝂₷or�d�������ꍇ�͍쐬�ł��Ȃ��悤�ɂ���

            /* ������(�������͎w��̕�����+�p�X���[�h�Őݒ�)�ɏ]���ă��[���쐬���ĎQ������ */
            PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
        }
    }

    /// <summary>
    /// �����ɎQ������
    /// </summary>
    /// <param name="isLock">�p�X���[�h�t�����ǂ���</param>
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
    /// ��������ޏo����
    /// </summary>
    private void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    /// <summary>
    /// �Q�[�����J�n�ł��邩�̔���
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
    /// ���O���͌��Photon�֐ڑ�����C�x���g
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
    /// �u�L�����N�^�[�̌����ڂ�ύX����v��I�������Ƃ�
    /// </summary>
    public void OnSelectPlayerAvater()
    {
        _playerState = PlayerState.PLAYERAVATER_SELECT;
    }

    /// <summary>
    /// �L�����N�^�[��I�������Ƃ�
    /// </summary>
    /// <param name="avaters">�C���X�^���X����I�u�W�F�N�g�̖��O</param>
    public void OnChangeAvater(GlobalDialogButton.PlayerAvater avaters)
    {
        playerObjName = avaters.ToString();
    }

    /// <summary>
    /// �L�����N�^�[�̌����ڂ����肵���Ƃ�
    /// </summary>
    public void OnDecidePlayerAvater()
    {
        _playerState = PlayerState.PLAYERAVATER_INSTANCE;
    }

    /// <summary>
    /// �u�����쐬������v��I�������Ƃ�
    /// </summary>
    public void OnCreateRoomSelected()
    {
        _playerState = PlayerState.CREATEROOM_INPUT;
    }

    /// <summary>
    /// �u�����ɎQ������v��I�������Ƃ�
    /// </summary>
    public void OnJoinRoomSelected()
    {
        _playerState = PlayerState.JOINROOM_INPUT;
    }

    /// <summary>
    /// �u��������������v��I�������Ƃ�
    /// </summary>
    public void OnSearchRoomSelected()
    {
        _playerState = PlayerState.SEARCHROOM_INPUT;
    }

    /// <summary>
    /// �������쐬�E�Q���E�����̉�ʂ���u�߂�v��I�������Ƃ�
    /// </summary>
    public void OnBackSelectRoomMode()
    {
        _playerState = PlayerState.ROOM_SELECT;
    }

    /// <summary>
    /// �����쐬������Ƃ��̃C�x���g
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
    /// �����Q������Ƃ��̃C�x���g
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
    /// �����ޏo����Ƃ��̃C�x���g
    /// </summary>
    public void OnLeaveRoom()
    {
        _playerState = PlayerState.CONNECTING_MASTERSERVER;

        LeaveRoom();
    }

    /// <summary>
    /// �Q�[�����J�n�����Ƃ��̃C�x���g
    /// </summary>
    public void OnStartGame()
    {
        PhotonNetwork.IsMessageQueueRunning = false;

        SceneManager.LoadSceneAsync(nextSceneName);
    }
    #endregion
}
