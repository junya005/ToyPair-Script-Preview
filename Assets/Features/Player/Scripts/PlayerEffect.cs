using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// プレイヤーのエフェクトを制御するクラス
/// </summary>
public class PlayerEffect : MonoBehaviour
{
    /// <summary>エフェクトのハッシュマップ用データクラス</summary>
    [System.Serializable]
    public class EffectData
    {
        public string id;
        public GameObject effect;
    }

    [Header("オブジェクト参照")]
    [SerializeField, Tooltip("死亡時のエフェクトを設定してください")]
    private GameObject _deathEffect;

    [SerializeField, Tooltip("エフェクトのハッシュを作成してください")]
    private List<EffectData> _effectDataList = new List<EffectData>();

    private Dictionary<string, GameObject> _effectMap = new Dictionary<string, GameObject>();
    private Dictionary<string, CancellationTokenSource> _cancellationTokenMap
                                                = new Dictionary<string, CancellationTokenSource>();


    private void Awake()
    {
        _effectMap.Clear();

        foreach (var data in _effectDataList)
        {
            _effectMap.Add(data.id, data.effect);
        }
    }

    private void OnDestroy()
    {
        foreach (var data in _effectDataList)
        {
            ClearEffect(data.id);
        }
    }

    /// <summary>
    /// 死亡エフェクトを再生する
    /// </summary>
    /// <param name="duration">再生秒数</param>
    /// <param name="position">オブジェクトの出現座標</param>
    public void PlayDeathEffect(float duration, Vector3 position)
    {
        _deathEffect.transform.position = position;

        // 非同期の呼び出し
        PlayDeathEffectAsync(duration, position).Forget();
    }

    /// <summary>
    /// 非同期でプレイヤーのエフェクトを再生
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private async UniTask PlayDeathEffectAsync(float duration, Vector3 position)
    {
        _deathEffect.SetActive(true);
        await UniTask.WaitForSeconds(duration);
        _deathEffect.SetActive(false);
    }

    public void PlayEffectForDuration(string id, float duration)
    {
        if (_effectMap.ContainsKey(id))
        {
            GameObject effect = _effectMap[id];

            if (_cancellationTokenMap.ContainsKey(id))
            {
                _cancellationTokenMap[id].Cancel();
                _cancellationTokenMap.Remove(id);
            }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            _cancellationTokenMap.Add(id, cancellationTokenSource);

            PlayEffectForDurationAsync(effect, duration, cancellationToken).Forget();
        }
    }

    private async UniTask PlayEffectForDurationAsync(GameObject effect, float duration,
                                                    CancellationToken cancellationToken = default)
    {
        effect.SetActive(true);

        try
        {
            await UniTask.WaitForSeconds(duration, cancellationToken: cancellationToken);
            effect.SetActive(false);
        }
        catch (System.OperationCanceledException ex)
        {
            Debug.Log($"[PlayerEffect] エフェクト再生は正常にキャンセルされました: {ex}");
            effect.SetActive(false);
        }
    }

    public void StartLoopEffect(string id)
    {
        if (_effectMap.ContainsKey(id))
        {
            GameObject effect = _effectMap[id];
            effect.SetActive(true);
        }
    }

    public void StopLoopEffect(string id)
    {
        if (_effectMap.ContainsKey(id))
        {
            GameObject effect = _effectMap[id];
            effect.SetActive(false);
        }
    }

    public void ClearEffect(string id)
    {
        if (_cancellationTokenMap.ContainsKey(id))
        {
            _cancellationTokenMap[id].Cancel();
            _cancellationTokenMap.Remove(id);
        }

        if (_effectMap.ContainsKey(id))
        {
            _effectMap[id].SetActive(false);
        }
    }

    public void SetEffectLocalPosition(string id, Vector3 position)
    {
        if (_effectMap.ContainsKey(id))
        {
            _effectMap[id].transform.localPosition = position;
        }
    }

    public void SetEffectLocalRotation(string id, Quaternion rotation)
    {
        if (_effectMap.ContainsKey(id))
        {
            _effectMap[id].transform.localRotation = rotation;
        }
    }
}
