using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameRoomTimeDisplay : MonoBehaviour
{
    #region enum

    #endregion

    #region const
    private const int FINISH_TIME = 120;
    #endregion

    #region public property

    #endregion

    #region private property
    private Text timelabel = null;
    #endregion

    #region public method

    #endregion

    #region private method

    #endregion

    #region event
    private void Start()
    {
        timelabel = this.GetComponent<Text>();
        timelabel.text = FINISH_TIME.ToString("f1");
    }

    private void Update()
    {
        /* ���[���ɎQ�����Ă��Ȃ��ꍇ�͍X�V���Ȃ� */
        if (!PhotonNetwork.InRoom) { return; }
        /* �Q�[���̊J�n�������ݒ肳��Ă��Ȃ��ꍇ�͍X�V���Ȃ� */
        if (!PhotonNetwork.CurrentRoom.TryGetStartTime(out int timestamp)) { return; }

        /* �Q�[���̌o�ߎ��Ԃ����߂āA�������ʂ܂ŕ\������ */
        float elapsedTime = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);
        if (elapsedTime > FINISH_TIME)
        {
            timelabel.text = 0.ToString("f1");
            return;
        }
        timelabel.text = (FINISH_TIME - elapsedTime).ToString("f1");
    }
    #endregion
}