using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class DetectorInitSystem : ComponentSystem
{
	public NativeHashMap<int, Entity> ColliderToDetectorEntity = new NativeHashMap<int, Entity>(1024, Allocator.Persistent);

	private EntityQuery _entityQuery;

	protected override void OnCreate()
	{
		var queryDesc = new EntityQueryDesc
		{
			None = new[] {ComponentType.ReadOnly<Initialized>()},
			All  = new[] {ComponentType.ReadOnly<DetectorComponent>(), ComponentType.ReadOnly<BoxCollider2D>(),}
		};

		_entityQuery = GetEntityQuery(queryDesc);
	}

	protected override void OnUpdate()
	{
		var detectors = _entityQuery.ToComponentDataArray<DetectorComponent>(Allocator.TempJob);
		var colliders = _entityQuery.ToComponentArray<BoxCollider2D>();
		var entities  = _entityQuery.ToEntityArray(Allocator.TempJob);

		for (var i = 0; i < detectors.Length; i++)
		{
			DetectorComponent detector = detectors[i];

			detector.DetectorId = colliders[i].GetInstanceID();

			detector.Size = colliders[i].size;

			detectors[i] = detector;

			ColliderToDetectorEntity.TryAdd(colliders[i].GetInstanceID(), entities[i]);

			PostUpdateCommands.AddComponent(entities[i], new Initialized());
		}

		_entityQuery.CopyFromComponentDataArray(detectors);

		entities.Dispose();
		detectors.Dispose();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ColliderToDetectorEntity.Dispose();
	}
}