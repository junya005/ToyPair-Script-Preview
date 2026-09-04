using System;
using UnityEngine;

class CharacterHeadView : MonoBehaviour
{
    [SerializeField, Tooltip("速度制限コンポーネント")]
    private VelocityLimiter _velocityLimiter;
    [SerializeField, Tooltip("ターゲットオブジェクト")]
    private Transform _targetTransform;
    [SerializeField, Tooltip("剛体コンポーネント")]
    private Rigidbody _rb;
    [SerializeField, Tooltip("旋回速度")]
    private float _speed;
    [SerializeField, Tooltip("前のめりの最大角度")]
    private float _maxLearnRot;

    private void Update()
    {
        // 完全に停止していると正面向きに戻ってしまうため、その場合は無視する
        if (_rb.linearVelocity == Vector3.zero) { return; }

        Vector3 currentVelocity = _rb.linearVelocity;
        Transform target = _targetTransform != null ? _targetTransform : this.transform;

        // 頭が向く方向を用いて計算する
        Quaternion rotation = Quaternion.LookRotation(new Vector3(
            currentVelocity.x,
            0.0f,
            currentVelocity.z
            ));
        Vector3 eular = rotation.eulerAngles;

        // 傾き度合いの計算
        float persentage = Mathf.Clamp01(currentVelocity.magnitude / _velocityLimiter.BaseMaxSpeed);
        float targetXAngle = persentage * _maxLearnRot;
        Quaternion learnRotation = Quaternion.Euler(targetXAngle, 0.0f, 0.0f);

        Quaternion targetRotation = Quaternion.Euler(eular + learnRotation.eulerAngles);

        // 補完を適用しつつ回転を反映
        target.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, _speed * Time.deltaTime);
    }

    /// <summary>
    /// 頭が向いている方向を取得します。（水平方向のみ）
    /// </summary>
    /// <returns>頭が向いている方向のベクトル</returns>
    public Vector3 GetHeadDerection()
    {
        return new Vector3(
            _targetTransform.forward.x,
            0.0f,
            _targetTransform.forward.z
            );
    }
}
