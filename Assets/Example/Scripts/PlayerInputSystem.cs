using Unity.Entities;
using UnityEngine;

namespace TriggerSystem.Example
{
	public class PlayerInputSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			Entities.WithAll<Enabled, Initialized>().ForEach((Entity e, ref InputComponent input) =>
			{
				input.PrepareAttack = Input.GetKeyDown(KeyCode.Z);
				input.Attack        = Input.GetKeyUp(KeyCode.Z);
				input.Axes.x        = Input.GetAxis("Horizontal");
				input.Axes.y        = Input.GetAxis("Vertical");
			}).Run();
		}
	}
}