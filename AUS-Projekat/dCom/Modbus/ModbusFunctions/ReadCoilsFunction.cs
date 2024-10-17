using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    // tip registra/ broj registara/ adresa/pozicija zareaz/ opseg od do/pocetna vrednost/ tip Ulaz/Izla/ Opis
    /// <summary>
    /// Class containing logic for parsing and packing modbus read coil functions/requests.
    /// </summary>
    public class ReadCoilsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
		public ReadCoilsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        

        /// <inheritdoc/>
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            // Kastovanje opstih parametara na Read parametre da bismo dobili StartAddress i Quantity
            ModbusReadCommandParameters parameters = this.CommandParameters as ModbusReadCommandParameters;

            // Kreiranje paketa koji saljemo simulatoru
            byte[] request = new byte[12];

            // Vrednosti velicine dva bajta pakujemo po dole prikazanom principu
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parameters.TransactionId)), 0, request, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parameters.ProtocolId)), 0, request, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parameters.Length)), 0, request, 4, 2);

            // Vrednosti velicine jednog bajta pakujemo sa jednostavnom jednakoscu
            request[6] = parameters.UnitId;
            request[7] = parameters.FunctionCode;

            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parameters.StartAddress)), 0, request, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parameters.Quantity)), 0, request, 10, 2);

            return request;
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
                ModbusReadCommandParameters parameters = this.CommandParameters as ModbusReadCommandParameters;
 
                ushort quantity = response[8];
                ushort value;
                for (int i = 0; i < quantity; i++)                  // Prolazak kroz sve bajte
                {
                    byte currentByte = response[9 + i];             // Uzimanje trenutnog bajta, podaci krecu od devetog
                    for (int j = 0; j < 8; j++)                     // Prolazak kroz bite
                    {
                        value = (ushort)(currentByte & (byte)0x1);  // Logicko i
                        currentByte >>= 1;                          // Shift bita u desno kako bi obradjeni ispao

                        if (parameters.Quantity < (j + i * 8))      // Provera kada prestati sa citanjem vrednosti
                            break;
                        // Adrese su sekvencijalne zato mozemo
                        // Na ovaj nacin ili nekim brojacem
                        responseDict.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, (ushort)(parameters.StartAddress + (j + i * 8))), value);
                    }
                }
            }
            return responseDict;
        }
    }
}