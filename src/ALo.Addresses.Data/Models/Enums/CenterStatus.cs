namespace ALo.Addresses.Data.Models.Enums
{
    public enum CenterStatus
    {
        /// <summary>
        /// Объект не является центром административно-территориального образования
        /// </summary>
        Territory = 0,
        /// <summary>
        /// Объект является центром района
        /// </summary>
        District = 1,
        /// <summary>
        /// Объект является центром (столицей) региона
        /// </summary>
        Region = 2,
        /// <summary>
        /// Объект является одновременно и центром района и центром региона
        /// </summary>
        RegionNDistrict = 3,
        /// <summary>
        /// Центральный район, т.е. район, в котором находится центр региона (только для объектов 2-го уровня
        /// </summary>
        CentralDistrict = 4,
    }
}
