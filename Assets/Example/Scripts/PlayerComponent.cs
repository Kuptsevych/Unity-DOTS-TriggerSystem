using Unity.Entities;

namespace Game
{
	[GenerateAuthoringComponent]
	public struct PlayerComponent : IComponentData
	{
		public float Speed;
		public int   Score;
	}
}