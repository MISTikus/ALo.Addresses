using System;

namespace ALo.Addresses.FiasUpdater.Fias.Models.Enums
{
    [Serializable]
    public enum AddressDiviationType
    {
        /// <summary>
        /// не определено
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// муниципальное деление
        /// </summary>
        Municial = 1,
        /// <summary>
        /// административно-территориальное деление
        /// </summary>
        Administrative = 2

    }
}
