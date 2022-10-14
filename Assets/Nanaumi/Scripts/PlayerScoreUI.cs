using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreUI : MonoBehaviour
{
    #region enum

    #endregion

    #region const

    #endregion

    #region public property

    #endregion

    #region private property
    private Text _scoreText = null;

    private float _deltaTime = 0f;
    #endregion

    #region public method

    #endregion

    #region private method

    #endregion

    #region event
    private void Start()
    {
        _scoreText = this.GetComponent<Text>();
    }

    private void Update()
    {
        if (!PhotonNetwork.InRoom) { return; }

        _deltaTime += Time.deltaTime;
        if (_deltaTime > 0.1f)
        {
            _scoreText.text = PhotonNetwork.LocalPlayer.GetScore().ToString();
            _deltaTime = 0f;
        }
    }
    #endregion
}