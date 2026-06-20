using System;
using System.Collections.Generic;
using UnityEngine;

namespace IronExiles.Combat
{
    public readonly struct TargetCandidate : IComparable<TargetCandidate>
    {
        public readonly ulong NetworkObjectId;
        public readonly float Distance;
        public readonly float AngleDegrees;

        public TargetCandidate(ulong networkObjectId, float distance, float angleDegrees)
        {
            NetworkObjectId = networkObjectId;
            Distance = distance;
            AngleDegrees = angleDegrees;
        }

        public int CompareTo(TargetCandidate other)
        {
            var angleCompare = AngleDegrees.CompareTo(other.AngleDegrees);
            return angleCompare != 0 ? angleCompare : Distance.CompareTo(other.Distance);
        }
    }

    public readonly struct RadarContact
    {
        public readonly ulong NetworkObjectId;
        public readonly float Distance;
        public readonly TargetAffiliation Affiliation;
        public readonly Vector2 RadarPlane01;

        public RadarContact(ulong networkObjectId, float distance, TargetAffiliation affiliation, Vector2 radarPlane01)
        {
            NetworkObjectId = networkObjectId;
            Distance = distance;
            Affiliation = affiliation;
            RadarPlane01 = radarPlane01;
        }
    }

    public static class TargetSelectionMath
    {
        public static bool IsWithinLockRange(Vector3 origin, Vector3 targetPosition, float lockRangeMeters)
        {
            return (targetPosition - origin).sqrMagnitude <= lockRangeMeters * lockRangeMeters;
        }

        public static float ComputeForwardAngleDegrees(Vector3 origin, Vector3 forward, Vector3 targetPosition)
        {
            var toTarget = targetPosition - origin;
            if (toTarget.sqrMagnitude < 0.0001f)
            {
                return 0f;
            }

            var flatForward = Vector3.ProjectOnPlane(forward, Vector3.up);
            var flatToTarget = Vector3.ProjectOnPlane(toTarget, Vector3.up);
            if (flatForward.sqrMagnitude < 0.0001f || flatToTarget.sqrMagnitude < 0.0001f)
            {
                return 0f;
            }

            return Vector3.Angle(flatForward.normalized, flatToTarget.normalized);
        }

        public static Vector2 ToRadarPlane01(Vector3 origin, Vector3 forward, Vector3 targetPosition, float lockRangeMeters)
        {
            var offset = targetPosition - origin;
            var flatForward = Vector3.ProjectOnPlane(forward, Vector3.up);
            if (flatForward.sqrMagnitude < 0.0001f)
            {
                flatForward = Vector3.forward;
            }

            flatForward.Normalize();
            var flatRight = Vector3.Cross(Vector3.up, flatForward).normalized;
            var planar = Vector3.ProjectOnPlane(offset, Vector3.up);
            var x = Vector3.Dot(planar, flatRight) / lockRangeMeters;
            var y = Vector3.Dot(planar, flatForward) / lockRangeMeters;
            return new Vector2(Mathf.Clamp(x, -1f, 1f), Mathf.Clamp(y, -1f, 1f));
        }

        public static bool IsTabSelectable(TargetAffiliation affiliation)
        {
            return affiliation == TargetAffiliation.Hostile || affiliation == TargetAffiliation.Neutral;
        }

        public static List<TargetCandidate> CollectTabCandidates(
            Vector3 origin,
            Vector3 forward,
            ulong selfNetworkObjectId,
            IEnumerable<TargetableEntity> entities,
            float lockRangeMeters)
        {
            var candidates = new List<TargetCandidate>();
            foreach (var entity in entities)
            {
                if (entity == null)
                {
                    continue;
                }

                var networkObjectId = entity.GetNetworkObjectId();
                if (networkObjectId == 0UL || networkObjectId == selfNetworkObjectId)
                {
                    continue;
                }

                if (!IsTabSelectable(entity.Affiliation))
                {
                    continue;
                }

                var position = entity.transform.position;
                if (!IsWithinLockRange(origin, position, lockRangeMeters))
                {
                    continue;
                }

                candidates.Add(new TargetCandidate(
                    networkObjectId,
                    Vector3.Distance(origin, position),
                    ComputeForwardAngleDegrees(origin, forward, position)));
            }

            candidates.Sort();
            return candidates;
        }

        public static List<RadarContact> CollectRadarContacts(
            Vector3 origin,
            Vector3 forward,
            ulong selfNetworkObjectId,
            IEnumerable<TargetableEntity> entities,
            float lockRangeMeters,
            int maxContacts)
        {
            var contacts = new List<RadarContact>();
            foreach (var entity in entities)
            {
                if (entity == null)
                {
                    continue;
                }

                var networkObjectId = entity.GetNetworkObjectId();
                if (networkObjectId == 0UL || networkObjectId == selfNetworkObjectId)
                {
                    continue;
                }

                var position = entity.transform.position;
                if (!IsWithinLockRange(origin, position, lockRangeMeters))
                {
                    continue;
                }

                contacts.Add(new RadarContact(
                    networkObjectId,
                    Vector3.Distance(origin, position),
                    entity.Affiliation,
                    ToRadarPlane01(origin, forward, position, lockRangeMeters)));
            }

            contacts.Sort((a, b) => a.Distance.CompareTo(b.Distance));
            if (contacts.Count > maxContacts)
            {
                contacts.RemoveRange(maxContacts, contacts.Count - maxContacts);
            }

            return contacts;
        }

        public static int SelectNextTabIndex(IReadOnlyList<TargetCandidate> candidates, ulong currentTargetId, int direction)
        {
            if (candidates == null || candidates.Count == 0)
            {
                return -1;
            }

            if (direction == 0)
            {
                direction = 1;
            }

            var currentIndex = -1;
            for (var i = 0; i < candidates.Count; i++)
            {
                if (candidates[i].NetworkObjectId == currentTargetId)
                {
                    currentIndex = i;
                    break;
                }
            }

            if (currentIndex < 0)
            {
                return 0;
            }

            var nextIndex = (currentIndex + direction) % candidates.Count;
            if (nextIndex < 0)
            {
                nextIndex += candidates.Count;
            }

            return nextIndex;
        }

        public static bool TryResolveTargetable(
            ulong networkObjectId,
            IEnumerable<TargetableEntity> entities,
            out TargetableEntity target)
        {
            foreach (var entity in entities)
            {
                if (entity != null && entity.GetNetworkObjectId() == networkObjectId)
                {
                    target = entity;
                    return true;
                }
            }

            target = null;
            return false;
        }
    }
}
