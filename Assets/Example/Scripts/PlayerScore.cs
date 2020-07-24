using Unity.Entities;

namespace TriggerSystem.Example
{
	[GenerateAuthoringComponent]
	public struct PlayerScore : IComponentData
	{
		public int Score;
	}
}