namespace ReplaceMeWithYourProjectsNamespace
{
  internal sealed class SystemTwo : ISystem
  {
    private readonly RuntimeData runtimeData;

    public SystemTwo(RuntimeData runtimeData)
    {
      this.runtimeData = runtimeData;
    }

    public bool Autostart => true;

    public void Start() { }

    public void Stop() { }

    public void Update()
    {
      runtimeData.SystemTwoData++;
    }
  }
}
