﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onenote2md.Shared
{
    public interface IGenerator
    {
        MarkdownPage GenerateMD(string parentId);
    }
}