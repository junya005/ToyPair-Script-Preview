using UnityEngine;

/// <summary>
/// 衝突判定が起こった時に死亡イベントを誘発するコンポーネント
/// </summary>
public class DeathTriggerByCollider : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<IDeathable>(out var component))
        {
            // 死亡処理を実行
            component.Execute();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // 死亡可能なコンポーネントを搭載しているか確認
        if (other.TryGetComponent<IDeathable>(out var component))
        {
            // 死亡処理を実行
            component.Execute();
        }
    }
}
