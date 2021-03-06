﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ArcDataCore.Models.Sensor
{
    /// <summary>
    /// Represents different sensor data types for server communication.
    /// </summary>
    public enum SensorDataType : ushort
    {
        Unknown = 0,

        /// <summary>
        /// Raw acceleration data as an array of three shorts.
        /// </summary>
        Accelerometer3D = 10,

        /// <summary>
        /// Raw flux data as an array of three shorts.
        /// </summary>
        Magnetometer3D = 11,

        /// <summary>
        /// Gas resistance as a double.
        /// </summary>
        GasResistance = 20,
        
        /// <summary>
        /// Relative humidity as a double.
        /// </summary>
        RelativeHumidity = 21,
        
        /// <summary>
        /// Pressure as a double.
        /// </summary>
        Pressure = 22,
        
        /// <summary>
        /// Temperature as a double.
        /// </summary>
        Temperature = 23,

        /// <summary>
        /// Spectral data as a byte array of six floats.
        /// </summary>
        Spectral = 25,

        /// <summary>
        /// Colour data
        /// </summary>
        Colour = 26,

        /// <summary>
        /// Radiation as counts per minute.
        /// </summary>
        Radiation = 30,

        // Virtual data types
        /// <summary>
        /// Index of Air Quality (IAQ)
        /// </summary>
        VAirQualityIndex = 1010,
    }
}
