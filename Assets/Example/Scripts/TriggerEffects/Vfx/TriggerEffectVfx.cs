using Unity.Entities;

public struct TriggerEffectVfxComponent : IComponentData
{
	public Entity VfxPrefab;
	public float  DepthDelta;
	public bool   Used;
}