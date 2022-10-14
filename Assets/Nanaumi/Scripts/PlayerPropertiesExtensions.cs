using ExitGames.Client.Photon;
using Photon.Realtime;

public static class PlayerPropertiesExtensions
{
    #region enum

    #endregion

    #region const
    private const string SCORE_KEY = "Score";
    private const string MESSAGE_KEY = "Message";

    private static readonly Hashtable propsToSet = new Hashtable();
    #endregion

    #region public property

    #endregion

    #region private property

    #endregion

    #region public method
    /// <summary>
    /// プレイヤーのスコアを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns>Player Score</returns>
    public static int GetScore(this Player player)
    {
        return (player.CustomProperties[SCORE_KEY] is int score) ? score : 0;
    }

    /// <summary>
    /// プレイヤーのスコアを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="score"></param>
    public static void SetScore(this Player player, int score)
    {
        propsToSet[SCORE_KEY] = score;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// プレイヤーのメッセージを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static string GetMessage(this Player player)
    {
        return (player.CustomProperties[MESSAGE_KEY] is string message) ? message : string.Empty;
    }

    public static void SetMessage(this Player player, string message)
    {
        propsToSet[MESSAGE_KEY] = message;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
    #endregion

    #region private method

    #endregion

    #region event

    #endregion
}