using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Combat
{
    public struct ShipMovementInput : INetworkSerializable
    {
        public Vector3 LocalThrust;
        public Vector3 LocalRotation;
        public bool Brake;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref LocalThrust);
            serializer.SerializeValue(ref LocalRotation);
            serializer.SerializeValue(ref Brake);
        }

        public static ShipMovementInput FromAxes(Vector3 thrust, Vector3 rotation, bool brake = false)
        {
            return new ShipMovementInput
            {
                LocalThrust = new Vector3(
                    Mathf.Clamp(thrust.x, -1f, 1f),
                    Mathf.Clamp(thrust.y, -1f, 1f),
                    Mathf.Clamp(thrust.z, -1f, 1f)),
                LocalRotation = new Vector3(
                    Mathf.Clamp(rotation.x, -1f, 1f),
                    Mathf.Clamp(rotation.y, -1f, 1f),
                    Mathf.Clamp(rotation.z, -1f, 1f)),
                Brake = brake
            };
        }
    }
}
