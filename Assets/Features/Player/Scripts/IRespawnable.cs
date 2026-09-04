using UnityEngine;

/// <summary>
/// リスポーン可能なコンポーネントに継承させるインターフェース
/// </summary>
public interface IRespawnable
{
    /// <summary>
    /// リスポーン処理を実行
    /// </summary>
    void Execute();

    /// <summary>
    /// リスポーン座標が登録済みか確認する
    /// </summary>
    /// <remarks>
    /// 新しいリスポーン座標を設定する際にこのメソッドを使って条件分岐を
    /// 記述しておくことを推奨します。
    /// 複数の座標（配列）から設定する場所を選択するという方式の場合、
    /// その場所からランダムに登録される可能性を防ぐためです。
    /// </remarks>
    /// <param name="respawnPos">比較対象の座標</param>
    /// <returns>登録済みであるか</returns>
    bool IsRegisteredSpawnPos(Vector3 respawnPos);

    /// <summary>
    /// リスポーン座標を設定（登録）する
    /// </summary>
    /// <param name="respawnPoint">設定するリスポーン座標</param>
    void SetRespawnPoint(Vector3 respawnPoint);
}
