namespace ReplaceMeWithYourProjectsNamespace
{
    internal sealed class SystemOne : ISystem
    {
        private readonly RuntimeData runtimeData;

        public SystemOne(RuntimeData runtimeData)
        {
            this.runtimeData = runtimeData;
        }

        public bool Autostart => true;

        public void Start() { }

        public void Stop() { }

        public void Update()
        {
            runtimeData.SystemOneData++;
        }
    }
}
