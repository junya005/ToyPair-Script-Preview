using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ギミックにプレイヤーをワープさせるコンポーネント
/// </summary>
public class GimmickWarpPlayer : MonoBehaviour
{
    [SerializeField]
    private Transform _warpTargetTransform;

    [SerializeField] private AddTargetPythagora targetPythagora;

    private bool _firstWarp = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player1"))
        {
            collision.gameObject.transform.position = _warpTargetTransform.position;
            targetPythagora.InWarp(true,_firstWarp);
            if (_firstWarp) _firstWarp = false;
        }

        if (collision.gameObject.CompareTag("Player2"))
        {
            collision.gameObject.transform.position = _warpTargetTransform.position;
            targetPythagora.InWarp(false, _firstWarp);
            if(_firstWarp) _firstWarp = false;
        }
    }
}
