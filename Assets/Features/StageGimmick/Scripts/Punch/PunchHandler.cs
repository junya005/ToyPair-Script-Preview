using UnityEngine;

/// <summary>
/// パンチを操作するためのハンドラ
/// </summary>
public class PunchHandler : MonoBehaviour, IPlayerHandle
{
    [SerializeField, Tooltip("パンチの操作ハンドラを設定")]
    private PunchAction _punchAction;

    [SerializeField] private PlayerSpin _player1Spin;
    [SerializeField] private PlayerSpin _player2Spin;

    // インタラクト解除のためにプレイヤーのIndex情報を保存
    private int _controlPlayerIndex;

    /// <summary>
    /// 操作中プレイヤーのインデックスを保存します
    /// </summary>
    /// <param name="playerIndex">プレイヤーのインデックス</param>
    public void SetControlPlayerIndex(int playerIndex)
    {
        _controlPlayerIndex = playerIndex;
    }

    #region Interface Implements

    /// <summary>
    /// 移動に使う入力を受け取る
    /// </summary>
    /// <param name="inputValue">入力値</param>
    public void SetMoveInput(Vector2 inputValue)
    {

    }

    /// <summary>
    /// ジャンプに使う入力を受け取る
    /// </summary>
    /// <param name="inputValue">入力値</param>
    public void SetJumpInput(bool inputValue)
    {
        if (inputValue)
        {
            // パンチを実行
            _punchAction?.Punch();

            if (_controlPlayerIndex == 0)
            {
                _player1Spin.Execute();
            }
            else
            {
                _player2Spin?.Execute();
            }
        }
    }

    /// <summary>
    /// ブレーキに使う入力を受け取る
    /// </summary>
    /// <param name="inputValue">入力値</param>
    public void SetBrakeInput(bool inputValue)
    {

    }

    /// <summary>
    /// ブレーキを離した時に使う入力を受け取る
    /// </summary>
    /// <param name="inputValue">入力値</param>
    public void SetBrakeInputRelesed(bool inputValue)
    {

    }

    /// <summary>
    /// 溜めダッシュに使う入力を受け取る
    /// </summary>
    /// <param name="inputValue">入力値</param>
    public void SetChargeDushInput(bool inputValue)
    {

    }

    /// <summary>
    /// 溜めダッシュを離した時に使う入力を受け取る
    /// </summary>
    public void SetChargeDushInputRelesed(bool inputValue)
    {

    }

    /// <summary>
    /// インタラクトに使う入力を受け取る
    /// </summary>
    public void SetInteractInput(bool inputValue)
    {
        if (inputValue)
        {
            // 操作を解除
            InputManager.Instance?.ResetHandle(_controlPlayerIndex);
        }
    }

    #endregion
}
