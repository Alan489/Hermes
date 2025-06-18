using Microsoft.AspNetCore.ResponseCompression;
using Hermes.Server;
using Hermes.Shared;
using System.Configuration;
using Hermes.Server.Speaker;
using System.Net.Http;
using Configuration = Hermes.Server.Configuration;

namespace Hermes
{
    public class Program
    {

        private static char[] frames = { '|', '/', '-', '\\'};
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
            Configuration.tones = app.Configuration.GetSection("Tones").Get<Dictionary<string,string>>();
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
                tb = tb.Replace('>', frames[i] );


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