using System;
using System.Media;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Hermes.Server.Speaker
{
 public class Toner
 {
  public static Toner? runningToner;
  public static Patch patch;


  private bool stop = false;
  private string myTone;
  private SoundPlayer? mySoundPlayer;
  private looper loop;

  private static WasapiCapture? micIn;
  private static WasapiCapture? sdrIn;
  private static WaveFormat format = new WaveFormat(44100, 16, 2);
  private static WasapiOut? radioOut;
  private static WasapiOut? headphoneOut;
  public static BufferedWaveProvider? radioBuffer;
  public static BufferedWaveProvider? headphoneBuffer;
  


  public Toner(string tone)
  {
   try
   {
    if (radioOut == null && tone == "SETUP")
    {
     radioBuffer = new BufferedWaveProvider(format);
     radioOut = new WasapiOut(Configuration.radioOutput, AudioClientShareMode.Shared, false, 20);
     radioOut.Init(radioBuffer);

     micIn = new WasapiCapture(Configuration.microphone);
     micIn.WaveFormat = format;
     radioBuffer.DiscardOnBufferOverflow = true;
     micIn.DataAvailable += (s, e) =>
     {
      radioBuffer.AddSamples(e.Buffer, 0, e.BytesRecorded);
     };

     headphoneBuffer = new BufferedWaveProvider(format);
     headphoneOut = new WasapiOut(Configuration.headphoneOutput, AudioClientShareMode.Shared, false, 20);
     headphoneOut.Init(headphoneBuffer);

     sdrIn = new WasapiCapture(Configuration.sdrTrunk);
     sdrIn.WaveFormat = format;
     headphoneBuffer.DiscardOnBufferOverflow = true;
     sdrIn.DataAvailable += (s, e) =>
     {
      headphoneBuffer.AddSamples(e.Buffer, 0, e.BytesRecorded);
     };

     sdrIn.StartRecording();
     headphoneOut.Play();
     radioOut.Play();
     return;
    }
   }catch(Exception ex)
   {
    Console.WriteLine(ex.ToString());
   }

   if (runningToner != null)
    runningToner.Stop();

   runningToner = this;
   myTone = tone;

   if (tone == "Transmit")
   {
    return;
   }
   string path = Configuration.toneLocation + "/" + myTone + ".wav";
   loop = new looper(path, radioBuffer);
  }

  public async Task go()
  {
   SerialController.keyUp();
   if (myTone == "Transmit")
   {
    sdrIn?.StopRecording();
    micIn?.StartRecording();
    return;
   }
   loop.StartLoop();

  }

  public void Stop()
  {
   
   SerialController.keyDown();
   if (stop)
    return;
   if (myTone == "Transmit")
   {
    sdrIn.StartRecording();
    micIn?.StopRecording();
   }
   else
    loop.StopLoop();

   radioBuffer.ClearBuffer();
   while (radioBuffer.BufferedBytes > 0)
   {
    Thread.Sleep(10); // Let audio catch up
   }
   runningToner = null;
   stop = true;
  }

 }

 public class Patch
 {
  private readonly BufferedWaveProvider _targetBuffer;
  private Thread? _loopThread;

  public Patch(BufferedWaveProvider targetBuffer)
  {
   _targetBuffer = targetBuffer;
  }

  public void StartLoop()
  {
   if (_loopThread != null && _loopThread.IsAlive)
    return; // Already looping
  }

 }

 public class looper
 {
  private readonly string _filePath;
  private readonly BufferedWaveProvider _targetBuffer;
  private Thread? _loopThread;
  private volatile bool _shouldLoop;

  public looper(string filePath, BufferedWaveProvider targetBuffer)
  {
   _filePath = filePath;
   _targetBuffer = targetBuffer;
  }

  public void StartLoop()
  {
   if (_loopThread != null && _loopThread.IsAlive)
    return; // Already looping

   _shouldLoop = true;
   _loopThread = new Thread(LoopPlayback);
   _loopThread.IsBackground = true;
   _loopThread.Start();
  }

  public void StopLoop()
  {
   _shouldLoop = false;
   //_loopThread?.Join();
  }

  private void LoopPlayback()
  {
   while (_shouldLoop)
   {
    using var reader = new AudioFileReader(_filePath);

    // Resample to match buffer format
    using var resampler = new MediaFoundationResampler(reader, _targetBuffer.WaveFormat)
    {
     ResamplerQuality = 60 // max quality
    };

    var tempBuffer = new byte[4096];
    int bytesRead;

    while (_shouldLoop && (bytesRead = resampler.Read(tempBuffer, 0, tempBuffer.Length)) > 0)
    {
     while (_targetBuffer.BufferedBytes > _targetBuffer.BufferLength / 2 && _shouldLoop)
     {
      Thread.Sleep(10); // Let audio catch up
     }
     _targetBuffer.AddSamples(tempBuffer, 0, bytesRead);
    }

    Thread.Sleep(10);
   }
  }

  public void Dispose()
  {
   StopLoop();
  }
 }
}
