using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public static class SESSION
    {
        
        public static string UserRole { get; set; }
       

        public static void Clear()
        {
            
            UserRole = string.Empty;
           
        }
    }
}
