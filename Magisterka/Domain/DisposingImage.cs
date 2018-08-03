using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;


namespace Domain
{
    public class DisposingImage<TColor, TDepth> : Image<TColor, TDepth>, IDisposable 
        where TColor : struct, IColor
        where TDepth : new()
    {
        public void Dispose()
        {
            
        }
    }
}
