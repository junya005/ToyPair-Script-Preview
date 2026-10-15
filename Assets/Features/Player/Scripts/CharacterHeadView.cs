using System;
using UnityEngine;

/// <summary>
/// キャラクターの向きを計算するコンポート
/// </summary>
/// <remarks>
/// このコンポーネントは、キャラクターの進行方向を決定する際に使用できるAPIを用意しています
/// </remarks>
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
    [SerializeField, Tooltip("プレイヤー入力を適用する速度の境界値")]
    private float _applyPlayerInputValue = 0.5f;

    private Vector2 _currentMoveInputValue;

    private void Update()
    {
        Vector3 currentVelocity = _rb.linearVelocity;
        Transform target = _targetTransform != null ? _targetTransform : this.transform;

        // 頭が向く方向を用いて計算する
        Quaternion rotation = Quaternion.identity;

        // 壁で止まった時など、速度の値を適用するとプレイヤーが想定している方向を向かないので、
        // 速度が小さい場合はプレイヤーの入力を採用して計算するようにします
        if (Mathf.Abs(currentVelocity.magnitude) >= _applyPlayerInputValue * 3.0f * 3.0f ||
            _currentMoveInputValue == Vector2.zero)
        {
            rotation = CalculateHeadRot(new Vector2(currentVelocity.x, currentVelocity.z));
        }
        else
        {
            rotation = CalculateHeadRot(new Vector2(_currentMoveInputValue.x, _currentMoveInputValue.y));
        }
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

    /// <summary>
    /// 現在のプレイヤーの入力値を保存します
    /// </summary>
    /// <remarks>
    /// このAPIを使用すると、プレイヤーの動きが小さいときの向きを
    /// プレイヤーの入力基準に使用するようになります
    /// </remarks>
    /// <param name="value"></param>
    public void SetCurrentMoveInputValue(Vector2 value)
    {
        _currentMoveInputValue = value;
    }

    /// <summary>
    /// ベクトル方向から頭の回転方向を計算する
    /// </summary>
    /// <param name="horizontalDirection"></param>
    /// <returns></returns>
    private Quaternion CalculateHeadRot(Vector2 horizontalDirection)
    {
        return Quaternion.LookRotation(new Vector3(
                horizontalDirection.x,
                0,
                horizontalDirection.y
            ));
    }
}
