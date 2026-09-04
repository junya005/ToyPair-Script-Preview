/// <summary>
/// インタラクト可能なスクリプトに実装するインターフェース
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// インタラクトを実行する
    /// </summary>
    void OnInteract();

    void OnInteract(int playerIndex) { }
}
