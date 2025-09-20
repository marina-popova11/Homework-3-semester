// <copyright file="MyThreadPool.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

public class MyThreadPool<TResult>
{
    private int numberThreads;
    public Func<TResult> Task = new();
    public bool Expectation;

    public MyThreadPool(int numberThreads)
    {
        this.numberThreads = numberThreads;
        this.Expectation = true;
    }

    public void ShutDown()
    {
    }

    // private class SelfThread<TResult>
    // {
    //     public Func<TResult> Task = new();
    //     public bool expectation;

    //     public SelfThread()
    //     {
    //         this.expectation = true;
    //     }
    // }
}