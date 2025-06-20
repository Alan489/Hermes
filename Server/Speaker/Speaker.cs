using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using Hermes.Server;
using NAudio.Wave;
using System.IO;

namespace Hermes.Server.Speaker
{
 public class Speaker
 {
  public static bool speaking = false;
  private static List<string> queue = new List<string>();
  public static void speak(string whatSay)
  {
   queue.Add(whatSay);
   if (speaking)
    return;

   Thread runner = new Thread(run);
   runner.Start();

  }

  private static void run()
  {
   if (queue.Count > 0)
   {
    Toner drop = new Toner("Job");
    drop.go();
    Thread.Sleep(1500);
    drop.Stop();
    Thread.Sleep(100);
    SerialController.keyUp();
    speaking = true;

   }
   for (int i = 0; i < queue.Count; i++)
   {
    Stream voiceStream = new MemoryStream();
    var synthesizer = new SpeechSynthesizer();
    synthesizer.SetOutputToWaveStream(voiceStream);
    synthesizer.SelectVoice("Microsoft Eva Mobile");
    synthesizer.Speak(queue[i]);
    synthesizer.SetOutputToNull();         // <-- Finalizes WAV header
    voiceStream.Position = 0;       // <-- Critical: rewind the stream
    synthesizer.Dispose();
    using var reader = new WaveFileReader(voiceStream);

    IWaveProvider provider = reader;
    if (!reader.WaveFormat.Equals(Toner.radioBuffer.WaveFormat))
    {
     var resampler = new MediaFoundationResampler(reader, Toner.radioBuffer.WaveFormat)
     {
      ResamplerQuality = 60
     };
     provider = resampler;
    }

    byte[] temp = new byte[4096];
    int bytesRead;
    while ((bytesRead = provider.Read(temp, 0, temp.Length)) > 0)
    {
     // Flow control to avoid buffer overflow
     while (Toner.radioBuffer.BufferedBytes > Toner.radioBuffer.BufferLength * 0.75)
      Thread.Sleep(5);

     Toner.radioBuffer.AddSamples(temp, 0, bytesRead);
    }

    while (Toner.radioBuffer.BufferedBytes > 0)
     Thread.Sleep(5);

   }
   if (speaking)
   {
    queue.Clear();
    speaking = false;
    SerialController.keyDown();
   }
   
  }

  private static Dictionary<char, String> kvps = new Dictionary<char, String>()
        {
            { 'a', "Alpha" },
            { 'b', "Bravo" },
            { 'c', "Charlie" },
            { 'd', "Delta" },
            { 'e', "Echo" },
            { 'f', "Foxtrot" },
            { 'g', "Gulf" },
            { 'h', "Hotel" },
            { 'i', "India" },
            { 'j', "Juliet" },
            { 'k', "Kilo" },
            { 'l', "Lima" },
            { 'm', "Mike" },
            { 'n', "November" },
            { 'o', "Oscar" },
            { 'p', "Papa" },
            { 'q', "Quebec" },
            { 'r', "Romeo" },
            { 's', "Sierra" },
            { 't', "Tango" },
            { 'u', "Uniform" },
            { 'v', "Victor" },
            { 'w', "Whiskey" },
            { 'x', "X-Ray" },
            { 'y', "Yankee" },
            { 'z', "Zulu" },
        };

  public static string Phoenetic(string s)
  {
   string K = s.Trim().ToLower();
   string ret = "";
   foreach (char c in K)
   {
    if (c >= '0' && c <= '9')
     ret += c + " ";
    else
    {
     //Console.WriteLine("c is " + c);
     //Console.WriteLine("kvps[c] is " + kvps[c]);
     if (c >= 'a' && c <= 'z')
      ret += kvps[c] + " ";
    }

   }

   return ret;
  }

 }
}
