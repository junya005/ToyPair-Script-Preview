using UnityEngine;

/// <summary>
/// プレイヤー内部の参照を取得するためのクラス
/// </summary>
public class PlayerReferenceGetter : MonoBehaviour
{
    [Header("オブジェクト参照")]
    [SerializeField, Tooltip("プレイヤー操作コンポーネント")]
    private PlayerHandle _playerHandle;

    [SerializeField, Tooltip("プレイヤー動作制御コンポーネント")]
    private PlayerController _playerController;

    [SerializeField, Tooltip("プレイヤーの本体オブジェクト")]
    private GameObject _body;

    /// <summary>
    /// プレイヤーの操作入力を受け付けるコンポーネントを取得する
    /// </summary>
    public PlayerHandle GetPlayerHandle()
    {
        return _playerHandle;
    }

    /// <summary>
    /// プレイヤー動作を制御するコンポーネントを取得する
    /// </summary>
    public PlayerController GetPlayerController()
    {
        return _playerController;
    }

    /// <summary>
    /// プレイヤーの本体を取得する
    /// </summary>
    public GameObject GetBody()
    {
        return _body;
    }
}
