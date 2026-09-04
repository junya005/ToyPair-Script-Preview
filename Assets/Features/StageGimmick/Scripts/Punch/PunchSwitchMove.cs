using UnityEngine;

/// <summary>
/// パンチギミックのスイッチを移動させるクラス
/// </summary>
public class PunchSwitchMove : MonoBehaviour
{
    [Header("パラメータ")]
    [SerializeField, Tooltip("移動の最小Y座標")]
    private float _min;

    [SerializeField, Tooltip("移動の最大Y座標")]
    private float _max;

    [SerializeField, Tooltip("移動の初期進捗率（初期位置に影響します）")]
    private float _initProgress;

    [SerializeField, Tooltip("何秒で端から端に達するか")]
    private float _halfDuration;

    [SerializeField, Tooltip("端についたときに待機する時間")]
    private float _stopDuration;

    private Vector3 _initPos;

    private float _progress;

    private float _time;
    private bool _isDown = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _initPos = this.transform.localPosition;
        _time = _halfDuration * _initProgress;
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime * (_isDown ? 1 : -1);

        _progress = _time / _halfDuration;

        if (_progress > 1.0f)
        {
            _time = _halfDuration;
            _isDown = false;
        }

        if (_progress < 0.0f)
        {
            _time = 0.0f;
            _isDown = true;
        }
    }

    private void FixedUpdate()
    {
        this.LimitPosOnFixedUpdate();

        this.transform.localPosition = Vector3.Lerp(
            new Vector3(_initPos.x, _min, _initPos.z),
            new Vector3(_initPos.x, _max, _initPos.z),
            _progress
        );
    }

    /// <summary>
    /// 座標の制限を掛ける
    /// </summary>
    /// <remarks>
    /// FixedUpdateから呼ばれることを想定
    /// </remarks>
    private void LimitPosOnFixedUpdate()
    {
        float limitVecY = this.transform.localPosition.y;
        if (this.transform.localPosition.y < _min)
        {
            limitVecY = _min;
        }
        else if (this.transform.localPosition.y > _max)
        {
            limitVecY = _max;
        }

        this.transform.localPosition = new Vector3(this.transform.localPosition.x, limitVecY, this.transform.localPosition.z);
    }
}
