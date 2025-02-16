using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;

[RequireComponent(typeof(LightningGenerator))]
public class NodePool : MonoBehaviour
{
    [SerializeField] private int _nodeCount = 25;
    
    private LightningGenerator _lightningGenerator;
    private VisualEffectAsset _vfxAsset;
    private IObjectPool<Transform> _pool;

    private void Awake()
    {
        _lightningGenerator = GetComponent<LightningGenerator>();
    }

    private void Start()
    {
        
        _vfxAsset = _lightningGenerator.LightningEffectAsset;
        _pool = new ObjectPool<Transform>(
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

    private void PrewarmPool()
    {
        Stack<Transform> objs = new();
        for (int i = 0; i < _nodeCount; i++)
        {
            objs.Push(_pool.Get());
        }
        
        for (int i = 0; i < _nodeCount; i++)
        {
            _pool.Release(objs.Pop());            
        }
    }


    private Transform OnNodeCreate()
    {
        var obj = new GameObject("VFX_Pool_Instance");
        obj.transform.SetParent(_lightningGenerator.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        var vfxComponent = obj.AddComponent<VisualEffect>();
        vfxComponent.visualEffectAsset = _vfxAsset;
        vfxComponent.SetVector3("Target", Vector3.zero);
        return vfxComponent.transform;
    }

    private void OnNodeGet(Transform obj)
    {
        obj.gameObject.SetActive(true);
    }

    private void OnNodeRelease(Transform obj)
    {
        // Would normally turn off but we want idle functionality
    }

    private void OnNodeDestroy(Transform obj)
    {
        Destroy(obj); // don't think we'll be doing this but whatevs
    }
}