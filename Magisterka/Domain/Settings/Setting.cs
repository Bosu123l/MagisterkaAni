using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Settings
{
    public static class Setting
    {
        public static int LargeDefectsKernel
        {
            get;
            set;
        }
        public static int SmallDefectsKernel
        {
            get;
            set;
        }
        public static float SmudgesMargin
        {
            get;
            set;
        }

        public static int _largeDefectsKernel;
        public static int _smallDefectsKernel;
        public static float _smudgesMargin;


        public static void GetSettings()
        {

        }

        public static void SaveSettings()
        {

        }
    }
}
