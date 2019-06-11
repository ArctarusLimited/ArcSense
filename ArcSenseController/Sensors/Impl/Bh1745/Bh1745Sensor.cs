﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcDataCore.Models.Sensor;

namespace ArcSenseController.Sensors.Impl.Bh1745
{
    internal class Bh1745Sensor : I2CSensor
    {
        private const byte BH1745_SLAVE_ADDRESS = 0x38;
        public override Task InitialiseAsync()
        {
            throw new NotImplementedException();
        }

        public override SensorModel Model { get; }
    }
}
