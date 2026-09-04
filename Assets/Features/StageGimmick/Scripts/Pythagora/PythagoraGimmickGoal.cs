using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ピタゴラギミックのゴールを検知するコンポーネント
/// </summary>
public class PythagoraGimmickGoal : MonoBehaviour
{
    [Header("ゴール時に呼ぶ関数を設定してください")]
    [SerializeField]
    private UnityEvent OnGoal;

    private void OnDestroy()
    {
        OnGoal = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerRespawn>(out var component))
        {
            component.Execute();
            OnGoal?.Invoke();
        }
    }
}
