using UnrealBuildTool;

public class IronExiles : ModuleRules
{
	public IronExiles(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

		PublicDependencyModuleNames.AddRange(new string[]
		{
			"Core",
			"CoreUObject",
			"Engine",
			"InputCore"
		});

		if (Target.bBuildDeveloperTools || Target.Configuration != UnrealTargetConfiguration.Shipping)
		{
			PrivateDependencyModuleNames.AddRange(new string[]
			{
				"AutomationTest",
				"UnrealEd"
			});
		}
	}
}
