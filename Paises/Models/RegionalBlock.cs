﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paises.Modelos
{
    public class RegionalBlock
    {
        public string acronym { get; set; }
        public string name { get; set; }
        public List<object> otherAcronyms { get; set; }
        public List<object> otherNames { get; set; }
    }
}
