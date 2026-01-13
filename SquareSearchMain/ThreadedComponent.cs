
namespace SquareSearchMain
{
    public interface IStartStop
    {
        void Start();
        void Stop();
    }

    public abstract class ThreadedComponent : IStartStop
    {
        private CancellationTokenSource cts = new CancellationTokenSource();
        private Task worker = Task.CompletedTask;

        public void Start()
        {
            Stop();

            cts = new CancellationTokenSource();
            Ct = cts.Token;
            worker = Task.Run(Worker);
        }

        public void Stop()
        {
            cts.Cancel();
            worker.Wait();
        }

        protected abstract void DoWork();
        protected CancellationToken Ct { get; private set; }

        private void Worker()
        {
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    DoWork();
                }
                catch (TaskCanceledException)
                {
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Environment.Exit(1);
                }

                cts.Token.WaitHandle.WaitOne(200);
            }
        }
    }
}
