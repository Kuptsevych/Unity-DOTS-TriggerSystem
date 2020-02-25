using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class DetectorOverlap2dSystem : ComponentSystem
{
	private EntityQuery _entityQuery;

	private const int MaxColliders = 8;

	private readonly LayerMask _triggerMask = LayerMask.GetMask("Triggers");

	private readonly Collider2D[] _colliders = new Collider2D[MaxColliders];

	protected override void OnCreate()
	{
		var queryDesc = new EntityQueryDesc
		{
			All = new[]
			{
				ComponentType.ReadOnly<Initialized>(),
				ComponentType.ReadOnly<DetectorComponent>(),
				ComponentType.ReadOnly<BoxCollider2D>(),
				ComponentType.ReadOnly<Transform>(),
			}
		};

		_entityQuery = GetEntityQuery(queryDesc);
	}

	protected override void OnUpdate()
	{
		var detectors  = _entityQuery.ToComponentDataArray<DetectorComponent>(Allocator.TempJob);
		var colliders  = _entityQuery.ToComponentArray<BoxCollider2D>();
		var transforms = _entityQuery.ToComponentArray<Transform>();

		var entities = _entityQuery.ToEntityArray(Allocator.TempJob);

		for (var i = 0; i < detectors.Length; i++)
		{
			var detector  = detectors[i];
			var transform = transforms[i];
			var collider  = colliders[i];

			var count = Physics2D.OverlapBoxNonAlloc(transform.position, collider.size, transform.rotation.eulerAngles.z, _colliders, _triggerMask);

			detector.TriggersCount = count;

			var triggerIds = new NativeArray<int>(count, Allocator.Temp);

			for (int j = 0; j < count; j++)
			{
				triggerIds[j] = _colliders[j].GetInstanceID();
			}

			int triggersHash = Utils.CombineHashCodes(triggerIds);

			if (detector.TriggersCheckSum != triggersHash)
			{
				detector.TriggersCheckSum = triggersHash;

				DynamicBuffer<Id> buffer = EntityManager.GetBuffer<Id>(entities[i]);

				buffer.Clear();

				for (var index = 0; index < triggerIds.Length; index++)
				{
					int id = triggerIds[index];
					buffer.Add(new Id {Value = id});
				}
			}

			detectors[i] = detector;

			triggerIds.Dispose();
		}

		_entityQuery.CopyFromComponentDataArray(detectors);

		entities.Dispose();
		detectors.Dispose();
	}
}