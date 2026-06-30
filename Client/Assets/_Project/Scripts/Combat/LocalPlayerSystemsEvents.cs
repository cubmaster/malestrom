using System;
using UnityEngine;

namespace IronExiles.Combat
{
    public static class LocalPlayerSystemsEvents
    {
        public static event Action<GameObject> LocalPlayerShipReady;
        public static event Action<Vector3> LocalPlayerHit;

        public static void NotifyLocalPlayerShipReady(GameObject ship)
        {
            LocalPlayerShipReady?.Invoke(ship);
        }

        public static void NotifyLocalPlayerHit(Vector3 attackerPosition)
        {
            LocalPlayerHit?.Invoke(attackerPosition);
        }
    }
}
