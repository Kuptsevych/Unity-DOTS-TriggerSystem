using Unity.Entities;
using UnityEngine;

namespace TriggerSystem.Example
{
	public class SimpleTriggerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		public Color Color;

		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			conversionSystem.AddHybridComponent(GetComponent<SpriteRenderer>());
			conversionSystem.AddHybridComponent(GetComponent<Transform>());
			conversionSystem.AddHybridComponent(GetComponent<Rigidbody2D>());
			conversionSystem.AddHybridComponent(GetComponent<BoxCollider2D>());

			dstManager.AddComponent<TriggerComponent>(entity);
			dstManager.AddComponent<Enabled>(entity);
			dstManager.AddComponent<SimpleTriggerEffect>(entity);

			dstManager.SetComponentData(entity, new SimpleTriggerEffect
			{
				Color = Color
			});
		}
	}
}