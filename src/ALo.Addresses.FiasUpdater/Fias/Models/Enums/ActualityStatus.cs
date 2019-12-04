using System;

namespace ALo.Addresses.FiasUpdater.Fias.Models.Enums
{
    [Serializable]
    public enum ActualityStatus
    {
        /// <summary>
        /// Не актуальный
        /// </summary>
        IrRelevant = 0,
        /// <summary>
        /// Актуальный
        /// </summary>
        Relevant = 1,
    }
}
