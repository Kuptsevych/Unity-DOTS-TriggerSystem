using Unity.Entities;

namespace Game
{
	public struct PostDestroyTrigger : ISystemStateComponentData
	{
		public int TriggerId;
	}
}