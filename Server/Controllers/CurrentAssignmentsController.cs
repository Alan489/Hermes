using Microsoft.AspNetCore.Mvc;
using Hermes.Shared;
using Hermes.Server.LocationServices;

namespace Hermes.Server.Controllers
{
    [ApiController]
    [Route("/API/[controller]")]

    public class CurrentAssignmentsController : ControllerBase
    {

        [HttpGet("g/{u}/{pw}/{lat}/{longa}")]
        public async Task<List<Assignment>?> Get(string? u, string? pw, string? lat, string? longa)
        {
            if (string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(u))
                return null;

            if (!validate(u,pw))
                return null;
            
            if (lat != null && longa != null && lat != "X" && longa != "X")
            {
                if (Locations.locations.ContainsKey(u)) Locations.locations.Remove(u);
                Locations.locations.Add(u, [lat,longa,DateTime.Now.ToString("MM/dd/yy HH:mm")]);
            }

            return await DBInterface.getCurrentAssignments();
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