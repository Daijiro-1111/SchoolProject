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
    /// ゲームの開始時刻が設定されていれば取得する
    /// </summary>
    /// <param name="room"></param>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static bool TryGetStartTime(this Room room, out int timestamp)
    {
        /* 設定されている場合 */
        if(room.CustomProperties[KEY_STATE_TIME] is int value)
        {
            timestamp = value;
            return true;
        }
        /* 設定されていない場合 */
        else
        {
            timestamp = 0;
            return false;
        }
    }

    /// <summary>
    /// ゲームの開始時刻を設定する
    /// </summary>
    /// <param name="room"></param>
    /// <param name="timestamp"></param>
    public static void SetStartTime(this Room room, int timestamp)
    {
        /* 開始時刻をHashTableに生成 */
        propsToSet[KEY_STATE_TIME] = timestamp;
        /* ルームのカスタムプロパティに設定 */
        room.SetCustomProperties(propsToSet);
        /* HashTableのクリア */
        propsToSet.Clear();
    }
    #endregion
}