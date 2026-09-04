using UnityEngine;

/// <summary>
/// パンチの土台を操作するためのハンドラ
/// </summary>
public class PunchMoveHandler : MonoBehaviour, IPlayerHandle
{
    [SerializeField, Tooltip("土台移動コンポーネント")]
    private PunchMove _punchMove;

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

    // 関数の概要は継承先の定義を参照してください
    // 使用する操作のみ動作を定義しています

    public void SetMoveInput(Vector2 inputValue)
    {

    }

    public void SetJumpInput(bool inputValue)
    {
        if (inputValue)
        {
            // 移動を実行
            _punchMove?.Move();
        }
    }

    public void SetBrakeInput(bool inputValue)
    {

    }

    public void SetBrakeInputRelesed(bool inputValue)
    {

    }

    public void SetChargeDushInput(bool inputValue)
    {

    }

    public void SetChargeDushInputRelesed(bool inputValue)
    {

    }

    public void SetInteractInput(bool inputValue)
    {
        if (inputValue)
        {
            // 操作を解除する
            InputManager.Instance?.ResetHandle(_controlPlayerIndex);
        }
    }

    #endregion
}
