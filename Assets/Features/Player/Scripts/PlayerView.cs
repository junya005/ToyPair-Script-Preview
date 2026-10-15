using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// プレイヤーの表示を統括するクラス
/// </summary>
[RequireComponent(typeof(PlayerAnimation))]
[RequireComponent(typeof(PlayerEffect))]
public class PlayerView : MonoBehaviour
{
    [Header("コンポーネント参照")]
    [SerializeField, Tooltip("プレイヤーのアニメーションを制御するコンポーネント")]
    private PlayerAnimation _playerAnimation;

    [SerializeField, Tooltip("プレイヤーのエフェクトを制御するコンポーネント")]
    private PlayerEffect _playerEffect;

    [SerializeField, Tooltip("プレイヤーの物理挙動を要するメッシュ")]
    private MeshRenderer _meshRenderer;

    [SerializeField, Tooltip("プレイヤーの物理挙動がないメッシュを制御するコンポーネント")]
    private PlayerSubMesh _playerSubMesh;

    [SerializeField, Tooltip("キャラクターの頭の方向を制御するコンポーネント")]
    private CharacterHeadView _characterHeadView;

    [SerializeField, Tooltip("キャラクターのスピンを制御するコンポーネント")]
    private PlayerSpin _playerSpin;

    [SerializeField, Tooltip("音の再生を制御するコンポーネント")]
    private PlayerSounds _playerSounds;

    private void Awake()
    {
        // メッシュレンダラの取得
        if (TryGetComponent<MeshRenderer>(out var component))
        {
            _meshRenderer = component;
        }
    }

    public void SetAnimParameter(string id, bool value)
    {
        _playerAnimation.SetBool(id, value);
    }

    public void SetAnimParameter(string id, float value)
    {
        _playerAnimation.SetFloat(id, value);
    }

    public void SetAnimTrigger(string id)
    {
        _playerAnimation.SetTrigger(id);
    }

    /// <summary>
    /// プレイヤー死亡時のアニメーションを再生する非同期処理
    /// （アニメーションを待つのでなく、指定秒数待つという形なので、自由に待機時間を指定できます）
    /// </summary>
    /// <param name="duration">待機秒数</param>
    /// <param name="cb">待機後に実行する処理</param>
    /// <param name="cancellationToken">キャンセル用トークン</param>
    public async UniTask PlayDeathAnimationAsync(float duration, Action cb,
                                                CancellationToken cancellationToken = default)
    {
        // エフェクト再生
        _playerEffect.PlayDeathEffect(duration, this.transform.position);

        // アニメーション再生
        await _playerAnimation.SetTriggerAsync(duration, "Death", cb, cancellationToken);
    }

    /// <summary>
    /// プレイヤーのスポーン時のアニメーションを再生する非同期処理
    /// （アニメーションを待つのでなく、指定秒数待つという形なので、自由に待機時間を指定できます）
    /// </summary>
    /// <param name="duration">待機秒数</param>
    /// <param name="cb">待機後に実行する処理</param>
    /// <param name="cancellationToken">キャンセル用トークン</param>
    public async UniTask PlayRespawnAnimationAsync(float duration, Action cb,
                                                    CancellationToken cancellationToken = default)
    {
        // アニメーション再生
        await _playerAnimation.SetTriggerAsync(duration, "Respawn", cb, cancellationToken);
    }

    /// <summary>
    /// プレイヤーのメッシュの表示/非表示を設定する
    /// </summary>
    /// <param name="enable">true: 表示、false: 非表示</param>
    public void EnableMesh(bool enable)
    {
        // 床に接しているメッシュを設定
        if (_meshRenderer != null)
        {
            _meshRenderer.enabled = enable;
        }

        // 床に接していないメッシュを設定
        if (_playerSubMesh != null)
        {
            _playerSubMesh.EnableAllMeshs(enable);
        }
    }

    /// <summary>
    /// プレイヤーのサブの見た目の表示/非表示を設定する
    /// </summary>
    /// <param name="enable">true: 表示、false: 非表示</param>
    public void SetEnablePlayerSubMesh(bool enable)
    {
        if (_playerSubMesh != null)
        {
            _playerSubMesh.EnableAllMeshObjects(enable);
        }
    }

    /// <summary>
    /// プレイヤーの頭の方向を取得します
    /// </summary>
    public Vector3 GetHeadDerection()
    {
        return _characterHeadView.GetHeadDerection();
    }

    /// <summary>
    /// スピンアニメーションを実行します
    /// </summary>
    public void ExecuteSpinAnim()
    {
        _playerSpin.Execute();
    }

    public void PlaySoundsOneShot(PlayerSounds.SoundType type)
    {
        _playerSounds.PlayOneShot(type);
    }

    public void PlaySoundsLoop(PlayerSounds.SoundType type)
    {
        _playerSounds.PlayLoop(type);
    }

    public void StopSoundsLoop()
    {
        _playerSounds.StopLoop();
    }

    public void PlayEffectForDuration(string id, float duration)
    {
        _playerEffect.PlayEffectForDuration(id, duration);
    }

    public void StartLoopEffect(string id)
    {
        _playerEffect.StartLoopEffect(id);
    }

    public void StopLoopEffect(string id)
    {
        _playerEffect.StopLoopEffect(id);
    }

    public void ClearEffect(string id)
    {
        _playerEffect.ClearEffect(id);
    }

    public void SetEffectLocalPosition(string id, Vector3 position)
    {
        _playerEffect.SetEffectLocalPosition(id, position);
    }

    public void SetEffectLocalRotation(string id, Quaternion rotation)
    {
        _playerEffect.SetEffectLocalRotation(id, rotation);
    }

    public void SetMoveInputValueToCharacterHead(Vector2 value)
    {
        _characterHeadView.SetCurrentMoveInputValue(value);
    }
}
