/// <summary>
/// プレイヤーの入力状態を表す列挙子
/// </summary>
/// <remarks>
/// 長押しして離す 等の真偽値での表現が困難な入力状態の時に使用します。
/// Pressed状態を用意していないので、一瞬押された という動きの場合は真偽値を使用してください
/// </remarks>
public enum PlayerInputState
{
    Idle,
    Hold,
    Release
}
