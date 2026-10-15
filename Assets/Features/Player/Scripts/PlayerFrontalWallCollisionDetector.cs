using UnityEngine;
using System;

/// <summary>
/// プレイヤーが壁に正面から衝突したか確認するクラス
/// </summary>
public class PlayerFrontalWallCollisionDetector : MonoBehaviour
{
    public bool IsCollition { get; private set; }
    private Rigidbody _rb;

    [Header("パラメーター")]
    [SerializeField, Tooltip("対象レイヤー")]
    private LayerMask _targetLayer;

    [SerializeField, Tooltip("壁までの判定の長さを設定してください")]
    private float _rayDistance = 1.0f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (_rb == null) { return; }

        float vx = _rb.linearVelocity.x;
        float vz = _rb.linearVelocity.z;

        Vector3 velocityXZ = new Vector3(vx, 0, vz);

        // 水平な壁に激突したかを計算
        bool isHorizontal = false;
        if (Physics.Raycast(this.transform.position, velocityXZ.normalized, out var hit, _rayDistance, _targetLayer))
        {
            if(hit.collider.isTrigger == false)
            {
                isHorizontal = Mathf.Abs(Vector3.Dot(hit.normal, Vector3.up)) < 0.1f;
            }
        }

        IsCollition = isHorizontal;
    }
}
