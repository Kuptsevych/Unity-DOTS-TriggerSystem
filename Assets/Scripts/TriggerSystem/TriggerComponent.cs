using Unity.Entities;

[GenerateAuthoringComponent]
public struct TriggerComponent : IComponentData
{
	public int TriggerId;
}