using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace TriggerSystem.Example
{
	public class SimpleDetectorAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			conversionSystem.AddHybridComponent(GetComponent<SpriteRenderer>());
			conversionSystem.AddHybridComponent(GetComponent<Transform>());
			conversionSystem.AddHybridComponent(GetComponent<Rigidbody2D>());
			conversionSystem.AddHybridComponent(GetComponent<BoxCollider2D>());

			dstManager.AddComponent<DetectorComponent>(entity);
			dstManager.AddComponent<InputComponent>(entity);
			dstManager.AddComponent<SimpleDetectorTag>(entity);
			dstManager.AddComponent<Enabled>(entity);
			dstManager.AddComponent<CopyTransformFromGameObject>(entity);
		}
	}
}
