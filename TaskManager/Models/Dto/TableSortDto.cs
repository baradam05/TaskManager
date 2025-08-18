using TaskManager.Components;

namespace TaskManager.Models.Dto
{
    public class TableSortDto
    {
        public Table? orderTable = null;
        public string? sortCol = null;
        public sortDirection? sortDir = null;

        public TableSortDto() { }

        public TableSortDto(Table orderTable, string sortCol, sortDirection sortDir)
        {
            SortDirChange(ref sortDir);

            this.orderTable = orderTable;
            this.sortCol = sortCol;
            this.sortDir = sortDir;
        }

        private void SortDirChange(ref sortDirection dir)
        {
            if (dir == sortDirection.None)
                dir = sortDirection.Ascending;
            else if (dir == sortDirection.Ascending)
                dir = sortDirection.Descendin;
            else if (dir == sortDirection.Descendin)
                dir = sortDirection.None;
        }

    }

    public enum sortDirection
    {
        None = 0,
        Ascending = 1,
        Descendin = 2
    }
}
