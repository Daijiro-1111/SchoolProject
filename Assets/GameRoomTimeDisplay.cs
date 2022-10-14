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
        /* ルームに参加していない場合は更新しない */
        if (!PhotonNetwork.InRoom) { return; }
        /* ゲームの開始時刻が設定されていない場合は更新しない */
        if (!PhotonNetwork.CurrentRoom.TryGetStartTime(out int timestamp)) { return; }

        /* ゲームの経過時間を求めて、小数第一位まで表示する */
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