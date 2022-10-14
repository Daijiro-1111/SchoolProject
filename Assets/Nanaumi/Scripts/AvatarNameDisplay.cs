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
            /* �v���C���[���ƃv���C���[ID��\������ */
            nameLabel.text = $"{PhotonNetwork.NickName}({photonView.OwnerActorNr})";

            /* ���g���Ǘ��҂��ǂ����𔻒肷�� */
            if (photonView.IsMine)
            {
                /* ���L�����擾���� */
                Player owner = photonView.Owner;
                /* ���L�҂̃v���C���[����ID���o�� */
                OutputDebug($"Owner : {owner.NickName}({photonView.OwnerActorNr})");
            }

            /* ���[�J���v���C���[���}�X�^�[�N���C�A���g���ǂ����𔻒肷�� */
            if (PhotonNetwork.IsMasterClient)
            {
                OutputDebug("���g���}�X�^�[�N���C�A���g�ł�");
            }
        }
        else
        {
            nameLabel.text = PhotonNetwork.LocalPlayer.NickName;
        }
    }
}
