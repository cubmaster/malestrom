#pragma once

#include "CoreMinimal.h"
#include "Engine/DataTable.h"
#include "ShipStatsRow.generated.h"

USTRUCT(BlueprintType)
struct IRONEXILES_API FShipStatsRow : public FTableRowBase
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Flight")
	float MaxSpeed = 5000.f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Flight")
	float ForwardThrust = 2500.f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Flight")
	float StrafeThrust = 1800.f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Flight")
	float RotationRate = 90.f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Flight")
	float BrakeDeceleration = 1200.f;

	static FShipStatsRow HumanStarterFighterDefaults()
	{
		FShipStatsRow Row;
		Row.MaxSpeed = 5000.f;
		Row.ForwardThrust = 2500.f;
		Row.StrafeThrust = 1800.f;
		Row.RotationRate = 90.f;
		Row.BrakeDeceleration = 1200.f;
		return Row;
	}
};
