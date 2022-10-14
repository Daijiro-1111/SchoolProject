using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class AvatarNameDisplay : DebugParent
{
    private void Start()
    {
        var nameLabel = this.GetComponent<TextMesh>();
        if (PhotonNetwork.InRoom)
        {
            /* プレイヤー名とプレイヤーIDを表示する */
            nameLabel.text = $"{PhotonNetwork.NickName}({photonView.OwnerActorNr})";

            /* 自身が管理者かどうかを判定する */
            if (photonView.IsMine)
            {
                /* 所有権を取得する */
                Player owner = photonView.Owner;
                /* 所有者のプレイヤー名とIDを出力 */
                OutputDebug($"Owner : {owner.NickName}({photonView.OwnerActorNr})");
            }

            /* ローカルプレイヤーがマスタークライアントかどうかを判定する */
            if (PhotonNetwork.IsMasterClient)
            {
                OutputDebug("自身がマスタークライアントです");
            }
        }
        else
        {
            nameLabel.text = PhotonNetwork.LocalPlayer.NickName;
        }
    }
}
