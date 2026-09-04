using UnityEngine;

/// <summary>
/// プレイヤー操作を抽象化するインターフェース
/// </summary>
public interface IPlayerHandle
{
    /// <summary>
    /// 移動に使う入力を受け取る
    /// </summary>
    /// <param name="inputValue">入力値</param>
    void SetMoveInput(Vector2 inputValue);

    /// <summary>
    /// ジャンプに使う入力を受け取る
    /// </summary>
    /// <param name="inputValue">入力値</param>
    void SetJumpInput(bool inputValue);

    /// <summary>
    /// ブレーキに使う入力を受け取る
    /// </summary>
    /// <param name="inputValue">入力値</param>
    void SetBrakeInput(bool inputValue);

    /// <summary>
    /// ブレーキを離した時に使う入力を受け取る
    /// </summary>
    /// <param name="inputValue">入力値</param>
    void SetBrakeInputRelesed(bool inputValue);

    /// <summary>
    /// 溜めダッシュに使う入力を受け取る
    /// </summary>
    /// <param name="inputValue">入力値</param>
    void SetChargeDushInput(bool inputValue);

    /// <summary>
    /// 溜めダッシュを離した時に使う入力を受け取る
    /// </summary>
    void SetChargeDushInputRelesed(bool inputValue);

    /// <summary>
    /// インタラクトに使う入力を受け取る
    /// </summary>
    void SetInteractInput(bool inputValue);
}
