using UnityEngine;
using BaseOxygenOverhaul.Types;

namespace BaseOxygenOverhaul.Mono.OxygenGenerator
{
    /// <summary>
    /// Manages the oxygen generator behavior.
    /// </summary>
    public class OxygenGeneratorManager : MonoBehaviour
    {
        public OxygenGeneratorSize type;

        private OxygenGeneratorAudioVisual audioVisual;

        private Base _parentBase;
        public Base ParentBase
        {
            get
            {
                if (_parentBase == null) _parentBase = GetComponentInParent<Base>();
                return _parentBase;
            }
        }

        private void Start()
        {
            audioVisual = GetComponent<OxygenGeneratorAudioVisual>();
        }
    }
}