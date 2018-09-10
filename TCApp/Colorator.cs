using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TCApp
{
    public class Colorator     // envelope for colored text
    {
        public string text;
        public Color color;
        public Colorator(string text, Color color)
        {
            this.text = text;
            this.color = color;
        }
    }
}
