using System.ComponentModel;
using UnityEngine;

/// <summary>
/// プレイヤーのインタラクト処理
/// </summary>
public class PlayerInteract : MonoBehaviour
{
    private IInteractable _interactable;

    /// <summary>
    /// インタラクトを実行します
    /// </summary>
    public bool Execute()
    {
        if (_interactable == null)
        {
            Debug.Log("インタラクト対象がありません");
            return false;
        }

        if (_interactable == null) return false;


        _interactable.OnInteract();
        return true;
    }

    /// <summary>
    /// インタラクトを実行します
    /// </summary>
    /// <remarks>
    /// プレイヤー判別用の関数を使用します
    /// </remarks>
    /// <param name="playerIndex"></param>
    /// <returns></returns>
    public bool Execute(int playerIndex)
    {
        if (_interactable == null)
        {
            Debug.Log("インタラクト対象がありません");
            return false;
        }

        if (_interactable == null) return false;


        _interactable.OnInteract(playerIndex);
        return true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            _interactable = interactable;
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            if (_interactable == interactable)
            {
                _interactable = null;
            }
        }
    }
}
