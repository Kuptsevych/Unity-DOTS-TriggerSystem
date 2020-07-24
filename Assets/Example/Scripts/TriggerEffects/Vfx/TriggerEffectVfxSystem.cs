using Unity.Collections;
using Unity.Entities;

namespace TriggerSystem.Example
{
	public class TriggerEffectVfxSystem : ComponentSystem
	{
		private EntityQuery _entityQuery;

		protected override void OnCreate()
		{
			var queryDesc = new EntityQueryDesc
			{
				All = new[]
				{
					ComponentType.ReadOnly<SimpleSpark>()
				}
			};

			_entityQuery = GetEntityQuery(queryDesc);
		}

		protected override void OnUpdate()
		{
			var sparks   = _entityQuery.ToComponentDataArray<SimpleSpark>(Allocator.TempJob);
			var entities = _entityQuery.ToEntityArray(Allocator.TempJob);

			var dt = Time.DeltaTime;

			for (var i = 0; i < sparks.Length; i++)
			{
				var spark = sparks[i];

				spark.Timer += dt;

				if (spark.Timer > spark.DestroyDelay)
				{
					PostUpdateCommands.DestroyEntity(entities[i]);
				}

				sparks[i] = spark;
			}

			_entityQuery.CopyFromComponentDataArray(sparks);

			sparks.Dispose();
			entities.Dispose();
		}
	}
}