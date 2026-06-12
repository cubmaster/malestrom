#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Pawn.h"
#include "Ship/ShipStatsRow.h"
#include "ShipPawn.generated.h"

class USceneComponent;
class UStaticMeshComponent;
class UBoxComponent;
class UShipMovementComponent;

UCLASS()
class IRONEXILES_API AShipPawn : public APawn
{
	GENERATED_BODY()

public:
	AShipPawn();

	virtual void SetupPlayerInputComponent(UInputComponent* PlayerInputComponent) override;
	virtual void Tick(float DeltaSeconds) override;
	virtual UPawnMovementComponent* GetMovementComponent() const override;

	UShipMovementComponent* GetShipMovement() const { return ShipMovement; }

protected:
	virtual void BeginPlay() override;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Ship")
	TObjectPtr<USceneComponent> ShipRoot;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Ship")
	TObjectPtr<UStaticMeshComponent> HullMesh;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Ship")
	TObjectPtr<UBoxComponent> CollisionHull;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Ship")
	TObjectPtr<UShipMovementComponent> ShipMovement;

	UPROPERTY(EditDefaultsOnly, Category = "Ship|Data")
	TSoftObjectPtr<UDataTable> ShipStatsTable;

	UPROPERTY(EditDefaultsOnly, Category = "Ship|Data")
	FName ShipStatsRowName = TEXT("Human_Starter_Fighter");

private:
	FVector ThrustInput = FVector::ZeroVector;
	FRotator RotationInput = FRotator::ZeroRotator;
	bool bBrakeHeld = false;

	void LoadStatsFromDataTable();
	void PushInputToMovement();

	void MoveForward(float Value);
	void MoveRight(float Value);
	void MoveUp(float Value);
	void Turn(float Value);
	void LookUp(float Value);
	void Roll(float Value);
	void BrakePressed();
	void BrakeReleased();
};
