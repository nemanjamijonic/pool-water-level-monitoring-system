using System;

namespace ProcessingModule
{
    /// <summary>
    /// Class containing logic for engineering unit conversion.
    /// </summary>
    public class EGUConverter
	{
        /// <summary>
        /// Converts the point value from raw to EGU form.
        /// </summary>
        /// <param name="scalingFactor">The scaling factor.</param>
        /// <param name="deviation">The deviation</param>
        /// <param name="rawValue">The raw value.</param>
        /// <returns>The value in engineering units.</returns>
		public double ConvertToEGU(double scalingFactor, double deviation, ushort rawValue)
		{
            double egu_value = rawValue * scalingFactor + deviation;
            return egu_value;
		}

        /// <summary>
        /// Converts the point value from EGU to raw form.
        /// </summary>
        /// <param name="scalingFactor">The scaling factor.</param>
        /// <param name="deviation">The deviation.</param>
        /// <param name="eguValue">The EGU value.</param>
        /// <returns>The raw value.</returns>
		public ushort ConvertToRaw(double scalingFactor, double deviation, double eguValue)
        {
            double raw_value;
            raw_value = (eguValue - deviation) / scalingFactor;
            return (ushort)(raw_value);
		}
	}
}
