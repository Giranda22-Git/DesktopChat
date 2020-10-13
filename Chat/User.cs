using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class User
    {
        public string Name { get; set; }
        public string FavoriteColor { get; set; }
        public string ImageUrl { get; set; }


        public override string ToString()
        {
            return $"User: {Name}";
        }
    }
}
