using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(NodePool))]
public class LightningGenerator : MonoBehaviour
{
    [field: SerializeField] public VisualEffectAsset LightningEffectAsset { get; private set; }
    private Dictionary<Transform, VisualEffect> _effects = new();
    private GraphicsBuffer _buffer;
    private Transform[] _idleNodes = new Transform[25];

    private void OnDestroy()
    {
        _buffer?.Release();
    }

    private void Update()
    {
        UpdateVfx();
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform t = other.transform;
        _effects[t] = GenerateEffect(t);
        Debug.Log("Entering trigger");
    }

    private void OnTriggerExit(Collider other)
    {
        Transform t = other.transform;
        if (_effects.TryGetValue(t, out VisualEffect effect))
        {
            Destroy(effect.gameObject);
            _effects.Remove(t);
        }
        else { Debug.LogWarning("could not find vfx to destroy"); }

        Debug.Log("Exiting trigger");
    }

    private VisualEffect GenerateEffect(Transform targetTransform)
    {
        var obj = new GameObject("VFX_Instance");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        var vfxComponent = obj.AddComponent<VisualEffect>();
        vfxComponent.visualEffectAsset = LightningEffectAsset;
        vfxComponent.SetVector3("Target", targetTransform.position);

        _effects[targetTransform] = vfxComponent;
        return vfxComponent;
    }

    private void UpdateVfx()
    {
        foreach (Transform key in _effects.Keys) { _effects[key].SetVector3("Target", key.position); }
    }
}