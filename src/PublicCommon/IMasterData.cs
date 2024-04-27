using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicCommon
    {
    public interface IMasterData
        {
        public int Id { get; set; }
        public int TypeId { get; set; }//TownItemType,SystemType...
        public string Name { get; set; }
        }
    }
