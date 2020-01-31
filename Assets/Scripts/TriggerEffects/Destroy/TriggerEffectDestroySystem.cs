using Unity.Collections;
using Unity.Entities;

public class TriggerEffectDestroySystem : ComponentSystem
{
	private EntityQuery _entityQuery;

	protected override void OnCreate()
	{
		_entityQuery = GetEntityQuery(typeof(Initialized),
			typeof(DetectorComponent), typeof(TriggerEffectDestroyComponent));
	}

	protected override void OnUpdate()
	{
		var destroys = _entityQuery.ToComponentDataArray<TriggerEffectDestroyComponent>(Allocator.TempJob);
		var entities = _entityQuery.ToEntityArray(Allocator.TempJob);

		for (var i = 0; i < destroys.Length; i++)
		{
			EntityManager.RemoveComponent<TriggerEffectDestroyComponent>(entities[i]);
			PostUpdateCommands.AddComponent<DestroyComponent>(destroys[i].EntityForDestroy);
		}

		destroys.Dispose();
		entities.Dispose();
	}
}