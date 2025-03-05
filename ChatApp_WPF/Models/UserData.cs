using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp_WPF.Models
{
    internal class UserData
    {
        private static string UserName;
        private static bool GenderMale;
        private static bool GenderFemale;
        public static void SetUserName(string name)
        {
            UserName = name;
        }
        public static void SetGender(string gender)
        {
            //Female : 0,Male : 1
            if(gender == "male"||gender == "Male")
            {
                GenderMale = true;
            }
            else if(gender == "Female" || gender == "female")
            {
                GenderFemale = true;
            }
        }
        public static string GetGender()
        {
            if (GenderMale = true)
            {
                return "Male";
            }
            else if (GenderFemale = true)
            {
                return "Female";
            }
            return "";
        }
        public static string GetUserName()
        {
            return UserName;
        }
    }
}
