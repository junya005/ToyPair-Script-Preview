using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

/// <summary>
/// プレイヤーのアニメーション再生を行うクラス
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    [Header("オブジェクト参照")]
    [SerializeField, Tooltip("アニメーションコンポーネントを設定してください")]
    private Animator _animator;

    /// <summary>
    /// アニメーショントリガーを設定する
    /// </summary>
    /// <param name="id">設定するトリガーのId</param>
    public void SetTrigger(string id)
    {
        if (_animator == null) { return; }

        _animator.SetTrigger(id);
    }

/// <summary>
/// アニメーション真偽値パラメータを設定する
/// </summary>
/// <param name="id">設定するパラメータのid</param>
/// <param name="value">設定値</param>
    public void SetBool(string id, bool value)
    {
        if (_animator == null) { return; }

        _animator.SetBool(id, value);
    }

/// <summary>
/// アニメーションの小数値パラメータを設定する
/// </summary>
/// <param name="id">設定するパラメータのid</param>
/// <param name="value">設定する値</param>
    public void SetFloat(string id, float value)
    {
        if (_animator == null) { return; }

        _animator.SetFloat(id, value);
    }

    /// <summary>
    /// アニメーショントリガーを設定して待機する
    /// </summary>
    /// <param name="duration">待機秒数</param>
    /// <param name="id">設定パラメータのid</param>
    /// <param name="cb">待機後に実行する処理</param>
    /// <param name="cancellationToken">キャンセル用トークン</param>
    public async UniTask SetTriggerAsync(float duration, string id,
                                        Action cb = null, CancellationToken cancellationToken = default)
    {
        if (_animator == null) { return; }

        _animator.SetTrigger(id);
        await UniTask.WaitForSeconds(duration, cancellationToken: cancellationToken);
        cb();
    }
}
