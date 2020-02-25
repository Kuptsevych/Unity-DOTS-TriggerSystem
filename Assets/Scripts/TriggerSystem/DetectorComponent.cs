using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct DetectorComponent : IComponentData
{
	public int    DetectorId;
	public int    TriggersCount;
	public int    TriggersCheckSum;
	public float2 Size;
}