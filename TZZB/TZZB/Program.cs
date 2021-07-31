using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZZB
{
    class Program
    {
        static string folderPath = @"C:\Users\yepyao\Documents\stocklist";
        static void Main(string[] args)
        {
            foreach (string filename in Directory.EnumerateFiles(folderPath))
            {
                TransferFormat(filename);
            }
        }

        static string[] TypeFilter = new string[]
        {
            "证券卖出",
            "证券买入"
        };

        static void TransferFormat(string filename)
        {
            string folder = Path.GetDirectoryName(filename);

            StreamReader reader = new StreamReader(filename, System.Text.Encoding.GetEncoding(936));

            StreamWriter writer = new StreamWriter(Path.Combine(folder, Path.GetFileNameWithoutExtension(filename)) + ".csv", false, System.Text.Encoding.GetEncoding(936));

            // read header first
            string line = reader.ReadLine();
            writer.WriteLine("成交日期, 证券代码, 证券名称, 交易类别, 成交数量, 成交价格, 发生金额, 证券余额");

            while ((line = reader.ReadLine()) != null)
            {
                string[] columns = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                columns = columns.Select(column => column.Trim().Trim('"')).ToArray();

                if (!TypeFilter.Contains(columns[4]))
                {
                    //continue;
                }
                writer.WriteLine(string.Join(", ",
                    columns[0], //成交日期
                    columns[2], //证券代码
                    columns[3], //证券名称
                    columns[4], //交易类别
                    columns[5], //成交数量
                    columns[6], //成交价格
                    columns[7], //发生金额
                    columns[18] //证券余额
                ));
            }
            reader.Close();
            writer.Close();
        }
    }
}
