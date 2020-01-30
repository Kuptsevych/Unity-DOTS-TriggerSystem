using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct DetectorComponent : IComponentData
{
	public int    DetectorId;
	public int    TriggersCount;
	public float2 Size;
}