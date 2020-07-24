using Unity.Entities;
using Unity.Mathematics;


namespace TriggerSystem.Example
{
	[GenerateAuthoringComponent]
	public struct InputComponent : IComponentData
	{
		public float2 Axes;
		public bool   PrepareAttack;
		public bool   Attack;
	}
}