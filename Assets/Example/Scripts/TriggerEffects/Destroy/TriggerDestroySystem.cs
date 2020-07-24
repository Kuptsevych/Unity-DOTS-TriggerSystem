using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace TriggerSystem.Example
{
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	public class TriggerDestroySystem : ComponentSystem
	{
		private EntityQuery _entityQuery;

		private TriggerInitSystem _triggerInitSystem;

		protected override void OnCreate()
		{
			_entityQuery = GetEntityQuery(new EntityQueryDesc
			{
				All = new[]
				{
					ComponentType.ReadOnly<Initialized>(),
					ComponentType.ReadOnly<TriggerComponent>(),
					ComponentType.ReadOnly<DestroyComponent>(),
					ComponentType.ReadOnly<BoxCollider2D>()
				},
				None = new[]
				{
					ComponentType.ReadOnly<PostDestroyTrigger>()
				}
			});
		}

		protected override void OnStartRunning()
		{
			_triggerInitSystem = EntityManager.World.GetExistingSystem<TriggerInitSystem>();
		}

		protected override void OnUpdate()
		{
			var triggers = _entityQuery.ToComponentDataArray<TriggerComponent>(Allocator.TempJob);
			var entities = _entityQuery.ToEntityArray(Allocator.TempJob);

			var colliders = _entityQuery.ToComponentArray<BoxCollider2D>();

			for (var i = 0; i < triggers.Length; i++)
			{
				if (_triggerInitSystem.ColliderToTriggerEntity.ContainsKey(triggers[i].TriggerId))
				{
					if (!EntityManager.HasComponent<PostDestroyTrigger>(entities[i]))
					{
						PostUpdateCommands.AddComponent(entities[i], new PostDestroyTrigger
						{
							TriggerId = triggers[i].TriggerId,
							SkipFrames = 3
						});
					}
					
					colliders[i].enabled = false;
				}
			}

			triggers.Dispose();
			entities.Dispose();
		}
	}
}