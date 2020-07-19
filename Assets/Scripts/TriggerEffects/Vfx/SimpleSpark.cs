using Unity.Entities;

[GenerateAuthoringComponent]
public struct SimpleSpark : IComponentData
{
	public float DestroyDelay;
	public float Timer;
}