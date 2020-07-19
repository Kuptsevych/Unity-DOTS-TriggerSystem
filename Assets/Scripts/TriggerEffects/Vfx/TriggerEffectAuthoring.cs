using Unity.Entities;
using UnityEngine;

public class TriggerEffectAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	public GameObject VfxPrefab;

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		using (var blobAssetStore = new BlobAssetStore())
		{
			var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);

			var vfxEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(VfxPrefab, settings);

			dstManager.AddComponentObject(vfxEntity, VfxPrefab.GetComponent<ParticleSystem>());
			dstManager.AddComponentObject(vfxEntity, VfxPrefab.GetComponent<SpriteRenderer>());

			dstManager.AddComponentData(entity, new TriggerEffectVfxComponent
			{
				VfxPrefab = vfxEntity
			});

			blobAssetStore.Dispose();
		}
	}
}

public class VfxPrefabConversionSystem : GameObjectConversionSystem
{
	protected override void OnUpdate()
	{
		Entities.ForEach((SpriteRenderer spriteRenderer, ParticleSystem particleSystem) =>
		{
			AddHybridComponent(spriteRenderer);
			AddHybridComponent(particleSystem);
			AddHybridComponent(spriteRenderer.gameObject.GetComponent<ParticleSystemRenderer>());
		});
	}
}