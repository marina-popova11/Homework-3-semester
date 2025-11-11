// <copyright file="Tests.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Test1.Tests;

public class Tests
{
    private readonly string testDirectory;

    public Tests()
    {
        this.testDirectory = Path.Combine(Path.GetTempPath(), "HashTest_" + Guid.NewGuid());
        Directory.CreateDirectory(this.testDirectory);
    }

    [Test]
    public async Task Test_CalculateEmptyDirectory()
    {
        var calculator = new SingleThread();
        var dir = Path.Combine(this.testDirectory, "Empty");
        Directory.CreateDirectory(dir);
        var hash = await calculator.CalculateDirectoryHashAsync(dir);
        Assert.That(hash, Is.Not.Null);
        Assert.That(hash.Length, Is.EqualTo(16));
    }

    [Test]
    public async Task Test_SingleAndMulti_REturnTheSame()
    {
        var singleCalculator = new SingleThread();
        var multiCalculator = new MultiThread();
        var dir = Path.Combine(this.testDirectory, "Single/Multi");
        Directory.CreateDirectory(dir);
        for (int i = 0; i < 5; ++i)
        {
            var filePath = Path.Combine(dir, $"file{i}.txt");
            await File.WriteAllTextAsync(filePath, $"Content {i}");
            var subDir = Path.Combine(dir, $"dir{i}");
            Directory.CreateDirectory(subDir);
            await File.WriteAllTextAsync(Path.Combine(subDir, "subfile.txt"), "Sub content");
        }

        var singleThreadHash = await singleCalculator.CalculateDirectoryHashAsync(dir);
        var multiThreadHash = await multiCalculator.CalculateDirectoryHashAsync(dir);

        Assert.That(singleThreadHash, Is.EqualTo(multiThreadHash));
    }
}
