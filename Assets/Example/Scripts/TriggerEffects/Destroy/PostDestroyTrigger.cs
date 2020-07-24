using Unity.Entities;

namespace TriggerSystem.Example
{
	public struct PostDestroyTrigger : ISystemStateComponentData
	{
		public int TriggerId;
		public int SkipFrames;
	}
}