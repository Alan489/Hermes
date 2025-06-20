﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hermes.Shared;
using NAudio.CoreAudioApi;

namespace Hermes.Server
{
 public static class Configuration
 {
  public static string Vers = "1.0.0";
  public static string Voco = "Microsoft Zira";
  public static Dictionary<string, string> units;
  public static string toneLocation;
  public static Dictionary<string, string> tones;
  public static string serialPort;
  public static MMDevice sdrTrunk;
  public static MMDevice microphone;
  public static MMDevice radioOutput;
  public static MMDevice headphoneOutput;
  public static bool useAutoDispatcher = false;
 }
}
