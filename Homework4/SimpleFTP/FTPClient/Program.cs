// <copyright file="Program.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using FTPClient;

if (args.Length != 2)
{
    Console.WriteLine("Please enter the IP address and port.");
    return;
}

if (!IPAddress.TryParse(args[0], out var ip))
{
    Console.WriteLine("Incorrect IP address. Enter the right IP address.");
    return;
}

if (!int.TryParse(args[1], out var port))
{
    Console.WriteLine("Incorrect port. Enter the port that is not busy and is working");
}

var client = new Client();
await client.Connect(ip, port);
var listCommand = "1";
var getCommand = "2";
var exitCommand = "exit";
var isContinue = true;
try
{
    while (isContinue)
    {
        Console.WriteLine("Enter the command");
        Console.WriteLine($"{listCommand} - for listing files.");
        Console.WriteLine($"{getCommand} - for getting file.");
        Console.WriteLine($"{exitCommand} - for getting file.");
        var command = Console.ReadLine();
        switch (command)
        {
            case "1":
                {
                    Console.WriteLine("Enter the path: ");
                    var path = Console.ReadLine();
                    var response = await client.CommandList(path!);
                    if (response.Count == 0)
                    {
                        Console.WriteLine("Directory is empty.");
                    }
                    else
                    {
                        Console.WriteLine("Files:\n");
                        foreach (var file in response)
                        {
                            var type = file.IsDir ? "<Dir>" : "File";
                            Console.WriteLine($"{type}: {file.Name}");
                        }
                    }

                    break;
                }

            case "2":
                {
                    Console.WriteLine("Enter the path: ");
                    var path = Console.ReadLine();
                    Console.WriteLine("Enter the path where file will be download to: ");
                    var part2 = Console.ReadLine();
                    var response = await client.CommandGet(path!, part2!);
                    Console.WriteLine("File was copied.");
                    break;
                }

            case "exit":
                {
                    await client.Disconnect();
                    isContinue = false;
                    break;
                }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Client error: {ex.Message}");
}