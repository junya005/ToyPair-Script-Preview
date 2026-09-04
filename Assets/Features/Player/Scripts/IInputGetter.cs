using UnityEngine;

/// <summary>
/// 入力を受け取るメソッドを抽象化したインターフェース
/// </summary>
public interface IInputGetter
{
    /// <summary>
    /// 移動入力の数値を取得する
    /// </summary>
    Vector2 GetMoveInput();
}
