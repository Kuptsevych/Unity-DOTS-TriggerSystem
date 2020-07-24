using Unity.Entities;
using UnityEngine;

namespace TriggerSystem.Example
{
	public class TriggerCoinAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		public int        Count;

		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			dstManager.AddComponent<TriggerCoin>(entity);
			dstManager.SetComponentData(entity, new TriggerCoin
			{
				Count = this.Count
			});

			conversionSystem.AddHybridComponent(GetComponent<Transform>());
			conversionSystem.AddHybridComponent(GetComponent<Rigidbody2D>());
			conversionSystem.AddHybridComponent(GetComponent<BoxCollider2D>());
			conversionSystem.AddHybridComponent(GetComponent<SpriteRenderer>());
			conversionSystem.AddHybridComponent(GetComponent<Animator>());
		}
	}
}