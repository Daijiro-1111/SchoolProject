using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalDialogButton : MonoBehaviour
{
    #region enum
    public enum PlayerAvater
    {
        Player,
        Player1
    }
    #endregion

    #region const

    #endregion

    #region public property

    #endregion

    #region private property
    [SerializeField] private TitleLobbySceneManager title = null;
    #endregion

    #region public method
    public PlayerAvater avaters;
    #endregion

    #region private method

    #endregion

    #region event
    public void OnClickedButton()
    {
        title.OnChangeAvater(avaters);
    }
    #endregion
}