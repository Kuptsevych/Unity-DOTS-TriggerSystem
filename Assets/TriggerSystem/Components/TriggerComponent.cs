using Unity.Entities;
using UnityEngine;

namespace TriggerSystem
{
	[GenerateAuthoringComponent]
	public struct TriggerComponent : IComponentData
	{
		[HideInInspector] public int TriggerId;
	}
}