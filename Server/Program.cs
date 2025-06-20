using Microsoft.AspNetCore.ResponseCompression;
using Hermes.Server;
using Hermes.Shared;
using System.Configuration;
using Hermes.Server.Speaker;
using System.Net.Http;
using Configuration = Hermes.Server.Configuration;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.IO.Ports;

namespace Hermes
{
 public class Program
 {

  private static char[] frames = { '|', '/', '-', '\\' };
  private static string[] framesS = {
            "|>     Send It      |",
            "| >    Send It      |",
            "|> >   Send It      |",
            "| > >  Send It      |",
            "|> > > Send It      |",
            "| > > > end It      |",
            "|  > > > nd It      |",
            "|   > > > d It      |",
            "|    > > >  It      |",
            "|     > > > It      |",
            "|      > > > t      |",
            "|       > > >       |",
            "|        > > >      |",
            "|t        > > >     |",
            "|It        > > >    |",
            "| It        > > >   |",
            "|d It        > > >  |",
            "|nd It        > > > |",
            "|end It        > > >|",
            "|Send It        > > |",
            "| Send It        > >|",
            "|  Send It        > |",
            "|   Send It        >|",
            "|    Send It        |",
            "|     Send It       |",
        };

  public static void Main(string[] args)
  {

   Console.WriteLine("    __  ___    _____   ______   __________  __  _____  ________\r\n   / / / / |  / /   | / ____/  / ____/ __ \\/  |/  /  |/  / ___/\r\n  / /_/ /| | / / /| |/ /      / /   / / / / /|_/ / /|_/ /\\__ \\ \r\n / __  / | |/ / ___ / /___   / /___/ /_/ / /  / / /  / /___/ / \r\n/_/ /_/  |___/_/  |_\\____/   \\____/\\____/_/  /_/_/  /_//____/  \r\n                                                               ");
   Console.WriteLine("");
   Console.WriteLine("CONFIGURATION\n-----------\n");

   var enumerator = new MMDeviceEnumerator();

   // List input devices
   Console.WriteLine("Input Devices:");
   MMDeviceCollection inputDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
   for (int ia = 0; ia < inputDevices.Count; ia++)
   {
    Console.WriteLine($"{ia}: {inputDevices[ia].FriendlyName}");
   }
   Console.WriteLine("-----------");

   // Select input
   Console.Write("Select SDR Trunk endpoint: ");
   int inputIndex = int.Parse(Console.ReadLine());
   Configuration.sdrTrunk = inputDevices[inputIndex];

   Console.Write("Select Microphone endpoint: ");
   inputIndex = int.Parse(Console.ReadLine());
   Configuration.microphone = inputDevices[inputIndex];
   Console.WriteLine("-----------");

   Console.WriteLine("\nOutput Devices:");
   var outputDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
   for (int ia = 0; ia < outputDevices.Count; ia++)
   {
    Console.WriteLine($"{ia}: {outputDevices[ia].FriendlyName}");
   }
   Console.WriteLine("-----------");

   Console.Write("Select Radio endpoint: ");
   inputIndex = int.Parse(Console.ReadLine());
   Configuration.radioOutput = outputDevices[inputIndex];

   Console.Write("Select Headphone endpoint: ");
   inputIndex = int.Parse(Console.ReadLine());
   Configuration.headphoneOutput = outputDevices[inputIndex];

   Console.WriteLine("-----------");

   Console.WriteLine("Serial Ports:");
   string[] ports = SerialPort.GetPortNames();
   for (int ia = 0; ia < ports.Length; ia++)
   {
    Console.WriteLine($"{ia}: {ports[ia]}");
   }
   Console.WriteLine("-----------");

   Console.Write("Select Key Bridge Com Port: ");
   inputIndex = int.Parse(Console.ReadLine());
   Configuration.serialPort = ports[inputIndex];

   Console.WriteLine("-----------");

   Console.WriteLine("Use Auto Dispatcher? [y/n]");
   ConsoleKeyInfo input = Console.ReadKey();
 
   if (input.KeyChar == 'y')
   {
    Console.WriteLine("Using auto dispatcher.");
    Configuration.useAutoDispatcher = true;
   }

   Thread.Sleep(5000);

   new Toner("SETUP");

   var builder = WebApplication.CreateBuilder(args);

   // Add services to the container.

   builder.Services.AddControllersWithViews();
   builder.Services.AddRazorPages();

   var app = builder.Build();

   // Configure the HTTP request pipeline.
   if (app.Environment.IsDevelopment())
   {
    app.UseWebAssemblyDebugging();
   }
   else
   {
    app.UseExceptionHandler("/Error");
   }

   Configuration.units = app.Configuration.GetSection("UnitLogins").Get<Dictionary<string, string>>();
   Configuration.tones = app.Configuration.GetSection("Tones").Get<Dictionary<string, string>>();
   Configuration.toneLocation = app.Configuration.GetValue<string>("ToneLocation");
   DBInterface.connect(app.Configuration.GetValue<string>("ConnectionString"), new HttpClient());

   app.UseBlazorFrameworkFiles();
   app.UseStaticFiles();


   app.UseRouting();


   app.MapRazorPages();
   app.MapControllers();
   app.MapFallbackToFile("index.html");

   Thread t = new Thread(app.Run);
   t.Start();
   int i = 0;
   int i2 = 0;

   Thread.Sleep(5000);

   Console.Clear();
   Console.WriteLine("    __  ___    _____   ______   __________  __  _____  ________\r\n   / / / / |  / /   | / ____/  / ____/ __ \\/  |/  /  |/  / ___/\r\n  / /_/ /| | / / /| |/ /      / /   / / / / /|_/ / /|_/ /\\__ \\ \r\n / __  / | |/ / ___ / /___   / /___/ /_/ / /  / / /  / /___/ / \r\n/_/ /_/  |___/_/  |_\\____/   \\____/\\____/_/  /_/_/  /_//____/  \r\n                                                               ");
   Console.CursorVisible = false;

   while (true)
   {

    Console.SetCursorPosition(22, 6);
    if (i >= frames.Length)
     i = 0;
    if (i2 >= framesS.Length)
     i2 = 0;
    string tb = framesS[i2];
    tb = tb.Replace('>', frames[i]);


    Console.Write(tb);

    //Console.SetCursorPosition(33,7);
    //if (i >= frames.Length)
    //    i = 0;
    //Console.Write(frames[i]);
    i++;
    i2++;
    Thread.Sleep(100);

   }

  }
 }
}