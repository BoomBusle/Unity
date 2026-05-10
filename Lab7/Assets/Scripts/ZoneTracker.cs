using System.Collections.Generic;
using UnityEngine;

namespace Lab7
{
    public class ZoneTracker : MonoBehaviour
    {
        readonly List<Zone> zones = new List<Zone>();

        public Zone Current => zones.Count == 0 ? null : zones[zones.Count - 1];

        public bool IsIn(ZoneType t)
        {
            for (int i = 0; i < zones.Count; i++)
                if (zones[i] != null && zones[i].type == t) return true;
            return false;
        }

        void OnTriggerEnter(Collider other)
        {
            var z = other.GetComponent<Zone>();
            if (z != null && !zones.Contains(z))
                zones.Add(z);
        }

        void OnTriggerExit(Collider other)
        {
            var z = other.GetComponent<Zone>();
            if (z != null) zones.Remove(z);
        }
    }
}
