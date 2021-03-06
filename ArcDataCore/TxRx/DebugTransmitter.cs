﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ArcDataCore.Models.Sensor;
using ArcDataCore.Transport;
using MessagePack;

namespace ArcDataCore.TxRx
{
    /// <summary>
    /// Transport that logs everything that it sees to the console.
    /// Useful for debugging or development purposes.
    /// </summary>
    public class DebugTransmitter : ITransmitter
    {
        private static int _totalWritten;

        public bool Enabled => true;

        public Task<bool> PushAsync(TransportDataPackage series)
        {
            var totalBytes = MessagePackSerializer.Serialize(series);
            _totalWritten += totalBytes.Length;
            
            //Debug.WriteLine($"Debug data: {data.Model}, {data.DataType}");
            return Task.FromResult(true);
        }
    }
}
