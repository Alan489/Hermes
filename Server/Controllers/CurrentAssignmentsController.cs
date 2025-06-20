using Microsoft.AspNetCore.Mvc;
using Hermes.Shared;
using Hermes.Server.LocationServices;
using System.Collections.Generic;

namespace Hermes.Server.Controllers
{
 [ApiController]
 [Route("/API/[controller]")]

 public class CurrentAssignmentsController : ControllerBase
 {

  public static List<string> currentPriorities = new List<string>();

  [HttpGet("g/{u}/{pw}/{lat}/{longa}")]
  public async Task<List<Assignment>?> Get(string? u, string? pw, string? lat, string? longa)
  {
   if (string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(u))
    return null;

   if (!validate(u, pw))
    return null;

   if (lat != null && longa != null && lat != "X" && longa != "X")
   {
    if (Locations.locations.ContainsKey(u)) Locations.locations.Remove(u);
    Locations.locations.Add(u, [lat, longa, DateTime.Now.ToString("MM/dd/yy HH:mm")]);
   }

   return await DBInterface.getCurrentAssignments();
  }

  [HttpGet("getprios/{u}/{pw}")]
  public async Task<List<Priority>?> GetPrios(string? u, string? pw)
  {
   if (string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(u) || u != "Board")
    return null;

   if (!validate(u, pw))
    return null;

   if (currentPriorities.Count == 0) return null;

   List<Assignment> assignments = await DBInterface.getCurrentAssignments();
   List<Priority> priorities = assignments
    .Where(ass => currentPriorities.Contains(ass.call_number))
    .GroupBy(ass => ass.call_number)
    .Select(g =>
    {
     var prio = g.First().convertToPriority();
     foreach (var ass in g.Skip(1))
     {
      prio.unit.Add(ass.unit);
     }
     return prio;
    })
    .ToList();

   currentPriorities.Clear();

   return priorities;
  }

  [HttpGet("{u}/{pw}/{ass}/{col}")]
  public async Task GetUpdateTime(string? u, string? pw, string? ass, string? col)
  {
   if (string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(u) || string.IsNullOrEmpty(col) || string.IsNullOrEmpty(ass))
    return;

   if (!validate(u, pw))
    return;

   await DBInterface.updateUnit(ass, col, u);

   return;
  }
  [HttpGet("locations/{u}/{pw}")]
  public Dictionary<string, string[]>? GetLocations(string? u, string? pw)
  {
   if (string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(u))
    return null;

   if (!validate(u, pw))
    return null;

   return Locations.locations;
  }

  [HttpGet("calllocations/{u}/{pw}")]
  public List<string[]>? GetCallLocations(string? u, string? pw)
  {
   if (string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(u))
    return null;

   if (!validate(u, pw))
    return null;

   return DBInterface.getCallLocations();
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