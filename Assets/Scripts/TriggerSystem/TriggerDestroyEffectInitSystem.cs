using Unity.Collections;
using Unity.Entities;

[UpdateBefore(typeof(TriggerInitSystem))]
public class TriggerDestroyEffectInitSystem : ComponentSystem
{
	private EntityQuery _entityQuery;

	protected override void OnCreate()
	{
		var queryDesc = new EntityQueryDesc
		{
			None = new[] {ComponentType.ReadOnly<Initialized>()},
			All = new[]
			{
				ComponentType.ReadOnly<TriggerComponent>(), ComponentType.ReadOnly<TriggerEffectDestroyComponent>()
			}
		};

		_entityQuery = GetEntityQuery(queryDesc);
	}

	protected override void OnUpdate()
	{
		var destroys = _entityQuery.ToComponentDataArray<TriggerEffectDestroyComponent>(Allocator.TempJob);
		var entities = _entityQuery.ToEntityArray(Allocator.TempJob);

		for (var i = 0; i < entities.Length; i++)
		{
			var destroy = destroys[i];

			destroy.EntityForDestroy = entities[i];

			destroys[i] = destroy;
		}

		_entityQuery.CopyFromComponentDataArray(destroys);

		destroys.Dispose();
		entities.Dispose();
	}
}