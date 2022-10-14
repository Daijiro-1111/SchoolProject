using ExitGames.Client.Photon;
using Photon.Realtime;

public static class GameRoomProperty
{
    #region enum

    #endregion

    #region const
    private const string KEY_STATE_TIME = "StartTime";

    private static readonly Hashtable propsToSet = new Hashtable();
    #endregion

    #region public property

    #endregion

    #region private property

    #endregion

    #region public method

    #endregion

    #region private method

    #endregion

    #region event
    /// <summary>
    /// �Q�[���̊J�n�������ݒ肳��Ă���Ύ擾����
    /// </summary>
    /// <param name="room"></param>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static bool TryGetStartTime(this Room room, out int timestamp)
    {
        /* �ݒ肳��Ă���ꍇ */
        if(room.CustomProperties[KEY_STATE_TIME] is int value)
        {
            timestamp = value;
            return true;
        }
        /* �ݒ肳��Ă��Ȃ��ꍇ */
        else
        {
            timestamp = 0;
            return false;
        }
    }

    /// <summary>
    /// �Q�[���̊J�n������ݒ肷��
    /// </summary>
    /// <param name="room"></param>
    /// <param name="timestamp"></param>
    public static void SetStartTime(this Room room, int timestamp)
    {
        /* �J�n������HashTable�ɐ��� */
        propsToSet[KEY_STATE_TIME] = timestamp;
        /* ���[���̃J�X�^���v���p�e�B�ɐݒ� */
        room.SetCustomProperties(propsToSet);
        /* HashTable�̃N���A */
        propsToSet.Clear();
    }
    #endregion
}