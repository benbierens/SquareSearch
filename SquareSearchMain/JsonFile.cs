using System.Text.Json;

namespace SquareSearchMain
{
    public abstract class JsonFile<T> : IStartStop where T : new()
    {
        protected abstract string Filename { get; }
        protected T Model { get; private set; } = new T();

        protected virtual void OnStartFinished()
        {
        }

        protected virtual void OnStopStarted()
        {
        }

        public void Start()
        {
            if (File.Exists(Filename))
            {
                try
                {
                    Model = JsonSerializer.Deserialize<T>(File.ReadAllText(Filename))!;
                    if (Model == null) Model = new T();
                }
                catch { }
            }

            OnStartFinished();
        }

        public void Stop()
        {
            OnStopStarted();

            File.WriteAllText(Filename, JsonSerializer.Serialize(Model));
        }
    }
}
