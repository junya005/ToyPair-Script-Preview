using UnityEngine;

/// <summary>
/// パンチによる接触を検知するクラス
/// </summary>
public class PushTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.TryGetComponent<IPushAble>(out var component))
        {
            component.Push();
        }
    }
}
