using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using Snapshooter;
using Snapshooter.Xunit;
using CycloneDX.CLI.Models;

namespace CycloneDX.CLI.Tests
{
    public class ConvertTests
    {
        [Theory]
        [InlineData("bom-1.0.xml", InputFormat.autodetect, "bom.xml", Commands.ConvertOutputFormat.autodetect)]
        [InlineData("bom-1.0.xml", InputFormat.xml, "bom.xml", Commands.ConvertOutputFormat.autodetect)]
        [InlineData("bom-1.0.xml", InputFormat.xml, "bom.xml", Commands.ConvertOutputFormat.xml_v1_1)]
        [InlineData("bom-1.1.xml", InputFormat.autodetect, "bom.xml", Commands.ConvertOutputFormat.autodetect)]
        [InlineData("bom-1.1.xml", InputFormat.xml, "bom.xml", Commands.ConvertOutputFormat.autodetect)]
        [InlineData("bom-1.1.xml", InputFormat.xml, "bom.xml", Commands.ConvertOutputFormat.xml_v1_1)]
        [InlineData("bom-1.2.xml", InputFormat.autodetect, "bom.xml", Commands.ConvertOutputFormat.autodetect)]
        [InlineData("bom-1.2.xml", InputFormat.xml, "bom.xml", Commands.ConvertOutputFormat.autodetect)]
        [InlineData("bom-1.2.xml", InputFormat.xml, "bom.xml", Commands.ConvertOutputFormat.xml)]
        [InlineData("bom-1.2.xml", InputFormat.xml, "bom.xml", Commands.ConvertOutputFormat.xml_v1_2)]
        [InlineData("bom-1.2.json", InputFormat.autodetect, "bom.json", Commands.ConvertOutputFormat.autodetect)]
        [InlineData("bom-1.2.json", InputFormat.json, "bom.json", Commands.ConvertOutputFormat.autodetect)]
        [InlineData("bom-1.2.json", InputFormat.json, "bom.json", Commands.ConvertOutputFormat.json)]
        [InlineData("bom.csv", InputFormat.autodetect, "bom.csv", Commands.ConvertOutputFormat.autodetect)]
        [InlineData("bom.csv", InputFormat.csv, "bom.csv", Commands.ConvertOutputFormat.autodetect)]
        [InlineData("bom.csv", InputFormat.csv, "bom.csv", Commands.ConvertOutputFormat.csv)]
        public async Task Convert(string inputFilename, InputFormat inputFormat, string outputFilename, Commands.ConvertOutputFormat outputFormat)
        {
            using (var tempDirectory = new TempDirectory())
            {
                var fullOutputPath = Path.Join(tempDirectory.DirectoryPath, outputFilename);
                var exitCode = await Program.Convert(
                    Path.Combine("Resources", inputFilename),
                    fullOutputPath,
                    inputFormat,
                    Commands.ConvertOutputFormat.autodetect);
                
                Assert.Equal(0, exitCode);
                var bom = File.ReadAllText(fullOutputPath);
                Snapshot.Match(bom, SnapshotNameExtension.Create(inputFilename, inputFormat, outputFilename, outputFormat));
            }
        }

        [Theory]
        [InlineData(Commands.ConvertOutputFormat.autodetect)]
        [InlineData(Commands.ConvertOutputFormat.spdxtag_v2_1)]
        [InlineData(Commands.ConvertOutputFormat.spdxtag_v2_2)]
        public async Task ConvertToSpdxTag(Commands.ConvertOutputFormat outputFormat)
        {
            using (var tempDirectory = new TempDirectory())
            {
                var outputFilename = Path.Combine(tempDirectory.DirectoryPath, "bom.spdx");
                var exitCode = await Program.Convert(
                    Path.Combine("Resources", "bom-1.2.xml"),
                    outputFilename,
                    Models.InputFormat.xml,
                    outputFormat);
                
                Assert.Equal(0, exitCode);
                var bom = File.ReadAllText(outputFilename);
                bom = Regex.Replace(bom, @"Created: .*\n", "");
                Snapshot.Match(bom, SnapshotNameExtension.Create(outputFormat));
            }
        }
    }
}