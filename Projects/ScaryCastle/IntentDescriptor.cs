using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// IntentDescriptor
    /// </summary>
    public sealed class IntentDescriptor : Definition
    {
        // Constructor
        public IntentDescriptor(JsonElement element)
            : base(element)
        {
            // Category
            if (element.GetEnum<IntentCategory>("category") is not IntentCategory category)
                throw new InvalidOperationException($"Missing intent category.");
            else
                this.Category = category;

            // Phrase
            this.Phrase = element.GetString("phrase");
        }
        
        // Category
        public IntentCategory Category { get; }

        // Phrase
        public string Phrase { get; }
    }
}