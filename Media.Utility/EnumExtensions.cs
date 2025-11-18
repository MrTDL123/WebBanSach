using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Media.Utility
{
    public static class EnumExtensions
    {
        // Hàm này phải là 'static'
        public static string GetDescription(this Enum value)
        {
            try
            {
                // 1. Lấy thông tin 'Field' của giá trị enum (ví dụ: 'ChoXuLy')
                FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

                if (fieldInfo == null)
                    return value.ToString(); // Trả về tên nếu không tìm thấy

                // 2. Lấy thuộc tính [Description] từ field đó
                var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false
                );

                // 3. Trả về text của Description nếu tìm thấy,
                //    nếu không thì trả về tên của enum
                return attributes.Length > 0 ? attributes[0].Description : value.ToString();
            }
            catch
            {
                return value.ToString(); // Trả về tên nếu có lỗi
            }
        }
    }
}