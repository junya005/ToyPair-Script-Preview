using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 物理挙動を使用しないプレイヤーメッシュの制御を行うクラス
/// </summary>
public class PlayerSubMesh : MonoBehaviour
{
    [Header("コンポーネント参照")]
    [SerializeField, Tooltip("物理挙動を使用しないメッシュを設定してください")]
    private List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();

    [SerializeField]
    private List<GameObject> _meshObjects = new List<GameObject>();

    /// <summary>
    /// リストにあるメッシュの有効無効を全て設定します
    /// </summary>
    /// <param name="enable">設定値</param>
    public void EnableAllMeshs(bool enable)
    {
        foreach (var meshRenderer in _meshRenderers)
        {
            meshRenderer.enabled = enable;
        }
    }

    /// <summary>
    /// リストにあるオブジェクトの有効無効を全て設定します
    /// </summary>
    /// <param name="enable"></param>
    public void EnableAllMeshObjects(bool enable)
    {
        foreach (var meshObject in _meshObjects)
        {
            meshObject.SetActive(enable);
        }
    }
}
