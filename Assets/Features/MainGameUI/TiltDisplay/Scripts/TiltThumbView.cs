using UnityEngine;

/// <summary>
/// プレイヤーのジャイロ傾きUIを制御するコンポーネント
/// </summary>
public class TiltThumbView : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField, Tooltip("ターゲットのプレイヤーコントローラーを設定")]
    private PlayerController _playerController;

    [SerializeField, Tooltip("傾きを反映する対象の円形UI")]
    private RectTransform _targetThumbRectTransform = null;

    [SerializeField, Tooltip("表示UIのオブジェクト")]
    private GameObject _targetUIObject = null;

    [Header("Parameter")]
    [SerializeField, Tooltip("表示の最大半径")]
    private float _maxRadius = 50.0f;

    private Vector2 _initTargetAnchordPos = Vector2.zero;

    private Vector2 _targetInputValue = Vector2.zero;

    void Awake()
    {
        // 円を相対的に移動させるために、初期値を保存しておく
        _initTargetAnchordPos = _targetThumbRectTransform.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (_targetThumbRectTransform == null) return;

        if (_playerController != null)
        {
            _targetInputValue = InputManager.Instance.CurrentInput.MoveInputVector[_playerController.PlayerIndex];
            // Debug.Log($"[TiltThumbView] {_targetInputValue}");
        }

        // 入力値を円の移動に反映
        Vector2 inputValue = _targetInputValue;
        Vector2 targetPosition = _initTargetAnchordPos + inputValue * _maxRadius;

        _targetThumbRectTransform.anchoredPosition = targetPosition;
    }

    /// <summary>
    /// UIのオブジェクトの表示/非表示を切り替えます
    /// </summary>
    /// <param name="value">表示、非表示のbool</param>
    public void SetIsEnable(bool value)
    {
        _targetUIObject.SetActive(value);
    }

    /// <summary>
    /// 入力値をUIにバインドします
    /// </summary>
    /// <param name="inputValue">入力値</param>
    public void BindInputValue(Vector2 inputValue)
    {
        _targetInputValue = inputValue;
    }
}
