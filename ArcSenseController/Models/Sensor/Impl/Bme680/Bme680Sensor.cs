﻿using System;
using System.Threading.Tasks;
using ArcDataCore.Models.Sensor;

namespace ArcSenseController.Models.Sensor.Impl.Bme680
{
    /// <summary>
    /// Implementation of the Bosch BME680 environment sensor driver.
    /// </summary>
    /// <remarks>
    /// The BME680 is able to measure gas resistance (air quality), humidity, pressure, and temperature.
    /// </remarks>
    internal class Bme680Sensor : I2CSensor, IDisposable
    {
        /// <summary>
        /// Gets the internal pressure sensor.
        /// </summary>
        internal Bme680PressureSensor PressureSensor { get; }

        /// <summary>
        /// Gets the internal humidity sensor.
        /// </summary>
        internal Bme680HumiditySensor HumiditySensor { get; }

        /// <summary>
        /// Gets the internal temperature sensor.
        /// </summary>
        internal Bme680TemperatureSensor TempSensor { get; }

        /// <summary>
        /// Gets the internal gas sensor.
        /// </summary>
        internal Bme680GasResistSensor GasSensor { get; }

        // I2C Slave Address
        private const byte BME680_SLAVE_ADDRESS = 0x76;

        #region Constructor

        /// <summary>
        /// Initiates the BME680 sensor to get air quality level, temperature, humidity, pressure and altitude.
        /// The <see cref="InitialiseAsync"/> method must be called in order to initialise the sensor for use.
        /// </summary>
        public Bme680Sensor()
        {
            PressureSensor = new Bme680PressureSensor(this);
            HumiditySensor = new Bme680HumiditySensor(this);
            TempSensor = new Bme680TemperatureSensor(this);
            GasSensor = new Bme680GasResistSensor(this);
        }

        #endregion

        #region I2CCom

        /// <summary>
        /// Initiates the sensor with the specified configuration,
        /// or the default if none is passed.
        /// </summary>
        public override async Task InitialiseAsync()
        {
            await InitI2C(BME680_SLAVE_ADDRESS);

            // Reset the sensor and wait for boot
            ResetSensor();
            Task.Delay(10).Wait();

            // Check it is reporting a valid ID
            CheckSensor();

            // Initialise & configure sub-sensor drivers
            await PressureSensor.InitialiseAsync();
            await HumiditySensor.InitialiseAsync();
            await TempSensor.InitialiseAsync();
            await GasSensor.InitialiseAsync();

            // Configure parameters
            ConfigureGlobal();
        }

        public override SensorModel Model => SensorModel.Bme680;

        /// <summary>
        /// Sets the initial configuration data.
        /// </summary>
        private void ConfigureGlobal()
        {
            // Select temperature and pressure oversamplings.
            byte configValue = 0x00;
            configValue |= (byte)TempSensor.TemperatureOversampling;
            configValue |= (byte)PressureSensor.PressureOversampling;
            Device.Write(new[] { (byte)Bme680Registers.CtrlMeasurement, Convert.ToByte(configValue) });
            Task.Delay(1).Wait();

            // Set mode to forced mode.
            configValue = ReadRegister_OneByte(Bme680Registers.Mode);
            configValue |= (byte)Bme680OperationModes.ForcedMode;
            Device.Write(new[] { (byte)Bme680Registers.Mode, configValue });
            Task.Delay(1).Wait();
        }

        /// <summary>
        /// Reads data from the I2C device.
        /// </summary>
        /// <param name="reg">Read address.</param>
        /// <returns>Register data.</returns>
        internal uint ReadRegister_TwoBytes_LSBFirst(Bme680Registers reg)
        {
            return ReadUint((byte) reg);
        }

        internal byte ReadRegister_OneByte(Bme680Registers reg)
        {
            return ReadByte((byte) reg);
        }

        #endregion

        #region Sensor Configuration

        /// <summary>
        /// Verifies the sensor ID.
        /// </summary>
        /// <returns>True if sensor responses correctly. False if not.</returns>
        private bool CheckSensor()
        {
            return ReadRegister_OneByte(Bme680Registers.Id) == 0x61;
        }

        /// <summary>
        /// Initiates a soft-reset procedure, which has the same effect like power-on reset.
        /// </summary>
        private void ResetSensor()
        {
            Device.Write(new byte[] { (byte)Bme680Registers.Reset, 0xB6 });
        }

        #endregion

        #region Sensor Readouts

        /// <summary>
        /// Triggers all measurements, and then waits for measurement completion.
        /// </summary>
        internal void ForceRead()
        {
            var temp = ReadRegister_OneByte(Bme680Registers.Mode);
            temp |= (byte)Bme680OperationModes.ForcedMode;

            Device.Write(new[] {(byte) Bme680Registers.Mode, temp});

            while (GetMeasuringState())
                Task.Delay(1).Wait();
        }

        /// <summary>
        /// Gets the measuring status from the measuring bit.
        /// </summary>
        /// <returns>True if all conversions are running. False if not.</returns>
        private bool GetMeasuringState()
        {
            var readValue = ReadRegister_OneByte(Bme680Registers.EasStatus0);
            return (readValue & 0b00100000) == 0b00100000;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Gets the new data status from the new_data_0 bit.
        /// </summary>
        /// <returns>True if measured data are stored into the output data registers. False if not.</returns>
        private bool GetNewDataStatus()
        {
            var readValue = ReadRegister_OneByte(Bme680Registers.EasStatus0);
            return (readValue & 0b10000000) == 0b10000000;
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Cleans up the resources.
        /// </summary>
        public void Dispose()
        {
            Device.Dispose();
        }

        #endregion
    }
}
