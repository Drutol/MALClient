using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalClient.Shared.NavArgs
{
    public class StaffDetailsNaviagtionArgs
    {
        public int Id { get; set; }

        public override bool Equals(object obj)
        {
            var arg = obj as CharacterDetailsNavigationArgs;
            return arg?.Id == Id;
        }
    }
}
