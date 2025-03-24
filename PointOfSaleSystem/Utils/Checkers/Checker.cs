using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;

namespace PointOfSaleSystem.Models
{
    public class Checker
    {
        public string? Check(Customer c)
        {
            if (c.Name is null || c.Name.Length == 0)
            {
                return "Tên không thể là rỗng";
            }
            else if (c.Gender is not null && !c.Gender.Equals("Nam") && !c.Gender.Equals("Nữ"))
            {
                return "Giới tính phải là nam hoặc nữ";
            }
            return null;
        }
    }
}
