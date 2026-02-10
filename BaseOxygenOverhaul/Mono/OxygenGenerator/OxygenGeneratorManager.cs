using UnityEngine;
using BaseOxygenOverhaul.Types;

namespace BaseOxygenOverhaul.Mono.OxygenGenerator
{
    /// <summary>
    /// Manages the oxygen generator behavior.
    /// </summary>
    public class OxygenGeneratorManager : MonoBehaviour
    {
        public OxygenGeneratorSize Size { get; internal set; }

        public Constructable Constructable { get; private set; }
        public Base ParentBase { get; private set; }
        public BaseFloodSim ParentBaseFloodSim { get; private set; }
        public BasePowerRelay ParentBasePowerRelay { get; private set; }

        private void Start()
        {
            Constructable = GetComponent<Constructable>();
            ParentBase = GetComponentInParent<Base>();
            ParentBaseFloodSim = GetComponentInParent<BaseFloodSim>();
            ParentBasePowerRelay = GetComponentInParent<BasePowerRelay>();
        }

        public OxygenGeneratorState GetState()
        {
            if (Constructable == null || ParentBase == null || ParentBaseFloodSim == null || ParentBasePowerRelay == null)
                return OxygenGeneratorState.None;
            else if (!Constructable.constructed)
                return OxygenGeneratorState.None;
            else if (!ParentBasePowerRelay.IsPowered())
                return OxygenGeneratorState.Offline;
            else if (ParentBaseFloodSim.IsUnderwater(transform.position))
                return OxygenGeneratorState.Flooded;
            else // constructed, has power, not flooded
                return OxygenGeneratorState.Online;
        }
    }
}