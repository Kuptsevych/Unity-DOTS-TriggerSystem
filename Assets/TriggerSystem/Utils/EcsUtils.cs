using Unity.Entities;

public static class EcsUtils
{
	public static void CopyComponent<T>(EntityManager em, Entity a, Entity b) where T : struct, IComponentData
	{
		if (!em.HasComponent<T>(b)) em.AddComponent<T>(b);

		var c = em.GetComponentData<T>(a);

		em.SetComponentData(b, c);
	}
}