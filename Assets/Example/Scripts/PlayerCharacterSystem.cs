using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TriggerSystem.Example
{
	public class PlayerCharacterSystem : ComponentSystem
	{
		private EntityQuery _entityQuery;

		private static readonly int Walk       = Animator.StringToHash("walk");
		private static readonly int Horizontal = Animator.StringToHash("horizontal");
		private static readonly int Vertical   = Animator.StringToHash("vertical");

		protected override void OnCreate()
		{
			_entityQuery = GetEntityQuery(
				typeof(Animator),
				typeof(PlayerComponent),
				typeof(InputComponent),
				typeof(Transform)
			);
		}

		protected override void OnUpdate()
		{
			var inputs     = _entityQuery.ToComponentDataArray<InputComponent>(Allocator.TempJob);
			var players    = _entityQuery.ToComponentDataArray<PlayerComponent>(Allocator.TempJob);
			var transforms = _entityQuery.ToComponentArray<Transform>();

			var animators = _entityQuery.ToComponentArray<Animator>();

			for (var i = 0; i < inputs.Length; i++)
			{
				var animator  = animators[i];
				var input     = inputs[i];
				var player    = players[i];
				var transform = transforms[i];

				bool walk = math.abs(input.Axes.x) + math.abs(input.Axes.y) > 0.01f;

				animator.SetBool(Walk, walk);

				animator.SetFloat(Horizontal, input.Axes.x);
				animator.SetFloat(Vertical,   input.Axes.y);

				if (walk)
				{
					var step = input.Axes * Time.DeltaTime * player.Speed;
					transform.Translate(new float3(step.x, step.y, 0));
				}
			}

			inputs.Dispose();
			players.Dispose();
		}
	}
}
