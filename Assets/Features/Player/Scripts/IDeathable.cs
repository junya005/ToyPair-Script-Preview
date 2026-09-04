/// <summary>
/// 死亡が可能なコンポーネントに継承させるインターフェース
/// </summary>
public interface IDeathable
{
    /// <summary>
    /// 死亡処理が実行中かどうか
    /// </summary>
    bool IsExecute { get; }
    /// <summary>
    /// 死亡処理を実行する
    /// </summary>
    void Execute();
}
