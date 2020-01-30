using Unity.Entities;

[GenerateAuthoringComponent]
public struct TriggerEffectVfxComponent : IComponentData
{
	public Entity VfxPrefab;
}