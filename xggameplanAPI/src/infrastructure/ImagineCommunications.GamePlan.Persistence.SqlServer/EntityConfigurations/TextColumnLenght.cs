using System;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    /// <summary>
    /// Wellknow maximal column sizes to use  in EntityConfigurations as maxLenght value
    /// </summary>
    public static class TextColumnLenght
    {
        /// <summary>
        /// Use as maxLenght value in tiny text column = 64
        /// </summary>
        public static int Tiny => 64;

        /// <summary>
        /// Use as maxLenght value in small text column (f.e shortName)= 64
        /// </summary>
        public static int Short => 128;

        /// <summary>
        /// Use as maxLenght value in normal text column (f.e Name) = 255
        /// </summary>
        public static int Normal => 255;

        /// <summary>
        /// Use as maxLenght value in medium text fields (f.e Description) 
        /// value: MySQL5.6 = 255,  Aurora = 512 
        /// </summary>
        public static int Medium =>
#if MYSQL56
            255;
#else
            512;
#endif
        /// <summary>
        /// Use as maxLenght value in large text fields = 1024
        /// </summary>

        public static int Large => 1024;

        /// <summary>
        /// Use as maxLenght value in huge text fields = Int32.MaxValue
        /// </summary>
        public static int MAX => Int32.MaxValue;

        /// <summary>
        /// Use as maxLenght value in TokenizedName fields 
        /// </summary>
        public static int SearchField => 500;


    }
}
