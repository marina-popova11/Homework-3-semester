// <copyright file="ServerTests.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace SimpleFTP.Tests;

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using FTPClient;
using FTPServer;

public class ServerTests
{
    private int serverPort;
    private Server? server;
    private CancellationTokenSource? cts;

    [SetUp]
    public async Task Setup()
    {
        var testDirPath = Path.Combine(Directory.GetCurrentDirectory(), "test");
        testDirPath = Path.GetFullPath(testDirPath);

        if (!Directory.Exists(testDirPath))
        {
            Directory.CreateDirectory(testDirPath);
            File.WriteAllText(Path.Combine(testDirPath, "hello.txt"), "hello world!");
        }

        var emptyDir = Path.Combine(Directory.GetCurrentDirectory(), "empty");
        if (!Directory.Exists(emptyDir))
        {
            Directory.CreateDirectory(emptyDir);
        }

        using var tempListener = new TcpListener(IPAddress.Loopback, 0);
        tempListener.Start();
        this.serverPort = ((IPEndPoint)tempListener.LocalEndpoint).Port;
        tempListener.Stop();

        this.cts = new CancellationTokenSource();
        this.server = new Server(IPAddress.Loopback, this.serverPort);
        _ = this.server.RunAsync(this.cts.Token);

        // Task.Delay(100).Wait();
        var retry = 0;
        const int maxRetries = 20;
        while (retry++ < maxRetries)
        {
            using var client = new TcpClient();
            try
            {
                await client.ConnectAsync(IPAddress.Loopback, this.serverPort);
                break;
            }
            catch
            {
                await Task.Delay(50);
            }
        }
    }

    [TearDown]
    public async Task TearDown()
    {
        this.cts?.Cancel();
        if (this.server != null)
        {
            await this.server.DisposeAsync();
        }

        this.cts?.Dispose();
    }

    [Test]
    public async Task Test_ServerShouldListTestDir()
    {
        var testDirPath = Path.Combine(Directory.GetCurrentDirectory(), "test");
        testDirPath = Path.GetFullPath(testDirPath);

        Console.WriteLine($"Looking for directory: {testDirPath}");

        var client = new Client();
        try
        {
            await client.ConnectAsync(IPAddress.Loopback, this.serverPort);
            var files = await client.CommandListAsync(testDirPath);

            Assert.That(files.Count, Is.GreaterThan(0));
            Assert.That(files.Any(f => f.Name == "hello.txt"), Is.True);
        }
        finally
        {
            await client.DisposeAsync();
        }
    }

    [Test]
    public async Task Test_ServerShouldReturnsEmptyListForEmptyDir()
    {
        var emptyDirPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "empty"));
        var client = new Client();
        try
        {
            await client.ConnectAsync(IPAddress.Loopback, this.serverPort);
            var files = await client.CommandListAsync(emptyDirPath);
            Assert.That(files.Count, Is.EqualTo(0));
        }
        finally
        {
            await client.DisposeAsync();
        }
    }
}