using Microsoft.AspNetCore.Mvc;
using Hermes.Shared;
using Hermes.Server.LocationServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hermes.Server.Speaker;

namespace Hermes.Server.Controllers
{
 [ApiController]
 [Route("/API/[controller]")]

 public class ToneoutController : ControllerBase
 {

  [HttpGet("{u}/{pw}/get")]
  public async Task<Dictionary<string, string>?> Get(string? u, string? pw)
  {
   if (string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(u))
    return null;

   if (!validate(u, pw))
    return null;

   

   return Configuration.tones;
  }

  [HttpGet("{u}/{pw}/start/{tone}")]
  public async Task startTone(string? u, string? pw, string? tone)
  {
   if (string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(u))
    return;

   if (!validate(u, pw))
    return;

   if (tone == null)
    return;

   new Toner(tone);
   await Toner.runningToner?.go();


   return;
  }

  [HttpGet("{u}/{pw}/stop")]
  public async Task stopTone(string? u, string? pw)
  {
   if (string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(u))
    return;

   if (!validate(u, pw))
    return;

   if (Toner.runningToner != null)
    Toner.runningToner.Stop();


   return;
  }

  [HttpGet("{u}/{pw}/page/{inc}")]
  public async Task pageout(string? u, string? pw, string inc)
  {
   CurrentAssignmentsController.currentPriorities.Add(inc);
   if (string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(u))
    return;

   if (!validate(u, pw))
    return;

   NewIncidentPage.page(inc);


   return;
  }

  private bool validate(string u, string p)
  {
   string? ret = null;
   Configuration.units.TryGetValue(u, out ret);

   if (ret == null || ret != p)
    return false;

   return true;
  }
 }
}