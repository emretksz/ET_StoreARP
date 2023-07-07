using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Enums
{
   public class Enums
    {

        //Colorları eklemek gerekecek.
       public enum Colors
        {

        }
        public enum Gender
        {
            Kız=1,
            Erkek=2
        }
        public enum Age
        {
          ÜçAltı=1,
          AltıDokuz=2,
          OnOnDört=3,
          OndörtOnSekiz=4,
        }

        public enum StockStatus
        {
            Az,
            BitmekUzere,
            Çok,
            Yok,
            UrunEklendi
        }

         public enum ShippingStasus
        {
            Acil,
            Beklemede,
            Önemli,
            Normal,
            Tamamlandı,
        }
        public enum Depocu
        {
            Beklemede,
            Hazırlanıyor,
            Tamamlandı,
        }
        public enum Orders
        {
            tamamlandı,
            iade,
            iptal
        }
    }
   
}
