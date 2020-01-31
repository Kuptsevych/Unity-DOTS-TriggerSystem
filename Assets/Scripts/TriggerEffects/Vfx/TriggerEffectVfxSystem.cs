﻿using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class TriggerEffectVfxSystem : ComponentSystem
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
				ComponentType.ReadOnly<Transform>()
			}
		};

		_entityQuery = GetEntityQuery(queryDesc);
	}

	protected override void OnUpdate()
	{
		var effects  = _entityQuery.ToComponentDataArray<TriggerEffectVfxComponent>(Allocator.TempJob);
		var entities = _entityQuery.ToEntityArray(Allocator.TempJob);

		var transforms = _entityQuery.ToComponentArray<Transform>();

		for (var i = 0; i < effects.Length; i++)
		{
			var effect = effects[i];

			if (!effect.Used)
			{
				effect.Used = true;

				//EntityManager.Instantiate(effect.VfxPrefab);
				//TODO instantiate gameobject
			}

			effects[i] = effect;
		}

		_entityQuery.CopyFromComponentDataArray(effects);

		effects.Dispose();
		entities.Dispose();
	}
}