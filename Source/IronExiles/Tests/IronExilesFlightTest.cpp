#include "Misc/AutomationTest.h"
#include "Ship/ShipPawn.h"
#include "Ship/ShipMovementComponent.h"

#if WITH_EDITOR
#include "Tests/AutomationEditorCommon.h"
#endif

IMPLEMENT_SIMPLE_AUTOMATION_TEST(
	FIronExilesFlightSpeedClamp,
	"IronExiles.Flight.SpeedClamp",
	EAutomationTestFlags::EditorContext | EAutomationTestFlags::ProductFilter)

bool FIronExilesFlightSpeedClamp::RunTest(const FString& Parameters)
{
#if WITH_EDITOR
	UWorld* World = FAutomationEditorCommon::CreateBlankMap();
	TestNotNull(TEXT("World"), World);

	FActorSpawnParameters SpawnParams;
	SpawnParams.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;
	AShipPawn* Ship = World->SpawnActor<AShipPawn>(AShipPawn::StaticClass(), FVector::ZeroVector, FRotator::ZeroRotator, SpawnParams);
	TestNotNull(TEXT("Ship"), Ship);

	UShipMovementComponent* Movement = Ship->GetShipMovement();
	TestNotNull(TEXT("Movement"), Movement);

	FShipStatsRow Stats;
	Stats.MaxSpeed = 1000.f;
	Stats.ForwardThrust = 50000.f;
	Stats.StrafeThrust = 50000.f;
	Stats.BrakeDeceleration = 0.f;
	Movement->SetStats(Stats);
	Movement->SetSectorBoundsExtent(FVector(1000000.f));
	Movement->SetMovementInput(FVector(1.f, 0.f, 0.f), FRotator::ZeroRotator, false);

	for (int32 Tick = 0; Tick < 600; ++Tick)
	{
		Movement->TickComponent(0.016f, LEVELTICK_All, nullptr);
	}

	const float Speed = Movement->GetCurrentVelocity().Size();
	TestTrue(FString::Printf(TEXT("Speed %.1f should be <= MaxSpeed %.1f"), Speed, Stats.MaxSpeed), Speed <= Stats.MaxSpeed + 1.f);
	return true;
#else
	AddWarning(TEXT("Flight tests require editor context."));
	return true;
#endif
}

IMPLEMENT_SIMPLE_AUTOMATION_TEST(
	FIronExilesFlightSectorBounds,
	"IronExiles.Flight.SectorBounds",
	EAutomationTestFlags::EditorContext | EAutomationTestFlags::ProductFilter)

bool FIronExilesFlightSectorBounds::RunTest(const FString& Parameters)
{
#if WITH_EDITOR
	UWorld* World = FAutomationEditorCommon::CreateBlankMap();
	TestNotNull(TEXT("World"), World);

	FActorSpawnParameters SpawnParams;
	SpawnParams.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;
	AShipPawn* Ship = World->SpawnActor<AShipPawn>(AShipPawn::StaticClass(), FVector::ZeroVector, FRotator::ZeroRotator, SpawnParams);
	TestNotNull(TEXT("Ship"), Ship);

	UShipMovementComponent* Movement = Ship->GetShipMovement();
	TestNotNull(TEXT("Movement"), Movement);

	const FVector Extent(500.f, 500.f, 500.f);
	Movement->SetSectorBoundsExtent(Extent);
	Movement->SetStats(FShipStatsRow::HumanStarterFighterDefaults());
	Movement->SetMovementInput(FVector(1.f, 0.f, 0.f), FRotator::ZeroRotator, false);

	for (int32 Tick = 0; Tick < 2000; ++Tick)
	{
		Movement->TickComponent(0.016f, LEVELTICK_All, nullptr);
	}

	const FVector Location = Ship->GetActorLocation();
	TestTrue(TEXT("Within X bounds"), FMath::Abs(Location.X) <= Extent.X + 1.f);
	TestTrue(TEXT("Within Y bounds"), FMath::Abs(Location.Y) <= Extent.Y + 1.f);
	TestTrue(TEXT("Within Z bounds"), FMath::Abs(Location.Z) <= Extent.Z + 1.f);
	return true;
#else
	AddWarning(TEXT("Flight tests require editor context."));
	return true;
#endif
}
