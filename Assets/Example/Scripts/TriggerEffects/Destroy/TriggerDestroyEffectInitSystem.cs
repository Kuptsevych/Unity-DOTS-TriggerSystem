// using Unity.Entities;
// using Unity.Jobs;
//
// [UpdateBefore(typeof(TriggerInitSystem))]
// public class TriggerDestroyEffectInitSystem : JobComponentSystem
// {
// 	private JobComponentSystem _jobComponentSystemImplementation;
//
// 	protected override JobHandle OnUpdate(JobHandle inputDependencies)
// 	{
// 		var jobHandle = Entities.WithNone<Initialized>()
// 			.ForEach((Entity e, ref TriggerEffectDestroyComponent destroyComponent, in TriggerComponent trigger) =>
// 			{
// 				destroyComponent.EntityForDestroy = e;
// 			}).Schedule(inputDependencies);
//
// 		return jobHandle;
// 	}
// }