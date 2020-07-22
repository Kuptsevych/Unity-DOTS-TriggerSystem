using Unity.Entities;

public class AddTriggerEffectsSystem : ComponentSystem
{
	private TriggerDetectorSystem _triggerDetectorSystem;

	protected override void OnStartRunning()
	{		
		_triggerDetectorSystem = EntityManager.World.GetExistingSystem<TriggerDetectorSystem>();
		
		//Add effect components
		_triggerDetectorSystem.Conversion.Add(typeof(TriggerEffectVfxComponent), EcsUtils.CopyComponent<TriggerEffectVfxComponent>);
		_triggerDetectorSystem.Conversion.Add(typeof(TriggerEffectDestroyComponent), EcsUtils.CopyComponent<TriggerEffectDestroyComponent>);
		_triggerDetectorSystem.Conversion.Add(typeof(TriggerCoin), EcsUtils.CopyComponent<TriggerCoin>);
	}

	protected override void OnUpdate()
	{
	}
}