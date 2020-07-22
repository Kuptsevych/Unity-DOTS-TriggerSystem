using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlayerScore : IComponentData
{
	public int  Score;
}