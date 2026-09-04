using UnityEngine;

/// <summary>
/// 床に接しているか調べるクラス
/// Raycastを使用して判定
/// </summary>
public class CheckGround : MonoBehaviour
{
    [Header("パラメーター")]
    [SerializeField, Tooltip("床までの判定の長さを設定してください")]
    private float _rayDistance = 0.5f;

    public bool IsGrounded { get; private set; }

    void FixedUpdate()
    {
        IsGrounded = Physics.Raycast(this.transform.position, Vector3.down, _rayDistance);
    }

    /// <summary>
    /// 床までの判定の長さを設定する
    /// オブジェクトの原点が足元でない場合、長めに設定することをお勧めします。
    /// </summary>
    /// <param name="rayDistance">指定する長さ</param>
    public void SetRayDistance(float rayDistance)
    {
        _rayDistance = rayDistance;
    }
}
