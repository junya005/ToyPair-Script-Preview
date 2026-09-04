using UnityEngine;

/// <summary>
/// ギミックにプレイヤーをワープさせるコンポーネント
/// </summary>
public class GimmickWarpPlayer : MonoBehaviour
{
    [SerializeField]
    private Transform _warpTargetTransform;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player1"))
        {
            collision.gameObject.transform.position = _warpTargetTransform.position;
        }

        if (collision.gameObject.CompareTag("Player2"))
        {
            collision.gameObject.transform.position = _warpTargetTransform.position;
        }
    }
}
