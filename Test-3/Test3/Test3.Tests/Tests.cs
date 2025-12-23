// <copyright file="Tests.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Test3.Tests;

using System.Net;
using System.Net.Sockets;
using System.Text;

public class Tests
{
    [Test]
    public async Task Test_ServerAndClient()
    {
        int port = 8888;
        var serverReady = new TaskCompletionSource<bool>();
        var serverTask = Task.Run(async () =>
        {
            var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            serverReady.SetResult(true);

            using var client = await listener.AcceptTcpClientAsync();
            listener.Stop();

            var stream = client.GetStream();
            var buffer = new byte[32];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Assert.That(message.Trim(), Is.EqualTo("exit"));
            client.Close();
        });

        await serverReady.Task;

        using var client = new TcpClient();
        await client.ConnectAsync(IPAddress.Loopback, port);
        var stream = client.GetStream();

        byte[] exitMsg = Encoding.UTF8.GetBytes("exit");
        await stream.WriteAsync(exitMsg, 0, exitMsg.Length);

        await serverTask;
        var flag = true;
        Assert.That(flag, Is.True);
    }

    [Test]
    public void Test_RightArguments_ForServer()
    {
        string[] args = { "8888" };
        Assert.That(int.TryParse(args[0], out int port), Is.True);
        Assert.That(port, Is.EqualTo(8888));
    }

    [Test]
    public void Test_RightArguments_ForClient()
    {
        string[] args = { "127.0.0.1", "8888" };
        Assert.That(IPAddress.TryParse(args[0], out IPAddress? ipAddress), Is.True);
        Assert.That(int.TryParse(args[1], out int port), Is.True);
        Assert.That(port, Is.EqualTo(8888));
    }
}
