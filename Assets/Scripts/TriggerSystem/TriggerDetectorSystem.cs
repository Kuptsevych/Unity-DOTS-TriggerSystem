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

	private readonly HashSet<int> _triggersSet = new HashSet<int>();

	private readonly LayerMask _triggerMask = LayerMask.GetMask("Triggers");

	private readonly Dictionary<Entity, HashSet<int>> _detectorTriggers = new Dictionary<Entity, HashSet<int>>();

	private readonly List<int> _removeOldTriggers = new List<int>(MaxColliders);

	private readonly List<ValueTuple<ComponentType, Entity>> _cachedComponentAndTriggers = new List<ValueTuple<ComponentType, Entity>>();

	private readonly HashSet<ComponentType> _remainTriggersEffects = new HashSet<ComponentType>();

	private readonly List<ComponentType> _triggerToRemoveEffects = new List<ComponentType>();

	private readonly Dictionary<ComponentType, Action<EntityManager, Entity, Entity>> _conversion =
		new Dictionary<ComponentType, Action<EntityManager, Entity, Entity>>(MaxEntitites);

	public Dictionary<ComponentType, Action<EntityManager, Entity, Entity>> Conversion => _conversion;

	private readonly Dictionary<Entity, HashSet<ComponentType>> _entityEffects = new Dictionary<Entity, HashSet<ComponentType>>(MaxEntitites);
	
	private TriggerInitSystem _triggerInitSystem;

	protected override void OnCreate()
	{
		_entityQuery = GetEntityQuery(typeof(Initialized), typeof(DetectorComponent), typeof(Transform));
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

			var count = Physics2D.OverlapBoxNonAlloc(transform.position, detector.Size, transform.rotation.eulerAngles.z, _colliders, _triggerMask);

			detector.TriggersCount = count;

			_triggersSet.Clear();

			for (int j = 0; j < count; j++)
			{
				_triggersCache[j] = _colliders[j].GetInstanceID();
				_triggersSet.Add(_triggersCache[j]);
			}

			if (_detectorTriggers.TryGetValue(entities[i], out HashSet<int> triggerIds))
			{
				_removeOldTriggers.Clear();

				foreach (var id in triggerIds)
				{
					// in new triggers there is no old
					if (!_triggersSet.Contains(id))
					{
						_removeOldTriggers.Add(id);
						Debug.Log("Trigger exit::" + id);
						TryRemoveComponentsFromTriggers(EntityManager, entities[i], id);
					}
				}

				for (var k = 0; k < _removeOldTriggers.Count; k++)
				{
					triggerIds.Remove(_removeOldTriggers[k]);
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


			detectors[i] = detector;
		}

		_entityQuery.CopyFromComponentDataArray(detectors);

		detectors.Dispose();
		entities.Dispose();
	}

	private void TryRemoveComponentsFromTriggers(EntityManager em, Entity detector, int trigger)
	{
		_remainTriggersEffects.Clear();
		_triggerToRemoveEffects.Clear();

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
							if (_conversion.ContainsKey(components[i])) _triggerToRemoveEffects.Add(components[i]);
						}
					}
					else
					{
						for (var i = 0; i < components.Length; i++)
						{
							if (_conversion.ContainsKey(components[i])) _remainTriggersEffects.Add(components[i]);
						}
					}

					components.Dispose();
				}
			}
		}

		for (var i = 0; i < _triggerToRemoveEffects.Count; i++)
		{
			var componentType = _triggerToRemoveEffects[i];

			if (!_remainTriggersEffects.Contains(componentType))
			{
				PostUpdateCommands.RemoveComponent(detector, componentType);
				_entityEffects[detector].Remove(componentType);
			}
		}
	}

	private void TryAddComponentsFromTriggers(EntityManager em, Entity detector)
	{
		_cachedComponentAndTriggers.Clear();

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

						if (!entityEffects.Contains(componentType)) _cachedComponentAndTriggers.Add((componentType, entity));
					}

					components.Dispose();
				}
			}
		}

		for (var i = 0; i < _cachedComponentAndTriggers.Count; i++)
		{
			var componentAndTrigger = _cachedComponentAndTriggers[i];

			if (_conversion.TryGetValue(componentAndTrigger.Item1, out var value))
			{
				value.Invoke(EntityManager, componentAndTrigger.Item2, detector);
				entityEffects.Add(componentAndTrigger.Item1);
			}
		}
	}
}