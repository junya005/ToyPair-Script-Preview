using UnityEngine;

/// <summary>
/// パンチ動作のインタラクト
/// </summary>
public class PunchInteract : MonoBehaviour, IInteractable
{
    [SerializeField, Tooltip("パンチ操作のハンドラを設定してください")]
    private PunchHandler _punchHandler;

    private bool _isControl = false;

    public void OnInteract()
    {

    }

    public void OnInteract(int playerIndex)
    {
        if (_punchHandler == null) return;

        // 操作中であればハンドラをリセット、そうで無ければハンドラを設定
        if (_isControl == true)
        {
            InputManager.Instance?.ResetHandle(playerIndex);
            _isControl = false;
        }
        else if (_punchHandler != null && _isControl == false)
        {
            InputManager.Instance?.SetCurrentHandle(playerIndex, _punchHandler);
            _punchHandler.SetControlPlayerIndex(playerIndex);
            _isControl = true;
        }
    }
}
