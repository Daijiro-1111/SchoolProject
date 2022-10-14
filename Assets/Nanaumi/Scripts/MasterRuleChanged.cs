using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

// :ToDo ���L���̏��n�ƕ����̍폜
public class MasterRuleChanged : DebugParent
{
    #region enum

    #endregion

    #region const
    #endregion

    #region public property

    #endregion

    #region private property
    private bool _isAttacked = false;

    private string id = null;

    private List<int> _scores = new List<int>();
    #endregion

    #region public method
    #endregion

    #region private method

    #endregion

    #region event
    private void Start()
    {
        _scores.Clear();
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }
        if (!PhotonNetwork.InRoom) { return; }


    }

    public void TryAttacked()
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(
            new Hashtable() { { id, PhotonNetwork.LocalPlayer.ActorNumber } },
            new Hashtable() { { _isAttacked, true } });
    }

    /// <summary>
    /// ���[�����X�g���X�V���ꂽ�Ƃ��ɌĂ΂��R�[���o�b�N
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        foreach (RoomInfo info in roomList)
        {
            if (!info.RemovedFromList)
            {
                OutputDebug($"���[���X�V:{info.Name}({info.PlayerCount}/{info.MaxPlayers})");
                if (info.PlayerCount >= info.MaxPlayers)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                }
                else
                {
                    PhotonNetwork.CurrentRoom.IsOpen = true;
                }
            }
            else
            {
                OutputDebug($"���[���폜:{info.Name}");
            }
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        foreach(var entry in propertiesThatChanged)
        {
            string k = (string)entry.Key;
            bool v = (bool)entry.Value;

            if (k == id && v)
            {
                // �U���ɐ���
                Punch((int)entry.Key);
            }
        }
    }

    private void Punch(int key)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"{player.NickName}({player.ActorNumber})");
            if (key == player.ActorNumber)
            {
                // ����ID�̃v���C���[���U������ɂ���or�X�^����Ԃɂ���
            }
        }
    }
    #endregion
}