using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write coil functions/requests.
    /// </summary>
    public class WriteSingleCoilFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleCoilFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            byte[] pack = new byte[12];
            ModbusWriteCommandParameters parameters = CommandParameters as ModbusWriteCommandParameters;

            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.TransactionId)), 0, pack, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.ProtocolId)), 0, pack, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.Length)), 0, pack, 4, 2);

            pack[6] = CommandParameters.UnitId;
            pack[7] = CommandParameters.FunctionCode;

            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)(parameters.OutputAddress))), 0, pack, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)(parameters.Value))), 0, pack, 10, 2);

            return pack;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            Dictionary<Tuple<PointType, ushort>, ushort> responseDict = new Dictionary<Tuple<PointType, ushort>, ushort>();

            if (response[7] == CommandParameters.FunctionCode + 0x80)   // Pogledati prezentaciju
            {
                HandeException(response[8]);                            // Pogledati paket
            }
            else
            {
                ushort address = BitConverter.ToUInt16(response, 8);    // Radimo isto kao i kod citanja vrednosti
                                                                        // ne smemo menjati vrednost na skada stanici ako nam polje 
                                                                        // ili simulator nije vratio potvrdu da je izvrsio komandu
                address = (ushort)IPAddress.NetworkToHostOrder((short)address);

                ushort value = BitConverter.ToUInt16(response, 10);
                value = (ushort)IPAddress.NetworkToHostOrder((short)value);

                responseDict.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, address), value);
            }

            return responseDict;
        }
    }
}