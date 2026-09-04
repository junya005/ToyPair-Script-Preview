using UnityEngine;
using System;

/// <summary>
/// プレイヤーの死亡イベントを制御するコンポーネント
/// </summary>
public class PlayerDeath : MonoBehaviour, IDeathable
{
    private int _playerIndex;

    /// <summary>死亡処理が実行中かどうか</summary>
    /// <remarks>
    /// OnTriggerEnterの判定が2連続で発生するという事案があったため、
    /// 防ぐためにフラグを用意しています。
    /// </remarks>
    public bool IsExecute { get; private set; } = true;

    /// <summary>プレイヤーが死亡したときに発火されるイベント</summary>
    /// <remarks>int値でどのプレイヤーが死亡したかを判別できます</remarks>
    public event Action<int> OnDeathEvent;

    private void OnDestroy()
    {
        // イベントの登録を解除
        OnDeathEvent = null;
    }

    /// <summary>
    /// 死亡イベントを実行する
    /// </summary>
    public void Execute()
    {
        if (!IsExecute) { return; }

        // 死亡イベントを呼ぶ
        OnDeathEvent?.Invoke(_playerIndex);
        IsExecute = false;
    }

    /// <summary>
    /// プレイヤーのインデックスを設定する
    /// </summary>
    /// <remarks>
    /// PlayerControllerからの呼び出しを想定しています
    /// </remarks>
    /// <param name="playerIndex">設定値</param>
    public void SetPlayerIndex(int playerIndex)
    {
        _playerIndex = playerIndex;
    }

    /// <summary>
    /// 死亡処理を実行中かどうかのフラグを設定する
    /// </summary>
    /// <param name="isExecute">設定値</param>
    public void SetIsExecute(bool isExecute)
    {
        IsExecute = isExecute;
    }
}
