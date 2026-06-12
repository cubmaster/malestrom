#include "Ship/ShipMovementComponent.h"

UShipMovementComponent::UShipMovementComponent()
{
	PrimaryComponentTick.bCanEverTick = true;
	Stats = FShipStatsRow::HumanStarterFighterDefaults();
}

void UShipMovementComponent::BeginPlay()
{
	Super::BeginPlay();
	Stats = FShipStatsRow::HumanStarterFighterDefaults();
}

void UShipMovementComponent::SetMovementInput(
	const FVector& LocalThrustInput,
	const FRotator& LocalRotationInput,
	bool bBrake)
{
	ThrustInput = LocalThrustInput;
	ThrustInput.X = FMath::Clamp(ThrustInput.X, -1.f, 1.f);
	ThrustInput.Y = FMath::Clamp(ThrustInput.Y, -1.f, 1.f);
	ThrustInput.Z = FMath::Clamp(ThrustInput.Z, -1.f, 1.f);

	RotationInput = LocalRotationInput;
	RotationInput.Pitch = FMath::Clamp(RotationInput.Pitch, -1.f, 1.f);
	RotationInput.Yaw = FMath::Clamp(RotationInput.Yaw, -1.f, 1.f);
	RotationInput.Roll = FMath::Clamp(RotationInput.Roll, -1.f, 1.f);

	bBrakeInput = bBrake;
}

void UShipMovementComponent::SetStats(const FShipStatsRow& InStats)
{
	Stats = InStats;
}

void UShipMovementComponent::SetSectorBoundsExtent(const FVector& InExtent)
{
	SectorBoundsExtent = InExtent.GetAbs();
}

void UShipMovementComponent::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	if (!ShouldSkipUpdate(DeltaTime))
	{
		PerformMovement(DeltaTime);
		ApplyRotation(DeltaTime);
		ClampSpeed();
		ClampToSectorBounds();
	}

	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);
}

void UShipMovementComponent::PerformMovement(float DeltaTime)
{
	if (!UpdatedComponent || DeltaTime <= KINDA_SMALL_NUMBER)
	{
		return;
	}

	const FQuat ActorRotation = UpdatedComponent->GetComponentQuat();
	const FVector Forward = ActorRotation.GetForwardVector();
	const FVector Right = ActorRotation.GetRightVector();
	const FVector Up = ActorRotation.GetUpVector();

	FVector Acceleration = FVector::ZeroVector;
	Acceleration += Forward * (ThrustInput.X * Stats.ForwardThrust);
	Acceleration += Right * (ThrustInput.Y * Stats.StrafeThrust);
	Acceleration += Up * (ThrustInput.Z * Stats.StrafeThrust);

	Velocity += Acceleration * DeltaTime;

	if (bBrakeInput && !Velocity.IsNearlyZero())
	{
		const float Speed = Velocity.Size();
		const float BrakeDelta = Stats.BrakeDeceleration * DeltaTime;
		if (BrakeDelta >= Speed)
		{
			Velocity = FVector::ZeroVector;
		}
		else
		{
			Velocity -= Velocity.GetSafeNormal() * BrakeDelta;
		}
	}

	const FVector Delta = Velocity * DeltaTime;
	FHitResult Hit;
	SafeMoveUpdatedComponent(Delta, UpdatedComponent->GetComponentQuat(), true, Hit);

	if (Hit.IsValidBlockingHit())
	{
		Velocity = FVector::VectorPlaneProject(Velocity, Hit.Normal);
	}
}

void UShipMovementComponent::ApplyRotation(float DeltaTime)
{
	if (!UpdatedComponent || DeltaTime <= KINDA_SMALL_NUMBER)
	{
		return;
	}

	const float Rate = Stats.RotationRate * DeltaTime;
	const FRotator DeltaRot(
		RotationInput.Pitch * Rate,
		RotationInput.Yaw * Rate,
		RotationInput.Roll * Rate);

	const FQuat NewQuat = UpdatedComponent->GetComponentQuat() * DeltaRot.Quaternion();
	UpdatedComponent->SetWorldRotation(NewQuat);
}

void UShipMovementComponent::ClampSpeed()
{
	const float SpeedSq = Velocity.SizeSquared();
	const float MaxSpeedSq = FMath::Square(Stats.MaxSpeed);
	if (SpeedSq > MaxSpeedSq && SpeedSq > KINDA_SMALL_NUMBER)
	{
		Velocity = Velocity.GetSafeNormal() * Stats.MaxSpeed;
	}
}

void UShipMovementComponent::ClampToSectorBounds()
{
	if (!UpdatedComponent || SectorBoundsExtent.IsNearlyZero())
	{
		return;
	}

	FVector Location = UpdatedComponent->GetComponentLocation();
	const FVector Clamped(
		FMath::Clamp(Location.X, -SectorBoundsExtent.X, SectorBoundsExtent.X),
		FMath::Clamp(Location.Y, -SectorBoundsExtent.Y, SectorBoundsExtent.Y),
		FMath::Clamp(Location.Z, -SectorBoundsExtent.Z, SectorBoundsExtent.Z));

	if (!Location.Equals(Clamped))
	{
		UpdatedComponent->SetWorldLocation(Clamped);
		const FVector NormalizedVelocity = Velocity.GetSafeNormal();
		Velocity = FVector(
			Location.X != Clamped.X ? 0.f : Velocity.X,
			Location.Y != Clamped.Y ? 0.f : Velocity.Y,
			Location.Z != Clamped.Z ? 0.f : Velocity.Z);

		if (Velocity.IsNearlyZero() && !NormalizedVelocity.IsNearlyZero())
		{
			Velocity = FVector::ZeroVector;
		}
	}
}
