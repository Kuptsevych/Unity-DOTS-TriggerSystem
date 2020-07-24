using Unity.Collections;
using Unity.Entities;

namespace TriggerSystem.Example
{
	[UpdateAfter(typeof(TriggerDetectorSystem))]
	public class TriggerPostDestroySystem : ComponentSystem
	{
		private EntityQuery _entityQuery;

		private TriggerInitSystem _triggerInitSystem;

		protected override void OnCreate()
		{
			_entityQuery = GetEntityQuery(new EntityQueryDesc
			{
				All = new[] {ComponentType.ReadOnly<PostDestroyTrigger>(), ComponentType.ReadOnly<TriggerComponent>()},
			});
		}

		protected override void OnStartRunning()
		{
			_triggerInitSystem = EntityManager.World.GetExistingSystem<TriggerInitSystem>();
		}

		protected override void OnUpdate()
		{
			var postTriggers = _entityQuery.ToComponentDataArray<PostDestroyTrigger>(Allocator.TempJob);
			var entities     = _entityQuery.ToEntityArray(Allocator.TempJob);

			for (var i = 0; i < postTriggers.Length; i++)
			{
				var postTrigger = postTriggers[i];

				postTrigger.SkipFrames -= 1;

				if (postTrigger.SkipFrames < 0)
				{
					if (_triggerInitSystem.ColliderToTriggerEntity.ContainsKey(postTriggers[i].TriggerId))
					{
						_triggerInitSystem.ColliderToTriggerEntity.Remove(postTriggers[i].TriggerId);
						PostUpdateCommands.RemoveComponent<PostDestroyTrigger>(entities[i]);
						PostUpdateCommands.DestroyEntity(entities[i]);
					}
				}

				postTriggers[i] = postTrigger;
			}

			_entityQuery.CopyFromComponentDataArray(postTriggers);

			postTriggers.Dispose();
			entities.Dispose();
		}
	}
}