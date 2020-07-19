using Unity.Entities;

public struct TriggerEffectVfxComponent : IComponentData
{
	public Entity VfxPrefab;
	public bool   Used;
}