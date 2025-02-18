using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;

[RequireComponent(typeof(LightningGenerator))]
public class NodePool : MonoBehaviour
{
    [SerializeField] private GameObject _nodePrefab; 
    [SerializeField] private int _nodeCount = 25;
    [SerializeField] private LayerMask _environmentLayerMask;
    [SerializeField] private float _nodeAngleRange = 30f;
    
    private LightningGenerator _lightningGenerator;
    private VisualEffectAsset _vfxAsset;
    private IObjectPool<GameObject> _pool;

    private void Awake()
    {
        _lightningGenerator = GetComponent<LightningGenerator>();
    }

    private void Start()
    {
        _vfxAsset = _lightningGenerator.LightningEffectAsset;
        _pool = new ObjectPool<GameObject>(
            OnNodeCreate,
            OnNodeGet,
            OnNodeRelease,
            OnNodeDestroy,
            false,
            _nodeCount,
            _nodeCount
        );

        PrewarmPool();
    }

    private void Update()
    {
        UpdateIdleNodes();
    }

    private void PrewarmPool()
    {
        Stack<GameObject> nodes = new();
        for (int i = 0; i < _nodeCount; i++)
        {
            GameObject node = _pool.Get();
            nodes.Push(node);
        }
        
        for (int i = 0; i < _nodeCount; i++)
        {
            _pool.Release(nodes.Pop());            
        }
    }


    private GameObject OnNodeCreate()
    {
        GameObject node = Instantiate(_nodePrefab, transform, true);
        node.transform.localPosition = Vector3.zero;
        node.transform.localRotation = Quaternion.identity;

        Vector3 randomDirection = GetRandomDirectionInCone(Vector3.down, _nodeAngleRange);
        if (Physics.Raycast(transform.position, randomDirection, out RaycastHit hit, Mathf.Infinity, layerMask: _environmentLayerMask))
        {
            node.transform.position = hit.point;
        }
        
        return node;
    }

    private void OnNodeGet(GameObject node)
    {
        node.gameObject.SetActive(true);
    }

    private void OnNodeRelease(GameObject node)
    {
    }

    private void OnNodeDestroy(GameObject node)
    {
        Destroy(node);
    }
    
    private Vector3 GetRandomDirectionInCone(Vector3 baseDirection, float maxAngle)
    {
        Quaternion randomRotation = Quaternion.Euler(Random.Range(-maxAngle, maxAngle), Random.Range(0f, 360f), 0);
        return randomRotation * baseDirection;
    }
    
    private void UpdateIdleNodes()
    {
        // TODO: need to spawn the vfx instances seaprately, have idles nodes, swap collision layers so idle nodes connect to those instead
    }
}