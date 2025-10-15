// <copyright file="ClientTests.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace SimpleFTP.Tests;

using System.Net;
using System.Threading.Tasks;
using FTPClient;
using FTPServer;

public class ClientTests
{
    private Client client;

    [SetUp]
    public void SetUp()
    {
        this.client = new();
    }

    [OneTimeSetUp]
    public void StartServer()
    {
        var server = new Server(IPAddress.Loopback, 8888);
        _ = Task.Run(async () => await server.Run());
    }

    [Test]
    public void Test_IsConnected_Default()
    {
        Assert.That(this.client.IsConnect, Is.False);
    }

    [Test]
    public async Task Test_IsConnected_ReturnTrue()
    {
        await this.client.Connect(IPAddress.Loopback, 8888);
        var result = this.client.IsConnect();
        Assert.That(result, Is.True);
        await this.client.Disconnect();
    }

    [Test]
    public void Test_CommandList_IfClientDoesNotConnect()
    {
        // await this.client.Connect(IPAddress.Loopback, 8888);
        // var path = "directory.txt";
        var result = Assert.ThrowsAsync<InvalidOperationException>(async () => await this.client.CommandList("C:\\Temp"));
        Assert.That(result.Message, Does.Contain("Client does not connect!"));
    }

    [Test]
    public void Test_CommandGet_IfClientDoesNotConnect()
    {
        var result = Assert.ThrowsAsync<InvalidOperationException>(async () => await this.client.CommandGet("C:\\Temp", "C:\\Temp"));
        Assert.That(result.Message, Does.Contain("Client does not connect!"));
    }
}
