﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ArcDataCore.Models.Sensor;
using ArcSenseController.Models.Sensor.Types;

namespace ArcSenseController.Models.Sensor.Impl.Lsm303D
{
    /// <summary>
    /// Interface to the LSM303D accelerometer/magnetometer.
    /// </summary>
    internal sealed class Lsm303DSensor : I2CSensor, ISplitSensor
    {
        internal Lsm303DAccelerometer Accelerometer { get; }
        internal Lsm303DMagnetometer Magnetometer { get; }

        private const byte LSM_303D_SLAVE_ADDRESS = 0x1D;

        public Lsm303DSensor()
        {
            Accelerometer = new Lsm303DAccelerometer(this);
            Magnetometer = new Lsm303DMagnetometer(this);
        }

        public override async Task InitialiseAsync()
        {
            await InitI2C(LSM_303D_SLAVE_ADDRESS);

            await Accelerometer.InitialiseAsync();
            await Magnetometer.InitialiseAsync();
        }

        public override SensorModel Model => SensorModel.Lsm303D;
        public IEnumerable<ISensor> SubSensors => new ISensor[] { Accelerometer, Magnetometer };
    }
}
