// <copyright file="Client.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace FTPClient;

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

/// <summary>
/// Class for ftp client,
/// you can listing files and
/// get a file from server.
/// </summary>
public class Client
{
    private TcpClient client;
    private Stream stream;
    private StreamWriter writer;
    private StreamReader reader;
    private bool isConnected;

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    public Client()
    {
        this.client = new TcpClient();
        this.stream = this.client.GetStream();
        this.writer = new StreamWriter(this.stream);
        this.reader = new StreamReader(this.stream);
        this.isConnected = false;
    }

    /// <summary>
    /// Gets flag for isConnected.
    /// </summary>
    /// <returns>True or false.</returns>
    public bool IsConnect() => this.isConnected;

    /// <summary>
    /// Connects the client to the server.
    /// </summary>
    /// <param name="ip">Ip address for connect to.</param>
    /// <param name="port">Port number for connect to.</param>
    /// <returns>Completed task.</returns>
    public async Task Connect(IPAddress ip, int port)
    {
        await this.client.ConnectAsync(ip, port);
        this.isConnected = true;
    }

    /// <summary>
    /// Viewing files in a directory on the server.
    /// </summary>
    /// <param name="path">What the file will be viewed.</param>
    /// <returns>List of files in directory.</returns>
    public async Task<List<FileInfo>> CommandList(string path)
    {
        if (!this.IsConnect())
        {
            await this.writer.WriteLineAsync("Client does not connect!");
            return new List<FileInfo>();
        }

        await this.writer.WriteLineAsync($"1 {path}");
        var response = await this.reader.ReadLineAsync();
        if (response == "-1")
        {
            return new List<FileInfo>();
        }

        if (!int.TryParse(response, out var size) || size < 0)
        {
            return new List<FileInfo>();
        }

        var list = new List<FileInfo>();
        for (int i = 0; i < size; ++i)
        {
            var line = await this.reader.ReadLineAsync();
            var parts = line!.Split(' ');
            var fileObject = new FileInfo(parts[1]);
            list.Add(fileObject);

            // list.Add(new FileInfo
            // {
            //     Size = long.Parse(parts[0]),
            //     Name = parts[1],
            //     IsDir = bool.Parse(parts[3]),
            // });
        }

        return list;
    }

    /// <summary>
    /// Downloading a file from server.
    /// </summary>
    /// <param name="path">What the file will be copy.</param>
    /// <param name="localPath">Where file will be download to.</param>
    /// <returns>True if file was successfully downloaded, false overwise.</returns>
    public async Task<bool> CommandGet(string path, string localPath)
    {
        if (!this.IsConnect())
        {
            await this.writer.WriteLineAsync("Client does not connect!");
            return false;
        }

        await this.writer.WriteLineAsync($"2 {path}");
        var response = await this.reader.ReadLineAsync();
        if (response == "-1")
        {
            return false;
        }

        if (!long.TryParse(response, out var size) || size < 0)
        {
            return false;
        }

        using (var localFile = File.Create(localPath))
        using (var netStream = this.client.GetStream())
        {
            var buffer = new byte[4096];
            var readBytes = 0;
            while (readBytes < size)
            {
                var remainingBytes = await netStream.ReadAsync(buffer, 0, (int)Math.Min(buffer.Length, size - readBytes));
                await localFile.WriteAsync(buffer, 0, remainingBytes);
                ++readBytes;
            }
        }

        return true;
    }

    /// <summary>
    /// Terminates the client's work.
    /// </summary>
    /// <returns>Completed task.</returns>
    public async Task Disconnect()
    {
        this.reader.Close();
        this.writer.Close();
        this.stream.Close();
        this.client.Close();
    }
}