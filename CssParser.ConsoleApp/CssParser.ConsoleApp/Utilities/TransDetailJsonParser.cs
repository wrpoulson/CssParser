using System.IO;
using System.Linq;
using System.Collections.Generic;
using CssParser.ConsoleApp.Models;
using Newtonsoft.Json;
using System;

namespace CssParser.ConsoleApp.Utilities
{
  public class TransDetailJsonParser
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

    private void ParseMostRecentTransDetJsonFiles(FileInfo[] allFiles)
    {
      var fileNames = allFiles.Select(m => m.Name).ToArray();
      var uniqueFilePrecursors = Array.ConvertAll(fileNames, fileName => fileName.Substring(0, fileName.IndexOf(".css"))).Distinct();
      var mostRecentFiles = uniqueFilePrecursors.Select(m => allFiles.OrderByDescending(f => f.CreationTime).First(f => f.Name.StartsWith(m))).ToList();
      mostRecentFiles.ForEach(file => ParseTransDetJsonFile(file.FullName, file.Name));
    }

    private void ParseAllTransDetJsonFiles(FileInfo[] allFiles)
    {
      allFiles.ToList().ForEach(file => ParseTransDetJsonFile(file.FullName, file.Name));
    }

    public void ParseTransDetJsonFile(string sourceFilePath, string currentFileName)
    {
      //TODO: WRP make the thing
      List<TransDetail> transDetails = JsonConvert.DeserializeObject<List<TransDetail>>(File.ReadAllText(sourceFilePath));
    }
  }
}
