using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneNoteParser.Shared
{
    public class TableDef
    {
        bool onTable;
        int rowCount;

        List<ColumnDef> columnInfoList;

        public TableDef()
        {
            onTable = false;
            columnInfoList = new List<ColumnDef>();
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
            columnInfoList.Add(new ColumnDef());
        }

        public int GetTableColumnCount()
        {
            return columnInfoList.Count();
        }
    }
}
