using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TriggerSystem.Example
{
	public class SimpleDetectorMovementSystem : ComponentSystem
	{
		private EntityQuery _entityQuery;

		protected override void OnCreate()
		{
			_entityQuery = GetEntityQuery(
				typeof(SimpleDetectorTag),
				typeof(DetectorComponent),
				typeof(InputComponent),
				typeof(Transform)
			);
		}

		protected override void OnUpdate()
		{
			var inputs     = _entityQuery.ToComponentDataArray<InputComponent>(Allocator.TempJob);
			var transforms = _entityQuery.ToComponentArray<Transform>();

			for (var i = 0; i < inputs.Length; i++)
			{
				var input     = inputs[i];
				var transform = transforms[i];


				var step = input.Axes * Time.DeltaTime * 3f;
				transform.Translate(new float3(step.x, step.y, 0));
			}

			inputs.Dispose();
		}
	}
}
