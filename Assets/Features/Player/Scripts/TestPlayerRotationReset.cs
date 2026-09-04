using UnityEngine;

/// <summary>
/// プレイヤー回転リセット機能のテスト用スクリプト
/// </summary>
public class TestPlayerRotationReset : MonoBehaviour
{
    [SerializeField]
    private PlayerController _playerController;

    public void Execute()
    {
        _playerController.RestRotation();
    }
}
