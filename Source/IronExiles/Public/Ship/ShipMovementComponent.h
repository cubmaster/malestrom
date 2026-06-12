#pragma once

#include "CoreMinimal.h"
#include "GameFramework/PawnMovementComponent.h"
#include "Ship/ShipStatsRow.h"
#include "ShipMovementComponent.generated.h"

UCLASS(ClassGroup = (Custom), meta = (BlueprintSpawnableComponent))
class IRONEXILES_API UShipMovementComponent : public UPawnMovementComponent
{
	GENERATED_BODY()

public:
	UShipMovementComponent();

	void SetMovementInput(const FVector& LocalThrustInput, const FRotator& LocalRotationInput, bool bBrake);
	void SetStats(const FShipStatsRow& InStats);
	void SetSectorBoundsExtent(const FVector& InExtent);

	const FShipStatsRow& GetStats() const { return Stats; }
	FVector GetCurrentVelocity() const { return Velocity; }

protected:
	virtual void BeginPlay() override;
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

private:
	FShipStatsRow Stats;
	FVector Velocity = FVector::ZeroVector;
	FVector ThrustInput = FVector::ZeroVector;
	FRotator RotationInput = FRotator::ZeroRotator;
	bool bBrakeInput = false;
	FVector SectorBoundsExtent = FVector(500000.f, 500000.f, 500000.f);

	void PerformMovement(float DeltaTime);
	void ApplyRotation(float DeltaTime);
	void ClampSpeed();
	void ClampToSectorBounds();
};
