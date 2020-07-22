using Unity.Entities;
using UnityEngine;

public class TriggerEffectAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	public GameObject VfxPrefab;
	public float      DepthDelta;

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		using (var blobAssetStore = new BlobAssetStore())
		{
			var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);

			var vfxEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(VfxPrefab, settings);

			dstManager.AddComponentObject(vfxEntity, VfxPrefab.GetComponent<ParticleSystem>());

			dstManager.AddComponentData(entity, new TriggerEffectVfxComponent
			{
				VfxPrefab  = vfxEntity,
				DepthDelta = DepthDelta
			});
		}
	}
}

public class VfxPrefabConversionSystem : GameObjectConversionSystem
{
	protected override void OnUpdate()
	{
		Entities.ForEach((ParticleSystem particleSystem) =>
		{
			AddHybridComponent(particleSystem);
			AddHybridComponent(particleSystem.gameObject.GetComponent<ParticleSystemRenderer>());
			AddHybridComponent(particleSystem.transform);
		});
	}
}