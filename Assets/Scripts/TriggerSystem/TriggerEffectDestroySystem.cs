using Unity.Collections;
using Unity.Entities;

public class TriggerEffectDestroySystem : ComponentSystem
{
	private EntityQuery _entityQuery;

	private EntityManager _entityManager;

	protected override void OnCreate()
	{
		_entityQuery = GetEntityQuery(typeof(Initialized),
			typeof(DetectorComponent), typeof(TriggerEffectDestroyComponent));

		_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
	}

	protected override void OnUpdate()
	{
		var destroys = _entityQuery.ToComponentDataArray<TriggerEffectDestroyComponent>(Allocator.TempJob);
		var entities = _entityQuery.ToEntityArray(Allocator.TempJob);

		for (var i = 0; i < destroys.Length; i++)
		{
			_entityManager.RemoveComponent<TriggerEffectDestroyComponent>(entities[i]);
			PostUpdateCommands.AddComponent<DestroyComponent>(destroys[i].EntityForDestroy);
		}

		destroys.Dispose();
		entities.Dispose();
	}
}