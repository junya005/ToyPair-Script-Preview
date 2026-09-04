using UnityEngine;

public class ShadowRendererMover : MonoBehaviour
{
    [SerializeField]
    private Transform _playerTransform;

    [SerializeField]
    private int _enableLayerId;

    [SerializeField]
    private float _maxHeight = 10f;

    private Vector3 _initLocalScale;

    void Awake()
    {
        _initLocalScale = this.transform.localScale;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        float y = 0;

        int layer = ~(1 << _enableLayerId);

        Ray ray = new(_playerTransform.position, Vector3.down);
        if(Physics.Raycast(ray,out RaycastHit hit, float.PositiveInfinity, layer))
        {
            // Debug.Log($"{hit.collider.gameObject.name}");
            y = hit.point.y;
        }

        float distanceY = _playerTransform.position.y - y;

        float clampedY = Mathf.Clamp01(distanceY / _maxHeight);
        float sizeOffsetY = Mathf.Lerp(1f, 0f, clampedY);

        this.transform.position = new Vector3(_playerTransform.position.x, y + 0.01f, _playerTransform.position.z);
        this.transform.localScale = _initLocalScale * sizeOffsetY;
    }
}
