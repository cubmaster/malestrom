#include "Ship/ShipPawn.h"

#include "Components/BoxComponent.h"
#include "Components/InputComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/DataTable.h"
#include "Engine/StaticMesh.h"
#include "GameFramework/PlayerController.h"
#include "IronExilesGameModeBase.h"
#include "Ship/ShipMovementComponent.h"
#include "UObject/ConstructorHelpers.h"

AShipPawn::AShipPawn()
{
	PrimaryActorTick.bCanEverTick = true;

	ShipRoot = CreateDefaultSubobject<USceneComponent>(TEXT("ShipRoot"));
	SetRootComponent(ShipRoot);

	ShipMovement = CreateDefaultSubobject<UShipMovementComponent>(TEXT("ShipMovement"));
	ShipMovement->SetUpdatedComponent(ShipRoot);

	HullMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("HullMesh"));
	HullMesh->SetupAttachment(ShipRoot);
	HullMesh->SetCollisionEnabled(ECollisionEnabled::NoCollision);

	static ConstructorHelpers::FObjectFinder<UStaticMesh> CubeMesh(TEXT("/Engine/BasicShapes/Cube.Cube"));
	if (CubeMesh.Succeeded())
	{
		HullMesh->SetStaticMesh(CubeMesh.Object);
		HullMesh->SetRelativeScale3D(FVector(2.f, 1.f, 0.5f));
	}

	CollisionHull = CreateDefaultSubobject<UBoxComponent>(TEXT("CollisionHull"));
	CollisionHull->SetupAttachment(ShipRoot);
	CollisionHull->SetBoxExtent(FVector(200.f, 100.f, 50.f));
	CollisionHull->SetCollisionProfileName(TEXT("Pawn"));

	ShipStatsTable = TSoftObjectPtr<UDataTable>(FSoftObjectPath(TEXT("/Game/Data/DT_ShipStats.DT_ShipStats")));
}

UPawnMovementComponent* AShipPawn::GetMovementComponent() const
{
	return ShipMovement;
}

void AShipPawn::BeginPlay()
{
	Super::BeginPlay();
	LoadStatsFromDataTable();

	if (const AIronExilesGameModeBase* GameMode = GetWorld()->GetAuthGameMode<AIronExilesGameModeBase>())
	{
		ShipMovement->SetSectorBoundsExtent(GameMode->GetSectorBoundsExtent());
	}

	if (APlayerController* PC = Cast<APlayerController>(GetController()))
	{
		PC->SetViewTarget(this);
	}
}

void AShipPawn::LoadStatsFromDataTable()
{
	FShipStatsRow Stats = FShipStatsRow::HumanStarterFighterDefaults();

	if (UDataTable* Table = ShipStatsTable.LoadSynchronous())
	{
		if (const FShipStatsRow* Row = Table->FindRow<FShipStatsRow>(ShipStatsRowName, TEXT("LoadShipStats")))
		{
			Stats = *Row;
		}
	}

	ShipMovement->SetStats(Stats);
}

void AShipPawn::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);
	check(PlayerInputComponent);

	PlayerInputComponent->BindAxis(TEXT("ThrustForward"), this, &AShipPawn::MoveForward);
	PlayerInputComponent->BindAxis(TEXT("ThrustRight"), this, &AShipPawn::MoveRight);
	PlayerInputComponent->BindAxis(TEXT("ThrustUp"), this, &AShipPawn::MoveUp);
	PlayerInputComponent->BindAxis(TEXT("Turn"), this, &AShipPawn::Turn);
	PlayerInputComponent->BindAxis(TEXT("LookUp"), this, &AShipPawn::LookUp);
	PlayerInputComponent->BindAxis(TEXT("Roll"), this, &AShipPawn::Roll);
	PlayerInputComponent->BindAction(TEXT("Brake"), IE_Pressed, this, &AShipPawn::BrakePressed);
	PlayerInputComponent->BindAction(TEXT("Brake"), IE_Released, this, &AShipPawn::BrakeReleased);
}

void AShipPawn::Tick(float DeltaSeconds)
{
	Super::Tick(DeltaSeconds);
	PushInputToMovement();
}

void AShipPawn::PushInputToMovement()
{
	if (ShipMovement)
	{
		ShipMovement->SetMovementInput(ThrustInput, RotationInput, bBrakeHeld);
	}
}

void AShipPawn::MoveForward(float Value)
{
	ThrustInput.X = Value;
}

void AShipPawn::MoveRight(float Value)
{
	ThrustInput.Y = Value;
}

void AShipPawn::MoveUp(float Value)
{
	ThrustInput.Z = Value;
}

void AShipPawn::Turn(float Value)
{
	RotationInput.Yaw = Value;
}

void AShipPawn::LookUp(float Value)
{
	RotationInput.Pitch = Value;
}

void AShipPawn::Roll(float Value)
{
	RotationInput.Roll = Value;
}

void AShipPawn::BrakePressed()
{
	bBrakeHeld = true;
}

void AShipPawn::BrakeReleased()
{
	bBrakeHeld = false;
}
