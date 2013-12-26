using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Graduation.Modules
{
    public static class GraduationEvents
    {
        public static void RaiseAssnChanged()
        {
            if (GraduationChanged != null)
                GraduationChanged(null, EventArgs.Empty);
        }

        public static event EventHandler GraduationChanged;
    }
}