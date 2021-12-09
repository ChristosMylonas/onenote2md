namespace Onenote2md.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class MDGeneratorOptions
    {
        public string RootOutputDirectory { get; set; }

        public string AttachmentSubDir { get; set; } = "assets";

        public AttachmentLocation AttachmentLocation { get; set; } = AttachmentLocation.SubDir;

        public bool Overwrite { get; set; } = true;
    }
}
