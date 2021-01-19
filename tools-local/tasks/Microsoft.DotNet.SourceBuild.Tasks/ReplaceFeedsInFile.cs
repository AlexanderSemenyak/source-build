// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Microsoft.DotNet.Build.Tasks
{
    /// <summary>
    /// Replaces feeds in a file given a mapping
    /// of old feeds to new feeds.
    /// </summary>
    public class ReplaceFeedsInFile : Task
    {
        /// <summary>
        /// The file in which to replace feeds.
        /// </summary>
        [Required]
        public string InputFile { get; set; }

        /// <summary>
        /// An item group of feeds to update.
        /// %(Identity): The feed URL to find in the input file.
        /// %(NewFeed): The feed URL to replace %(Identity) with.
        /// </summary>
        [Required]
        public ITaskItem[] FeedMapping { get; set; }

        public override bool Execute()
        {
            string fileContents = File.ReadAllText(InputFile);
            bool updated = false;

            Log.LogMessage(
                MessageImportance.High,
                $"Searching for feeds in {InputFile}");

            foreach (var feed in FeedMapping)
            {
                string oldFeed = feed.ItemSpec;
                string newFeed = feed.GetMetadata("NewFeed");

                if (fileContents.Contains(oldFeed))
                {
                    Log.LogMessage(
                        MessageImportance.High,
                        $"Replacing {oldFeed} with {newFeed}");
                    fileContents = fileContents.Replace(oldFeed, newFeed);
                    updated = true;
                }
            }

            if (updated) File.WriteAllText(InputFile, fileContents);

            return true;
        }
    }
}
