﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ProjectFinder.Model;

namespace ProjectFinder.Utilities
{
    public class SlnParser
    {
        public Dictionary<string, string> ParseSolutionFile()
        {
            return ParseSolutionFile(slnFileInfo);
        }

        public static Dictionary<string, string> ParseSolutionFile(FileCacheRecord slnFile)
        {
            if (slnFile == null)
            {
                throw new ArgumentNullException(nameof(slnFile));
            }

            var projects = new Dictionary<string, string> { { "root", slnFile.Path } };

            var content = File.ReadAllText(slnFile.FilePath);
            var query = from match in Regex.Matches(content, @"(?<=Project.*=\s).*(?=,.*?\r?\nEndProject)", RegexOptions.IgnoreCase)
                        let m = match.Value.Split(',')[1]
                        select m.Replace("\\", "/")
                                .Replace("\"", string.Empty)
                                .Trim();
            foreach (var item in query)
            {
                var proj = new FileInfo(Path.Combine(slnFile.Path, item));
                projects.Add(proj.Name.Substring(0, proj.Name.LastIndexOf('.')).ToLowerInvariant(), proj.DirectoryName);
            }
            return projects;
        }

        private readonly FileCacheRecord slnFileInfo;
        public SlnParser(FileCacheRecord slnFileInfo)
        {
            this.slnFileInfo = slnFileInfo;
        }
    }
}
