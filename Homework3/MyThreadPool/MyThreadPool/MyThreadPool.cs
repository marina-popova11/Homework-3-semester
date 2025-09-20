// <copyright file="MyThreadPool.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyThreadPool;

public class MyThreadPool<TResult>
{
    private int numberThreads;
    private Queue<Thread> threads;
    private bool expectation;

    public MyThreadPool(int numberThreads)
    {
        this.numberThreads = numberThreads;
        this.expectation = true;
    }

    public void ShutDown()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="func"></param>
    public void Enqueue(Func<TResult> func)
    {
    }
}