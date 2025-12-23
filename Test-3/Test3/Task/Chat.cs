// <copyright file="Chat.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Test3;

using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// Class for handle chat.
/// </summary>
public static class Chat
{
    /// <summary>
    /// Creates and manages a chat.
    /// </summary>
    /// <param name="client">Client for connecting.</param>
    /// <returns>Result.</returns>
    public static async Task CreateChatAsync(TcpClient client)
    {
        var stream = client.GetStream();
        var buffer = new byte[1024];
        var readTask = Task.Run(async () =>
        {
            while (client.Connected)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (message.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Interlocutor disconnected.");
                        return;
                    }

                    Console.WriteLine($"Get a message: {message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        });

        while (client.Connected)
        {
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                continue;
            }

            byte[] data = Encoding.UTF8.GetBytes(input);
            await stream.WriteAsync(data, 0, data.Length);

            if (input.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
        }

        await Task.WhenAny(readTask, Task.Delay(100));
        client.Close();
    }
}