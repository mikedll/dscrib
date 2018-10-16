using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DScrib2;
using AngleSharp.Parser.Html;
using System.Text.RegularExpressions;

namespace Tester
{
    class Program
    {

        static void Main(string[] args)
        {
            var c2 = new SearchController();
            var body = c2.GetDataLocal();
            c2.ExtractProductInfo(body);

            Console.WriteLine("Booting...");
        }
    }
}
