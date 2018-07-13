using ManualConnectedCiotola.Models;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TwinE;

namespace Ciotola
{
    public class Program
    {

        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            var cd = new CiotolaData();
            cd.Dose = "5";

            var deviceId = configuration["deviceId"];
            var authenticationMethod =
                new DeviceAuthenticationWithRegistrySymmetricKey(
                    deviceId,
                    configuration["deviceKey"]);

            //cd.Id = deviceId;

            var transportType =TransportType.Mqtt;
            if (!string.IsNullOrWhiteSpace(configuration["transportType"]))
            {
                transportType = (TransportType)
                    Enum.Parse(typeof(TransportType),
                    configuration["transportType"], true);

            }

            var client = DeviceClient.Create(
                configuration["hostName"],
                authenticationMethod,
                transportType
            );

            var twin = await client.GetTwinAsync();

            if (twin.IsDesiredPropertyEmpty("Dose"))
            {
                cd.Dose = twin.DesiredProperty("Dose");
            }

            await client.SetDesiredPropertyUpdateCallbackAsync(async (tc, oc) =>
            {
                cd.Dose = (string)tc["Dose"];
            }, null);
            while (true)
            {
                var message = await client.ReceiveAsync();
                if (message == null) continue;

                var bytes = message.GetBytes();
                if (bytes == null) continue;

                var text = Encoding.UTF8.GetString(bytes);

                Console.WriteLine($"Messaggio ricevuto: {text}");
                var textParts = text.Split();
                switch (textParts[0].ToLower())
                {
                    case "send":
                        await Send(client);
                        break;
                    case "refill":
                        await Refill(client);
                        break;
                    default:
                        Console.WriteLine("Syntax error");
                        break;
                }
                await client.CompleteAsync(message);
            }
        }

        private static async Task Send(DeviceClient client)
        {
            var Dose = "2";
            var resp = new TwinCollection();
            resp["Dose"] = Dose;
            await client.UpdateReportedPropertiesAsync(resp);
        }

        private static async Task Refill(DeviceClient client)
        {
            string Dose = "4";
            var resp = new TwinCollection();
            resp["Dose"] = Dose;
            await client.UpdateReportedPropertiesAsync(resp);

        }
    }
}
