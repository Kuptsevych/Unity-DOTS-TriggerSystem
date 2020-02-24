using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public class TriggerDetectorSystem : ComponentSystem
{
	private const int MaxEntitites = 8;
	private const int MaxColliders = 8;

	private EntityQuery _entityQuery;

	private readonly Collider2D[] _colliders = new Collider2D[MaxColliders];

	private readonly int[] _triggersCache = new int[MaxColliders];
	
	private readonly LayerMask _triggerMask = LayerMask.GetMask("Triggers");

	private readonly Dictionary<Entity, HashSet<int>> _detectorTriggers = new Dictionary<Entity, HashSet<int>>();

	private readonly Dictionary<ComponentType, Action<EntityManager, Entity, Entity>> _conversion =
		new Dictionary<ComponentType, Action<EntityManager, Entity, Entity>>(MaxEntitites);

	public Dictionary<ComponentType, Action<EntityManager, Entity, Entity>> Conversion => _conversion;

	private readonly Dictionary<Entity, HashSet<ComponentType>> _entityEffects = new Dictionary<Entity, HashSet<ComponentType>>(MaxEntitites);

	private TriggerInitSystem _triggerInitSystem;
	
	protected override void OnCreate()
	{
		_entityQuery = GetEntityQuery(typeof(Initialized), typeof(DetectorComponent), typeof(Transform));
	}

	protected override void OnDestroy()
	{
		
	}

	protected override void OnStartRunning()
	{
		_triggerInitSystem = EntityManager.World.GetExistingSystem<TriggerInitSystem>();
	}

	protected override void OnUpdate()
	{
		var detectors  = _entityQuery.ToComponentDataArray<DetectorComponent>(Allocator.TempJob);
		var transforms = _entityQuery.ToComponentArray<Transform>();
		var entities   = _entityQuery.ToEntityArray(Allocator.TempJob);

		for (var i = 0; i < detectors.Length; i++)
		{
			var detector  = detectors[i];
			var transform = transforms[i];

			//TODO physics move to separate system
			var count = Physics2D.OverlapBoxNonAlloc(transform.position, detector.Size, transform.rotation.eulerAngles.z, _colliders, _triggerMask);

			detector.TriggersCount = count;

			var triggersSet = new NativeHashMap<int, bool>(MaxColliders, Allocator.Temp);
			
			for (int j = 0; j < count; j++)
			{
				_triggersCache[j] = _colliders[j].GetInstanceID();
				triggersSet.Add(_triggersCache[j], false);
			}

			if (_detectorTriggers.TryGetValue(entities[i], out HashSet<int> triggerIds))
			{
				int removeOldTriggersCount = 0;
				var removeOldTriggers      = new NativeArray<int>(MaxColliders, Allocator.Temp);

				foreach (var id in triggerIds)
				{
					// in new triggers there is no old
					if (!triggersSet.ContainsKey(id))
					{
						removeOldTriggers[removeOldTriggersCount++] = id;
						Debug.Log("Trigger exit::" + id);
						TryRemoveComponentsFromTriggers(EntityManager, entities[i], id);
					}
				}

				for (var k = 0; k < removeOldTriggersCount; k++)
				{
					triggerIds.Remove(removeOldTriggers[k]);
				}

				for (int j = 0; j < count; j++)
				{
					var triggerId = _triggersCache[j];

					if (!triggerIds.Contains(triggerId))
					{
						triggerIds.Add(triggerId);
						Debug.Log("Trigger enter::" + triggerId);
						TryAddComponentsFromTriggers(EntityManager, entities[i]);
					}
				}

				removeOldTriggers.Dispose();
			}
			else
			{
				for (int j = 0; j < count; j++)
				{
					var triggerId = _triggersCache[j];

					_detectorTriggers.Add(entities[i], new HashSet<int> {triggerId});
					Debug.Log("Trigger enter::" + triggerId);
					TryAddComponentsFromTriggers(EntityManager, entities[i]);
				}
			}

			triggersSet.Dispose();

			detectors[i] = detector;
		}

		_entityQuery.CopyFromComponentDataArray(detectors);

		detectors.Dispose();
		entities.Dispose();
	}

	private void TryRemoveComponentsFromTriggers(EntityManager em, Entity detector, int trigger)
	{
		var remainTriggersEffects  = new NativeHashMap<ComponentType, bool>();
		var triggerToRemoveEffects = new NativeList<ComponentType>(Allocator.Temp);

		if (_detectorTriggers.TryGetValue(detector, out var triggerIds))
		{
			foreach (var triggerId in triggerIds)
			{
				if (_triggerInitSystem.ColliderToTriggerEntity.TryGetValue(triggerId, out var entity))
				{
					var components = em.GetComponentTypes(entity);

					if (trigger == triggerId)
					{
						for (var i = 0; i < components.Length; i++)
						{
							if (_conversion.ContainsKey(components[i])) triggerToRemoveEffects.Add(components[i]);
						}
					}
					else
					{
						for (var i = 0; i < components.Length; i++)
						{
							if (_conversion.ContainsKey(components[i])) remainTriggersEffects.Add(components[i], false);
						}
					}

					components.Dispose();
				}
			}
		}

		for (var i = 0; i < triggerToRemoveEffects.Length; i++)
		{
			var componentType = triggerToRemoveEffects[i];

			if (!remainTriggersEffects.IsCreated || !remainTriggersEffects.ContainsKey(componentType))
			{
				PostUpdateCommands.RemoveComponent(detector, componentType);
				_entityEffects[detector].Remove(componentType);
			}
		}

		if (remainTriggersEffects.IsCreated) remainTriggersEffects.Dispose();
		triggerToRemoveEffects.Dispose();
	}

	private void TryAddComponentsFromTriggers(EntityManager em, Entity detector)
	{
		var cachedComponentAndTriggers = new NativeList<ValueTuple<ComponentType, Entity>>(Allocator.Temp);

		if (!_entityEffects.TryGetValue(detector, out var entityEffects))
		{
			entityEffects = new HashSet<ComponentType>();
			_entityEffects.Add(detector, entityEffects);
		}

		if (_detectorTriggers.TryGetValue(detector, out var triggerIds))
		{
			foreach (var triggerId in triggerIds)
			{
				if (_triggerInitSystem.ColliderToTriggerEntity.TryGetValue(triggerId, out var entity))
				{
					var components = em.GetComponentTypes(entity);

					for (var i = 0; i < components.Length; i++)
					{
						var componentType = components[i];

						if (!entityEffects.Contains(componentType)) cachedComponentAndTriggers.Add((componentType, entity));
					}

					components.Dispose();
				}
			}
		}

		for (var i = 0; i < cachedComponentAndTriggers.Length; i++)
		{
			var componentAndTrigger = cachedComponentAndTriggers[i];

			if (_conversion.TryGetValue(componentAndTrigger.Item1, out var value))
			{
				value.Invoke(EntityManager, componentAndTrigger.Item2, detector);
				entityEffects.Add(componentAndTrigger.Item1);
			}
		}

		cachedComponentAndTriggers.Dispose();
	}
}