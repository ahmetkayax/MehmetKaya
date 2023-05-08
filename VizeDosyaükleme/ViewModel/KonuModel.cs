using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VizeDosyaükleme.ViewModel
{
    public class KonuModel
    {
        public int konuId { get; set; }
        public string konuAdi { get; set; }
        public int dersById { get; set; }
        public int dosyaById { get; set; }
        public int konuKodu { get; set; }
    }
}