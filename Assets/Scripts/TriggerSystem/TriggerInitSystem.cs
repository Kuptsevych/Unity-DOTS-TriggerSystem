using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class TriggerInitSystem : ComponentSystem
{
	public NativeHashMap<int, Entity> ColliderToTriggerEntity = new NativeHashMap<int, Entity>(1024, Allocator.Persistent);

	private EntityQuery _entityQuery;

	protected override void OnCreate()
	{
		var queryDesc = new EntityQueryDesc
		{
			None = new[] {ComponentType.ReadOnly<Initialized>()},
			All  = new[] {ComponentType.ReadOnly<TriggerComponent>(), ComponentType.ReadOnly<BoxCollider2D>(),}
		};

		_entityQuery = GetEntityQuery(queryDesc);
	}

	protected override void OnUpdate()
	{
		var triggers  = _entityQuery.ToComponentDataArray<TriggerComponent>(Allocator.TempJob);
		var colliders = _entityQuery.ToComponentArray<BoxCollider2D>();
		var entities  = _entityQuery.ToEntityArray(Allocator.TempJob);

		for (var i = 0; i < triggers.Length; i++)
		{
			TriggerComponent trigger = triggers[i];

			trigger.TriggerId = colliders[i].GetInstanceID();

			triggers[i] = trigger;

			ColliderToTriggerEntity.TryAdd(colliders[i].GetInstanceID(), entities[i]);

			PostUpdateCommands.AddComponent(entities[i], new Initialized());
		}

		_entityQuery.CopyFromComponentDataArray(triggers);

		entities.Dispose();
		triggers.Dispose();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ColliderToTriggerEntity.Dispose();
	}
}