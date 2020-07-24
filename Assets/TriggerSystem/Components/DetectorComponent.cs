using Unity.Entities;
using UnityEngine;

namespace TriggerSystem
{
	[GenerateAuthoringComponent]
	public struct DetectorComponent : IComponentData
	{
		[HideInInspector] public int DetectorId;
		[HideInInspector] public int TriggersCount;
		[HideInInspector] public int TriggersHash;
	}
}