#pragma once

#include "CoreMinimal.h"
#include "GameFramework/GameModeBase.h"
#include "IronExilesGameModeBase.generated.h"

UCLASS()
class AIronExilesGameModeBase : public AGameModeBase
{
	GENERATED_BODY()

public:
	AIronExilesGameModeBase();

	UFUNCTION(BlueprintPure, Category = "IronExiles|Sector")
	FVector GetSectorBoundsExtent() const { return SectorBoundsExtent; }

protected:
	UPROPERTY(EditDefaultsOnly, Category = "IronExiles|Sector")
	FVector SectorBoundsExtent = FVector(500000.f, 500000.f, 500000.f);
};
