using UnityEngine;

/// <summary>
/// パンチの土台操作のインタラクト
/// </summary>
public class PunchMoveInteract : MonoBehaviour, IInteractable
{
    [SerializeField, Tooltip("パンチ操作のハンドラを設定してください")]
    private PunchMoveHandler _punchMoveHandler;

    private bool _isControl = false;

    public void OnInteract()
    {

    }

    public void OnInteract(int playerIndex)
    {
        if (_punchMoveHandler == null) return;

        // 操作中であればハンドラをリセット、そうで無ければハンドラを設定
        if (_isControl == true)
        {
            InputManager.Instance?.ResetHandle(playerIndex);
            _isControl = false;
        }
        else if (_punchMoveHandler != null && _isControl == false)
        {
            InputManager.Instance?.SetCurrentHandle(playerIndex, _punchMoveHandler);
            _punchMoveHandler.SetControlPlayerIndex(playerIndex);
            _isControl = true;
        }
    }
}
