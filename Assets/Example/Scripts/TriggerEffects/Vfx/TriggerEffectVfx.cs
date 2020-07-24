using Unity.Entities;

namespace TriggerSystem.Example
{
	public struct TriggerEffectVfxComponent : IComponentData
	{
		public Entity VfxPrefab;
		public float  DepthDelta;
		public bool   Used;
	}
}