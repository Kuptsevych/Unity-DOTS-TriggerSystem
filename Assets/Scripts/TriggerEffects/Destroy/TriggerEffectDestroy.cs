using Unity.Entities;

[GenerateAuthoringComponent]
public struct TriggerEffectDestroyComponent : IComponentData
{
	public Entity EntityForDestroy;
}