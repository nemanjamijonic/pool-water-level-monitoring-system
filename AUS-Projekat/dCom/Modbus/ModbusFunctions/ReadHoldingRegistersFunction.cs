using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read holding registers functions/requests.
    /// </summary>
    public class ReadHoldingRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadHoldingRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadHoldingRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            ModbusReadCommandParameters parameters = this.CommandParameters as ModbusReadCommandParameters;
            byte[] pack = new byte[12];

            short networkOrderBytes = IPAddress.HostToNetworkOrder((short)parameters.TransactionId); //Konvertovanje redosleda bajtova
            byte[] byteArray = BitConverter.GetBytes(networkOrderBytes); // Pretvaranje short vrednosti u niz bajta

            // Kopiranje pretvorenog niza bajta u krajnji niz pack 
            Buffer.BlockCopy(byteArray, 0, pack, 0, 2); // Iz kog niza, sa koje pozicije, u koji niz, sa koje pozicije, koliko bajtova


            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parameters.ProtocolId)), 0, pack, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parameters.Length)), 0, pack, 4, 2);

            pack[6] = parameters.UnitId;
            pack[7] = parameters.FunctionCode;

            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parameters.StartAddress)), 0, pack, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parameters.Quantity)), 0, pack, 10, 2);

            return pack;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            Dictionary<Tuple<PointType, ushort>, ushort> responseDict = new Dictionary<Tuple<PointType, ushort>, ushort>();
            ModbusReadCommandParameters parameters = CommandParameters as ModbusReadCommandParameters;
            if (response[7] == CommandParameters.FunctionCode + 0x80)   // Pogledati prezentaciju
            {
                HandeException(response[8]);                            // Pogledati paket
            }
            else
            {
                ushort address = parameters.StartAddress;                       // Adresa prve vrednosti 
                ushort value;
                for (int i = 0; i < response[8]; i = i + 2)                     // Prolazak kroz sve bajte, prolazimo duplo manje puta jer
                {                                                               // znamo da je vrednost velicine dva bajta
                    value = BitConverter.ToUInt16(response, (i + 9));           // Dva bajta -> ushort
                    value = (ushort)IPAddress.NetworkToHostOrder((short)value); // Network -> host
                    // value = (ushort)(response[10+i] + (response[9+i] << 8)); // Radi istu stvar koji i dve linije iznad
                    responseDict.Add(new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, address), value);
                    address++; //Sekvencijalne adrese
                }
            }

            return responseDict;
        }
    }
}