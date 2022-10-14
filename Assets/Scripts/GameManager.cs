using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

// :ToDo �e�E�X�e�[�W�Ȃǂ̓��� �����̗D�揇�ʂ̊m�� �^�C�g����ʂ��d�l�ʂ�ɐ���
//       Player�X�L���̈��p��
public class GameManager : DebugParent
{
    private GameObject[] _cannons;
    private CameraController _cameraController;
    [SerializeField] private Text _startText;

    /* Add ���C */
    #region const
    private const int FINISH_TIME = 120;
    #endregion

    #region private property
    // :ToDo �A�o�^�[�̈��p�������āA���̃V�[���̃C���X�^���X�ł����ꂪ���f�����悤�ɂ���
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
    /* �����܂� */

    void Start()
    {
        /* Add ���C */
        startButtonObj.SetActive(false);
        ui.SetActive(false);
        _isStartReady = false;
        _goalCount = 0;
        _goalActive = false;

        PhotonNetwork.IsMessageQueueRunning = true;

        PhotonNetwork.ConnectUsingSettings();

        /* �����_���ȍ��W�Ɏ��g�̃A�o�^�[(�l�b�g���[�N�I�u�W�F�N�g)�𐶐����� */
        var position = new Vector3(Random.Range(-3f, 3f), 0f);
        GameObject obj = PhotonNetwork.Instantiate(playerObjName, position, Quaternion.identity);

        _playerController = obj.GetComponent<PlayerController>();
        _playerController.enabled = false;
        /* �����܂� */

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

    /* Add ���C */
    private void Update()
    {
        /* ���[���ɎQ�����Ă��Ȃ��ꍇ�͍X�V���Ȃ� */
        if (!PhotonNetwork.InRoom) { return; }

        if (!_isStartReady)
        {
            /* �����̐l�������������v���C���[�����X�g�ɒǉ����A�J�n�ł���悤�ɂ��� */
            if (_playersParentObj.childCount != PhotonNetwork.CurrentRoom.MaxPlayers) { return; }
            _isStartReady = true;

            /* �S�����낦�΃v���C���[���X�g�ɑ�� */
            for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
            {
                _players.Add(_playersParentObj.GetChild(i).GetComponent<PlayerController>());
                _players[i].enabled = false;
            }

            /* �}�X�^�[�N���C�A���g�̂݊J�n�ł��� */
            if (PhotonNetwork.IsMasterClient)
            {
                startButtonObj.SetActive(true);
            }
        }

        /* �Q�[���̊J�n�������ݒ肳��Ă��Ȃ��ꍇ�͍X�V���Ȃ� */
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
    /// �Q�[���J�n�C�x���g
    /// </summary>
    public void OnGameStart()
    {
        startButtonObj.SetActive(false);
        photonView.RPC("GameStart",RpcTarget.All);
    }

    /// <summary>
    /// �Q�[���I��
    /// </summary>
    [PunRPC]
    private void GoalorTimeup()
    {
        _goalActive = true;
    }

    /// <summary>
    /// �Q�[���I����Ƀ��[���ɖ߂�C�x���g
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

    //    /* ���[���I�v�V�����̊�{�ݒ� */
    //    RoomOptions roomOptions = new RoomOptions
    //    {
    //        MaxPlayers = (byte)1, // �����̎Q���ő�l��
    //    };

    //    PhotonNetwork.JoinOrCreateRoom("Room", roomOptions, TypedLobby.Default);
    //}

    //public override void OnJoinedRoom()
    //{
    //    base.OnJoinedRoom();

    //    /* �����_���ȍ��W�Ɏ��g�̃A�o�^�[(�l�b�g���[�N�I�u�W�F�N�g)�𐶐����� */
    //    var position = new Vector3(Random.Range(-3f, 3f), 0f);
    //    GameObject obj = PhotonNetwork.Instantiate(playerObjName, position, Quaternion.identity);

    //    _playerController = obj.GetComponent<PlayerController>();
    //    _cameraController.isPlayerMove = true;
    //}
    /* �����܂� */

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

        // :ToDo �S���̃X�^�[�g�^�C�~���O�����킹��
        yield return new WaitForSeconds(1);
        _startText.text = "��[��";
        yield return new WaitForSeconds(2);
        _startText.text = "�ǂ�!";
        yield return new WaitForSeconds(1);
        _startText.gameObject.SetActive(false);

        //photonView.RPC("Started",RpcTarget.All);


        /* Add ���C */
        PhotonNetwork.CurrentRoom.SetStartTime(PhotonNetwork.ServerTimestamp);
        /* �����܂� */
    }

    [PunRPC]
    private void Started()
    {
        _isStart = true;
    }
}
