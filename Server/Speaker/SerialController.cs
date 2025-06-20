using System.IO.Ports;

namespace Hermes.Server.Speaker
{
 public class SerialController
 {
  static SerialPort? serialPort;

  private static bool newConnection()
  {
   if (serialPort != null)
   {
    try
    {
     serialPort.Close();
     serialPort.Dispose();
    } catch (Exception ex) 
    { 
     serialPort = null;
    }
   }

   try
   {
    serialPort = new SerialPort(Configuration.serialPort, 9600);
    serialPort.Open();
    return true;
   }
   catch (Exception ex)
   {
    serialPort = null;
   }
   return false;
  }

  public static void keyUp()
  {
   if (serialPort == null)
    if (!newConnection())
     return;
   try
   {
    serialPort.Write("1");
    if (!newConnection())
     return;
    serialPort.Write("1");
   }
   catch (Exception ex) {
    if (!newConnection())
     return;
    serialPort.Write("1");
   }
   
  }

  public static void keyDown()
  {
   if (serialPort == null)
    if (!newConnection())
     return;

   try
   {
    serialPort.Write("0");
    
   }
   catch (Exception ex) {
    if (!newConnection())
     return;
    serialPort.Write("0");
   }
  }

 }
}
