using System.Reflection;
namespace MotorcycleRental.Models
{
    public static class Queues
    {
        public static string DPS_AUTHIN => "AuthenticationIn";
        public static string DPS_AUTHOUT => "AuthenticationOut";
        public static string DPS_MANAGEIN => "UserManagementIn";
        public static string DPS_MANAGEOUT => "UserManagementOut";
        public static string DPS_IN => "UserIn";
        public static string DPS_OUT => "UserOut";

        public static string MRS_MANAGEIN => "MotorcycleManagementIn";
        public static string MRS_MANAGEOUT => "MotorcycleManagementOut";
        public static string MRS_IN => "MotorcycleIn";
        public static string MRS_OUT => "MotorcycleOut";

        public static List<KeyValuePair<string, string>> All => typeof(Queues)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(string))
            .Select(p => new KeyValuePair<string, string>(p.Name, (string)p.GetValue(null)!))
            .ToList();
    }
}
