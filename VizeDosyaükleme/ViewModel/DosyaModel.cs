using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VizeDosyaükleme.ViewModel
{
    public class DosyaModel
    {
        public int dosyaId { get; set; }
        public string dosyaAdi { get; set; }
        public string dosyaYolu { get; set; }
        public int konuByID { get; set; }
        public int dosyaKodu { get; set; }
    }
}