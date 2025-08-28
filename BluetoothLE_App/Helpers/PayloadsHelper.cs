using BluetoothLE_App.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE_App.Helpers
{
    internal class PayloadsHelper
    {
        public static string GetStringFromPayload<TPayload>(TPayload payload) where TPayload : BasePagePayload
        {
            var payloadString = string.Empty;

            try
            {
                payloadString = JsonConvert.SerializeObject(payload, Formatting.Indented);
            }
            catch (JsonSerializationException exception)
            {
                Console.WriteLine(exception.Message);
            }
            catch (JsonException exception)
            {
                Console.WriteLine(exception.Message);
            }

            return payloadString;
        }

        public static TPayload? GetPayloadFromString<TPayload>(string? payloadString)
            where TPayload : BasePagePayload
        {
            TPayload? payload = default;

            try
            {
                if (string.IsNullOrWhiteSpace(payloadString))
                {
                    return default;
                }

                payload = JsonConvert.DeserializeObject<TPayload>(Uri.UnescapeDataString(payloadString));
            }
            catch (JsonSerializationException exception)
            {
                Console.WriteLine(exception.Message);
            }
            catch (JsonException exception)
            {
                Console.WriteLine(exception.Message);
            }

            return payload;
        }
    }
}
