using UnityEngine;

/// <summary>
/// 速さ制限の構造体
/// </summary>
public struct SpeedLimit
{
    /// <summary>速さ制限の基礎値</summary>
    public float BaseValue { get; private set; }

    // 1.0→0.3倍というような倍率の減速の仕様があるので、
    // それに対応するためValueを基礎値とし、現在の値を管理する変数を別に作成しています。
    /// <summary>現在の速さ制限</summary>
    public float CurrentValue { get; private set; }

    /// <summary>
    /// 速さ制限を初期化します
    /// </summary>
    /// <param name="speedLimit">速さ制限の初期値</param>
    public SpeedLimit(float speedLimit)
    {
        if (speedLimit < 0)
        {
            Debug.LogError($"速度上限は0より小さい値にできません。渡された値: {speedLimit}");
            speedLimit = 0;
        }

        BaseValue = speedLimit;
        CurrentValue = speedLimit;
    }

    /// <summary>
    /// 速さ制限の基礎値を設定する
    /// 現在の速さ制限も設定値に変更されます
    /// </summary>
    /// <param name="speedLimit">設定値</param>
    public void SetBaseValue(float speedLimit)
    {
        BaseValue = speedLimit;
        CurrentValue = speedLimit;
    }

    /// <summary>
    /// 現在の速さ制限を基礎値にリセットする
    /// </summary>
    public void ResetCurrentValueToBaseValue()
    {
        CurrentValue = BaseValue;
    }

    /// <summary>
    /// 現在の速さ制限を設定する
    /// 基礎値を変えたくない場合(一時的なスピードアップ等)に使用してください
    /// </summary>
    /// <param name="currentValue">設定値</param>
    public void SetCurrentValue(float currentValue)
    {
        CurrentValue = currentValue;
    }

    /// <summary>
    /// 現在の速さ制限を倍率を元に設定する
    /// 基礎値を変えたくない場合(一時的なスピードアップ等)に使用してください
    /// </summary>
    /// <param name="multiply">倍率値</param>
    public void SetCurrentValueByMultiply(float multiply)
    {
        CurrentValue = BaseValue * multiply;
    }
}
