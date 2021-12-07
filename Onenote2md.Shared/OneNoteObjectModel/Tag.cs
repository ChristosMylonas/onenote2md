namespace Onenote2md.Shared.OneNoteObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public partial class Tag
    {
        public string GetMD(TagDef tagDef)
        {
            if (tagDef == null)
            {
                return String.Empty;
            }

            switch (tagDef.type)
            {
                case TagDefType.ToDo:
                    if (this.completed)
                    {
                        return "-  [x] ";
                    }
                    else
                    {
                        return "-  [ ] ";
                    }
                case TagDefType.Star:
                    return ":star:";
                case TagDefType.Question:
                    return ":question:";
                case TagDefType.Critical:
                    return ":exclamation:";
                default:
                    return ":red_circle:";
            }
        }
    }
}
