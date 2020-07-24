using Unity.Entities;
using UnityEngine;

namespace TriggerSystem.Example
{
	public class SimpleTriggerEffectSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			Entities.WithoutBurst().WithAll<DetectorComponent>().ForEach(
				(Entity e, int entityInQueryIndex, ref SimpleTriggerEffect triggerEffect, in SpriteRenderer spriteRenderer) =>
				{
					if (!triggerEffect.Used)
					{
						triggerEffect.Used   = true;
						spriteRenderer.color = triggerEffect.Color;
					}
				}).Run();
		}
	}
}