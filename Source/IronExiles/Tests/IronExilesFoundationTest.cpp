#include "Misc/AutomationTest.h"
#include "GameMapsSettings.h"
#include "Kismet/GameplayStatics.h"
#include "IronExilesGameModeBase.h"

#if WITH_EDITOR
#include "Tests/AutomationEditorCommon.h"
#endif

namespace IronExilesFoundation
{
	static const TCHAR* EmptySectorMapPath = TEXT("/Game/Maps/Test/EmptySector");
}

IMPLEMENT_SIMPLE_AUTOMATION_TEST(
	FIronExilesFoundationDefaultMapConfigured,
	"IronExiles.Foundation.DefaultMapConfigured",
	EAutomationTestFlags::EditorContext | EAutomationTestFlags::ProductFilter)

bool FIronExilesFoundationDefaultMapConfigured::RunTest(const FString& Parameters)
{
	const UGameMapsSettings* Settings = GetDefault<UGameMapsSettings>();
	TestNotNull(TEXT("GameMapsSettings"), Settings);

	const FString GameDefaultMap = Settings->GetGameDefaultMap().ToString();
	TestEqual(TEXT("GameDefaultMap"), GameDefaultMap, FString(IronExilesFoundation::EmptySectorMapPath));

	const FString EditorStartupMap = Settings->EditorStartupMap.ToString();
	TestEqual(TEXT("EditorStartupMap"), EditorStartupMap, FString(IronExilesFoundation::EmptySectorMapPath));

	return true;
}

IMPLEMENT_SIMPLE_AUTOMATION_TEST(
	FIronExilesFoundationProjectLoads,
	"IronExiles.Foundation.ProjectLoads",
	EAutomationTestFlags::EditorContext | EAutomationTestFlags::ProductFilter)

bool FIronExilesFoundationProjectLoads::RunTest(const FString& Parameters)
{
#if WITH_EDITOR
	if (!FAutomationEditorCommon::LoadMap(IronExilesFoundation::EmptySectorMapPath))
	{
		AddError(FString::Printf(
			TEXT("Failed to load map %s. Run Scripts/Initialize-Content.ps1 to generate content."),
			IronExilesFoundation::EmptySectorMapPath));
		return false;
	}

	UWorld* World = GWorld;
	TestNotNull(TEXT("World"), World);

	AGameModeBase* GameMode = UGameplayStatics::GetGameMode(World);
	TestNotNull(TEXT("GameMode"), GameMode);
	TestTrue(TEXT("GameMode is IronExilesGameModeBase"), GameMode->IsA(AIronExilesGameModeBase::StaticClass()));

	return true;
#else
	AddWarning(TEXT("ProjectLoads requires editor context."));
	return true;
#endif
}
