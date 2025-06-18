using Mysqlx;

namespace Hermes.Server.Speaker
{
    public class NewIncidentPage
    {

        public static List<NewIncidentPage> IncidentsList { get; set; } = new List<NewIncidentPage>();

        public int id;
        public string location;
        public string description;
        public bool locked = false;
        public bool changed = false;
        List<string> units = new List<string>();
        List<string> newUnits = new List<string>();

        public void addUnit(string Unit)
        {
            if (!units.Contains(Unit) && !newUnits.Contains(Unit))
            {
                newUnits.Add(Unit);
                changed = true;
            }
        }

        public override string ToString()
        {
            if (newUnits.Count == 0)
                return "";

            string ret = "";

            if (newUnits.Count > 1)
                ret += "Attention the following units for assignment... ";
            else
                ret += "Attention the following unit for assignment... ";



            foreach (string unit in newUnits) 
            { 
                units.Add(unit);
                ret += $"{Speaker.Phoenetic(unit)}, ";
            }
            newUnits.Clear();
            if (location != null)
                ret += "... Respond to. " + location;

            if (description != null)
                ret += "..... " + description;

            ret += "... Time now " + DateTime.Now.ToString("HH:mm");

            return ret;
        }

        public static void setLock(int IncidentID, bool value)
        {
            NewIncidentPage? nip = getIncident(IncidentID);
            if (nip.locked == value) return;
            if (nip.locked && !value && nip.changed)
                page(IncidentID);
            nip.locked = value;
        }

        public static void page(int IncidentID)
        {
            //Speaker.speak(getIncident(IncidentID).ToString());
        }

        public static NewIncidentPage getIncident(int IncidentID)
        {
            NewIncidentPage? nip = IncidentsList.Find(i => i.id == IncidentID);
            if (nip == null)
            {
                nip = new NewIncidentPage();
                nip.id = IncidentID;
                IncidentsList.Add(nip);
            }

            return nip;
        }

        public static void addLocation(int IncidentID, string lok)
        {
            NewIncidentPage nip = getIncident(IncidentID);
            if (nip.location == null || nip.location != lok)
            {
                nip.location = lok;
                nip.changed = true;
            }
        }

        public static void addDetails(int IncidentID, string desk)
        {
            NewIncidentPage nip = getIncident(IncidentID);
            if (nip.description == null || nip.description != desk)
            {
                nip.description = desk;
                nip.changed = true;
            }
        }

        public static void addUnit(int IncidentID,  string UnitName)
        {
            NewIncidentPage nip = getIncident(IncidentID);
            nip.addUnit(UnitName);
        }

    }
}
