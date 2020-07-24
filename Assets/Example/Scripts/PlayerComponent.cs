using Unity.Entities;

namespace TriggerSystem.Example
{
	[GenerateAuthoringComponent]
	public struct PlayerComponent : IComponentData
	{
		public float Speed;
		public int   Score;
	}
}