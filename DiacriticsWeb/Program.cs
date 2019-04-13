using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Diacritics;

namespace DiacriticsWeb
{
    public class Program
    {
        private static string binaryFilePath = "C:/Users/emanuel.zaymus/Documents/compoundBinFile/compoundBinFile.dat";
        private static string positionTriePath = "C:/Users/emanuel.zaymus/Documents/compoundBinFile/positionTrie.txt";

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();

            //using (var w = new Reconstructor(binaryFilePath, positionTriePath))
            //{
            //    w.Reconstruct(@"D:\testovacie_texty\FileDR\odborne\astronomia\Astronomia_WITHOUT-DIACRITICS.txt", 
            //        @"D:\testovacie_texty\FileDR\odborne\astronomia\Astronomia_zrekonstruovane.txt");
            //}

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
