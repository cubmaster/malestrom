using System;
using UnityEngine;

namespace IronExiles.Combat
{
    public static class LocalPlayerSystemsEvents
    {
        public static event Action<GameObject> LocalPlayerShipReady;

        public static void NotifyLocalPlayerShipReady(GameObject ship)
        {
            LocalPlayerShipReady?.Invoke(ship);
        }
    }
}
