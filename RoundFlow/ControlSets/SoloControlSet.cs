using SMG.Coordination;
using SMG.Santas.ObjectScripts;


namespace SMG.Santas.RoundFlow
{
  public class SoloControlSet : AbstractControlSet
  {
    /// <summary>
    /// Injected settings, set in the Installer via the Unity Inspector.
    /// </summary>
    Settings _Settings;

    public SoloControlSet(
      EventSource EventSource,
      Settings Settings)
      : base(EventSource)
    {
      _Settings = Settings;
    }

    public override string Slug()
    {
      return "soloControlSet";
    }

    public override void Activate()
    {
      isActive = true;
      // @TODO start coroutine to spawn random item
    }

    public override void Deactivate()
    {
      isActive = false;
      // @TODO stop coroutine that spawns random item
    }

    public class Settings
    {

    }
  }
}

