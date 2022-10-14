using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RPCSample : DebugParent
{
    #region enum

    #endregion

    #region const

    #endregion

    #region public property

    #endregion

    #region private property

    #endregion

    #region public method

    #endregion

    #region private method
    [PunRPC]
    private void RPCSendMessage(string message, PhotonMessageInfo info)
    {
        OutputDebug($"{info.Sender.NickName} : {message}");
    }
    #endregion

    #region event
    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            photonView.RPC(nameof(RPCSendMessage), RpcTarget.All, "konkonkonkon");
        }
    }
    #endregion
}