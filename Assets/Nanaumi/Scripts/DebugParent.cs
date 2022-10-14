using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

// MonoBehaviourPunCallBacks���p�����āAPUN�̃R�[���o�b�N���󂯐܂��悤�ɂ���
public class DebugParent : MonoBehaviourPunCallbacks
{
    #region Debug
    private GameObject debugObj;
    private Transform debugObjParent;
    #endregion

    public virtual void Awake()
    {
        debugObj = (GameObject)Resources.Load("Text");
        debugObjParent = GameObject.FindGameObjectWithTag("Debug").transform;
    }

    public void OutputDebug(string sentence)
    {
        GameObject obj = Instantiate(debugObj, debugObjParent.transform);
        obj.GetComponent<Text>().text = sentence;
    }

    public void ErrorPopup(string errorMessage)
    {
        OutputDebug(errorMessage);
    }
}
