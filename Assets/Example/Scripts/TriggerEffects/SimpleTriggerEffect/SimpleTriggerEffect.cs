using Unity.Entities;
using UnityEngine;

namespace TriggerSystem.Example
{
	public struct SimpleTriggerEffect : IComponentData
	{
		public Color Color;
		public bool  Used;
	}
}