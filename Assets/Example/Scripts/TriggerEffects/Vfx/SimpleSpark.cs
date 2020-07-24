using Unity.Entities;

namespace TriggerSystem.Example
{
	[GenerateAuthoringComponent]
	public struct SimpleSpark : IComponentData
	{
		public float DestroyDelay;
		public float Timer;
	}
}