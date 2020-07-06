using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneNoteParser.Shared
{
    public class ColumnInfo
    {
    }

    public class TableInfo
    {
        bool onTable;
        int rowCount;

        List<ColumnInfo> columnInfoList;

        public TableInfo()
        {
            onTable = false;
            columnInfoList = new List<ColumnInfo>();
            rowCount = 0;
        }

        public void SetOnTable()
        {
            onTable = true;
        }


        public void Reset()
        {
            onTable = false;
            columnInfoList.Clear();
            rowCount = 0;
        }

        public void AppendRow()
        {
            rowCount++;
        }

       
        public bool OnHeaderRow()
        {
            if (rowCount == 1)
                return true;
            else
                return false;
        }

        public bool IsOnTable()
        {
            return onTable;
        }

        public void AppendTableColumn()
        {
            columnInfoList.Add(new ColumnInfo());
        }

        public int GetTableColumnCount()
        {
            return columnInfoList.Count();
        }

        //public MarkdownContent GetHeaderMD()
        //{
        //    if (String.IsNullOrEmpty(Name))
        //        return MarkdownContent.Empty();
        //    else
        //    {
        //        switch (Name)
        //        {
        //            case "To Do":
        //                return MarkdownContent.SingleContent(" [ ] ");

        //            case "Important":
        //                return MarkdownContent.SingleContent(":star: ");

        //            case "Question":
        //                return MarkdownContent.SingleContent(":question: ");

        //            case "Critical":
        //                return MarkdownContent.SingleContent(":exclamation: ");



        //            default:
        //                return MarkdownContent.SingleContent(":red_circle: ");
        //        }
        //    }

        //}
    }
}
