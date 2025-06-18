using System.Media;

namespace Hermes.Server.Speaker
{
 public class Toner
 {
  public static Toner? runningToner;

  private bool stop = false;
  private string myTone;
  private SoundPlayer? mySoundPlayer;

  public Toner(string tone)
  {
   if (runningToner != null)
    runningToner.Stop();

   mySoundPlayer = new SoundPlayer();
   myTone = tone;
   runningToner = this;
  }

  public async Task go()
  {
   string path = Configuration.toneLocation + "/" + myTone + ".wav";
   mySoundPlayer.SoundLocation = path;
   mySoundPlayer.Load();
   mySoundPlayer.PlayLooping();
  }

  public void Stop()
  {
   if (stop)
    return;
   runningToner = null;
   mySoundPlayer?.Stop();
   mySoundPlayer?.Dispose();
   mySoundPlayer = null;
   stop = true;
  }

 }
}
