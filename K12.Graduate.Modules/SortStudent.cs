using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Graduation.Modules
{
    static public class SortStudent
    {
        public static List<StudentRecord> sort(List<StudentRecord> list)
        {
            List<StudentRecord> _list = list;
            _list.Sort(SortK12Student);
            return _list;
        }


        private static int SortK12Student(StudentRecord x, StudentRecord y)
        {
            string student1 = "";
            string student2 = "";

            if (x.Class != null)
            {
                student1 += x.Class.Name.PadLeft(8, '0');
            }
            else
            {
                student1 += "00000000";
            }

            student1 += x.SeatNo.HasValue ? x.SeatNo.Value.ToString().PadLeft(8, '0') : "00000000";

            if (y.Class != null)
            {
                student2 += y.Class.Name.PadLeft(8, '0');
            }
            else
            {
                student2 += "00000000";
            }

            student2 += y.SeatNo.HasValue ? y.SeatNo.Value.ToString().PadLeft(8, '0') : "00000000";

            return student1.CompareTo(student2);

        }
    }
}
