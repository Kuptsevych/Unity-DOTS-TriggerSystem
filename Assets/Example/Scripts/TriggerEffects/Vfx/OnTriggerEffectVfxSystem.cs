using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TriggerSystem.Example
{
	public class OnTriggerEffectVfxSystem : ComponentSystem
	{
		private EntityQuery _entityQuery;

		protected override void OnCreate()
		{
			var queryDesc = new EntityQueryDesc
			{
				//None = new[] {ComponentType.ReadOnly<Initialized>()},
				All = new[]
				{
					ComponentType.ReadOnly<DetectorComponent>(),
					ComponentType.ReadOnly<TriggerEffectVfxComponent>(),
					ComponentType.ReadOnly<Transform>(),
					ComponentType.ReadOnly<Translation>()
				}
			};

			_entityQuery = GetEntityQuery(queryDesc);
		}

		protected override void OnUpdate()
		{
			var effects    = _entityQuery.ToComponentDataArray<TriggerEffectVfxComponent>(Allocator.TempJob);
			var entities   = _entityQuery.ToEntityArray(Allocator.TempJob);
			var transforms = _entityQuery.ToComponentArray<Transform>();

			for (var i = 0; i < effects.Length; i++)
			{
				var effect = effects[i];

				if (!effect.Used)
				{
					effect.Used = true;

					var vfxEntity = EntityManager.Instantiate(effect.VfxPrefab);

					var pos = transforms[i].position;
					EntityManager.SetComponentData(vfxEntity, new Translation
					{
						Value = new float3(pos.x, pos.y, pos.z + effect.DepthDelta)
					});

					var particleSystem = EntityManager.GetComponentObject<ParticleSystem>(vfxEntity);

					particleSystem.Play();
				}

				effects[i] = effect;
			}

			_entityQuery.CopyFromComponentDataArray(effects);

			effects.Dispose();
			entities.Dispose();
		}
	}
}