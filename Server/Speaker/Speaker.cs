using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using Hermes.Server;

namespace Hermes.Server.Speaker
{
    public class Speaker
    {
        private static bool speaking = false;
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
            speaking = true;
            for (int i = 0 ; i < queue.Count; i++)
            {
                Stream voiceStream = new MemoryStream();
                var synthesizer = new SpeechSynthesizer();
                synthesizer.SelectVoice("Microsoft Eva Mobile");
                synthesizer.Speak(queue[i]);
                synthesizer.Dispose();
            }
            queue.Clear();
            speaking = false;
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
