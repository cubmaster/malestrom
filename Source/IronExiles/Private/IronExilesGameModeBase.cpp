#include "IronExilesGameModeBase.h"
#include "Ship/ShipPawn.h"

AIronExilesGameModeBase::AIronExilesGameModeBase()
{
	DefaultPawnClass = AShipPawn::StaticClass();
}
