using Unity.Entities;

namespace Game
{
	public class OnPlayerTriggerCoin : SystemBase
	{
		private EndSimulationEntityCommandBufferSystem _endSimulationEcbSystem;

		protected override void OnCreate()
		{
			_endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		}

		protected override void OnUpdate()
		{
			var commandBuffer = _endSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();

			Entities.ForEach((Entity e, int entityInQueryIndex, ref PlayerComponent playerComponent, ref TriggerCoin triggerCoin) =>
			{
				playerComponent.Score += triggerCoin.Count;
				commandBuffer.RemoveComponent<TriggerCoin>(entityInQueryIndex, e);
			}).Schedule();

			_endSimulationEcbSystem.AddJobHandleForProducer(Dependency);
		}
	}
}