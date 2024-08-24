using System.Reflection;
namespace MotorcycleRental.Models
{
    public static class Commands
    {
        public static string AUTHENTICATE => "authenticate";
        public static string CREATEVEHICLE => "createvehicle";
        public static string DELETEVEHICLE => "deletevehicle";
        public static string EDITVIN => "editvin";
        public static string LISTVEHICLES => "listvehicles";
        public static string NOTIFY => "notify";
        public static string CREATEUSER => "createuser";
        public static string UPLOADCNHIMAGE => "uploadcnhimg";
        public static string LISTPLANS => "listplans";
        public static string HIREVEHICLE => "hirevehicle";
        public static string RETURNVEHICLE => "returnvehicle";
        public static string LISTRENTALS => "listuserrentals";
        
        public static List<KeyValuePair<string, string>> All => typeof(Commands)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(string))
            .Select(p => new KeyValuePair<string, string>(p.Name, (string)p.GetValue(null)!))
            .ToList();
    }
}
