using UnityEngine;

/// <summary>
/// インタラクトを所持するコンポーネントのテスト
/// </summary>
public class InteractableTest : MonoBehaviour, IInteractable
{
    /// <summary>
    /// インタラクト時に呼ばれる関数
    /// </summary>
    public void OnInteract()
    {
        Debug.Log("Hi!");
    }
}
