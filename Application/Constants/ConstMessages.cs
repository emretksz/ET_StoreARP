using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Constants
{
   public static class ConstMessages
    {

        public static string ProductAdded = "Ürün Eklendi";
        public static string ProductError = "Ürün Eklenemedi";
        public static string ProductRemoveError = "Ürün Silinirken Hata oluştu!";
        public static string ProductNameInvalid = "Ürün ismi geçersiz";
        public static string MaintenanceTime = "Bakım Zamanı";
        public static string ProductsListed = "Ürünler Listelendi"; 
        public static string UserAdded = "Kullanıcı Eklendi";
        public static string GetUser = "Kullanıcı Getirildi";
        public static string GetUserList = "Kullanıcılar Listelendi";
        public static string UserDelete = "Kullanıcı Silindi";
        public static string UserUpdate = "Kullanıcı Bilgileri Güncellendi";
        public const string DateOnlyFormat = "{0:MM/dd/yyyy}";
    }
}
