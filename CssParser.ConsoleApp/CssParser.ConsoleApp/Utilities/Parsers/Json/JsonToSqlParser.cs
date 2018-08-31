using System.IO;
using System.Linq;
using System.Collections.Generic;
using CssParser.ConsoleApp.Models;
using Newtonsoft.Json;
using System;
using CssParser.ConsoleApp.Utilities.SqlQueryBuilders;
using CssParser.ConsoleApp.Utilities.SqlQueryBuilders.ClaimForm;

namespace CssParser.ConsoleApp.Utilities.Parsers.Json
{
    public class JsonToSqlParser
    {
        const string OUTPUT_PATH = @"Content\OutputSQL\";

        public void ParseTransDetJsonFiles(string sourceFilesPath, bool parseOnlyMostRecentFiles)
        {
            if (!Directory.Exists(sourceFilesPath)) throw new DirectoryNotFoundException();
            FileInfo[] allFiles = new DirectoryInfo(sourceFilesPath).GetFiles("*.TransDetails.json");
            if (parseOnlyMostRecentFiles)
            {
                ParseMostRecentTransDetJsonFiles(allFiles);
            }
            else
            {
                ParseAllTransDetJsonFiles(allFiles);
            }
        }

        public void ParseTransDetJsonFile(string sourceFilePath, string currentFileName)
        {
            List<TransDetail> transDetails = JsonConvert.DeserializeObject<List<TransDetail>>(File.ReadAllText(sourceFilePath));
            var sqlQuery = BuildSqlQuery(transDetails);
            SaveSqlQueryScript(sqlQuery, currentFileName);
        }

        private void ParseMostRecentTransDetJsonFiles(FileInfo[] allFiles)
        {
            var fileNames = allFiles.Select(m => m.Name).ToArray();
            var uniqueFilePrecursors = Array.ConvertAll(fileNames, fileName => fileName.Substring(0, fileName.IndexOf(".css"))).Distinct();
            var mostRecentFiles = uniqueFilePrecursors.Select(m => allFiles.OrderByDescending(f => f.CreationTime).First(f => f.Name.StartsWith(m))).ToList();
            mostRecentFiles.ForEach(file => ParseTransDetJsonFile(file.FullName, file.Name.Substring(0, file.Name.IndexOf(".css"))));
        }

        private void ParseAllTransDetJsonFiles(FileInfo[] allFiles)
        {
            allFiles.ToList().ForEach(file => ParseTransDetJsonFile(file.FullName, file.Name.Substring(0, file.Name.IndexOf(".css"))));
        }

        private string BuildSqlQuery(List<TransDetail> transDetails)
        {
            var transDetailUpdates = transDetails.Select(t => { return SqbTransDetail.SqlIfElseUpdateTransDet(t); });

            var ifTableExistsUpdateElseRollback = SqlQueryBuilder.IfElse(
                                                      SqlQueryBuilder.TableExistsCondition("dbo", "TRANS_DET"), // condition
                                                      $"{string.Join("\n", transDetailUpdates)}\n", // if
                                                      SqbTransDetail.PrintTableDoesNotExistRollback("TRANS_DET")); // else


            return $"{SqlQueryBuilder.Print("\t--SCRIPT START--")}\n\n{SqlQueryBuilder.Use("CODETABLES")}{ifTableExistsUpdateElseRollback}\n{SqlQueryBuilder.Print("\t--SCRIPT END--")}";
        }

        private void SaveSqlQueryScript(string query, string outputFileName)
        {
            if (!Directory.Exists(OUTPUT_PATH))
            {
                Directory.CreateDirectory(OUTPUT_PATH);
            }
            File.WriteAllText($"{OUTPUT_PATH}{outputFileName}_{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.Updates.sql", query);
        }
    }
}
