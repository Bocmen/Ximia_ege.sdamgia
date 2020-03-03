using System;
using System.IO;
using System.Collections.Generic;
using XimiaEGE;

namespace Ximia5
{
    class Program
    {
        static void Main(string[] args)
        {
             XimiaEGE.MainF.DownloadBD(messenge:Console.WriteLine);
           /*
            Console.WriteLine();
            if (MainF.LoadData())
            {
                Console.WriteLine("Yes");
            }
            Console.WriteLine(XimiaEGE.MainF.Bd.Patch);
            /*
            List<string> vs = new List<string>(0);
            foreach (var Elem in MainF.Bd.months[0].vars)
            {
                vs.Add(Elem.varOtvet.Id);
            }
            Console.WriteLine(MainF.GetHtmlReshenie(new List<uint>{ 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35 }, vs.ToArray(),null));
           */
            while (true) Console.ReadKey();
        }
    }
}
