using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class StockListDto: IDto
    {
        public string ModelCode { get; set; }
        public string ModelName { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Renk { get; set; }
        public long Miktar { get; set; }
        public string RenkBarcode { get; set; }
        public long Counts { get; set; }

    }
}
