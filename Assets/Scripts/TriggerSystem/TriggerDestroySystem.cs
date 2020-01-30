using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class TriggerDestroySystem : ComponentSystem
{
	private EntityQuery _entityQuery;

	private TriggerInitSystem _triggerInitSystem;

	protected override void OnCreate()
	{
		_entityQuery = GetEntityQuery(typeof(Initialized),
			typeof(TriggerComponent), typeof(DestroyComponent), typeof(Transform));
	}

	protected override void OnStartRunning()
	{
		_triggerInitSystem = EntityManager.World.GetExistingSystem<TriggerInitSystem>();
	}

	protected override void OnUpdate()
	{
		var triggers = _entityQuery.ToComponentDataArray<TriggerComponent>(Allocator.TempJob);
		var entities = _entityQuery.ToEntityArray(Allocator.TempJob);

		var transforms = _entityQuery.ToComponentArray<Transform>();

		for (var i = 0; i < triggers.Length; i++)
		{
			if (_triggerInitSystem.ColliderToTriggerEntity.ContainsKey(triggers[i].TriggerId))
			{
				_triggerInitSystem.ColliderToTriggerEntity.Remove(triggers[i].TriggerId);
				PostUpdateCommands.DestroyEntity(entities[i]);
				Object.Destroy(transforms[i].gameObject);
			}
		}

		triggers.Dispose();
		entities.Dispose();
	}
}