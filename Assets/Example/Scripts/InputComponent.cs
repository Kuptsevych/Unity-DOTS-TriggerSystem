using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
	[GenerateAuthoringComponent]
	public struct InputComponent : IComponentData
	{
		public float2 Axes;
		public bool   PrepareAttack;
		public bool   Attack;
	}
}