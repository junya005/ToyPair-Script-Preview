using UnityEngine;
using System.Collections.Generic;

public class PythagoraGimmickLimitControl : MonoBehaviour
{
    [SerializeField]
    private float _force = 1.0f;

    [SerializeField]
    private float _gravity = -0.2f;

    private List<PlayerController> _rbs = new List<PlayerController>();

    private Vector3 _slopeDownDir;

    private void Update()
    {
        // 変動する床にも対応するため、毎フレーム計算を行う
        _slopeDownDir = CaluculateSlopeDownDirection(this.transform.up);

        foreach (var rb in _rbs)
        {
            Vector3 dir = new Vector3(_slopeDownDir.x, _gravity, _slopeDownDir.z);
            rb.AddForce(dir * _force, ForceMode.VelocityChange);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 各種必要なコンポーネントをリストに格納
        if (other.gameObject.TryGetComponent<PlayerController>(out var rb))
        {
            _rbs.Add(rb);
        }

        if (other.gameObject.TryGetComponent<PlayerHandle>(out var component))
        {
            component.DisableInput();
            component.SetEnableInput(PlayerHandle.PlayerInputType.Jump, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 各種オブジェクトが離れた時のコンポーネント削除処理
        if (other.gameObject.TryGetComponent<PlayerController>(out var rb))
        {
            rb.ResetForce();
            _rbs.Remove(rb);
        }

        if (other.gameObject.TryGetComponent<PlayerHandle>(out var component))
        {
            component.EnableInput();
        }
    }

    /// <summary>
    /// 坂の下り方向を計算する
    /// </summary>
    /// <param name="normal">坂の法線</param>
    /// <returns>下りの方向ベクトル</returns>
    private Vector3 CaluculateSlopeDownDirection(Vector3 normal)
    {
        // 重力を斜面上に投影し、重力の影響による進行方向を計算
        Vector3 projectedGravity = Vector3.ProjectOnPlane(Physics.gravity, normal);

        // 計算結果が斜面であるかを判定し、斜面の場合は計算結果の正規化ベクトルを返却
        Vector3 slopeDownDirection = Vector3.zero;
        if (projectedGravity.sqrMagnitude > 0.0001f)
        {
            slopeDownDirection = projectedGravity.normalized;
        }

        return slopeDownDirection;
    }
}
