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

        static string outputHeader = "成交日期, 证券代码, 证券名称, 交易类别, 成交数量, 成交价格, 发生金额, 证券余额, 佣金, 税费";

        static void Main(string[] args)
        {
            Directory.Delete(Path.Combine(folderPath, "output"), true);
            Directory.CreateDirectory(Path.Combine(folderPath, "output"));

            StreamWriter streamWriter = new StreamWriter(Path.Combine(folderPath, "output", "all.csv"), false, System.Text.Encoding.GetEncoding(936));
            streamWriter.WriteLine(outputHeader);

            foreach (string filename in Directory.EnumerateFiles(folderPath))
            {
                TransferFormat(filename, streamWriter);
            }
            streamWriter.Close();
        }

        static string[] TypeFilter = new string[]
        {
            "证券卖出",
            "证券买入",
            "新股入帐",
            "转托转入"
        };

        static void TransferFormat(string filename, StreamWriter streamWriter)
        {
            string folder = Path.GetDirectoryName(filename);

            StreamReader reader = new StreamReader(filename, System.Text.Encoding.GetEncoding(936));

            StreamWriter writer = new StreamWriter(Path.Combine(folder, "output", Path.GetFileNameWithoutExtension(filename)) + ".csv", false, System.Text.Encoding.GetEncoding(936));

            // read header first
            string line = reader.ReadLine();
            writer.WriteLine(outputHeader);

            while ((line = reader.ReadLine()) != null)
            {
                string[] columns = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                columns = columns.Select(column => column.Trim().Trim('"')).ToArray();

                if (!TypeFilter.Contains(columns[4]))
                {
                    continue;
                }


                if (columns[2].StartsWith("11") || columns[2].StartsWith("132"))
                {
                    columns[5] += "0";
                }

                string calcAmount = (Math.Abs(int.Parse(columns[5])) * double.Parse(columns[6])).ToString("F3");

                if (columns[4] == "新股入帐")
                {
                    columns[4] = "证券买入";
                    columns[7] = calcAmount;
                }

                if (columns[4] == "转托转入")
                {
                    columns[4] = "证券买入";
                    columns[7] = calcAmount;
                }

                bool attentionNeed = false;
                if (columns[7] != calcAmount)
                {
                    double calcAmountParsed = double.Parse(calcAmount);
                    double realAmountParsed = double.Parse(columns[7]);

                    if (Math.Abs(calcAmountParsed - realAmountParsed) / Math.Min(calcAmountParsed, realAmountParsed) > 0.03)
                    {
                        attentionNeed = true;
                    }
                }

                if (attentionNeed)
                {
                    Console.WriteLine("!!! " + line);
                    Console.WriteLine("\tcalc:\t" + calcAmount + "\treal:\t" + columns[7]);
                }

                string outline = string.Join(", ",
                    columns[0], //成交日期
                    columns[2], //证券代码
                    columns[3], //证券名称
                    columns[4], //交易类别
                    columns[5], //成交数量
                    columns[6], //成交价格
                    columns[7], //发生金额
                    columns[18], //证券余额
                    columns[8], //佣金
                    (double.Parse(columns[9])+double.Parse(columns[10])).ToString("F3") //税费
                );
                writer.WriteLine(outline);
                streamWriter.WriteLine(outline);
            }
            reader.Close();
            writer.Close();
        }
    }
}
