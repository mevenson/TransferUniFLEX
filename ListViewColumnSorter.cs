using System;
using System.Collections;
using System.Windows.Forms;
using System.Globalization;

namespace TransferUniFLEX
{
    /// <summary>
    /// This class is an implementation of the 'IComparer' interface.
    /// </summary>
    public class ListViewColumnSorter : IComparer
    {
        /// <summary>
        /// Specifies the column to be sorted
        /// </summary>
        private int ColumnToSort;
        /// <summary>
        /// Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private System.Windows.Forms.SortOrder OrderOfSort;
        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private CaseInsensitiveComparer ObjectCompare;
        /// <summary>
        /// Int32 comparer object
        /// </summary>
        private Comparer IntCompare;

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        /// 
        string totalLineText = "Total";

        // used by:
        //
        //  frmFixDuplicateLocationRecords_Load         has a total line with the totalLineTex = "Total"
        //  FrmInactiveAccounts_Load                    does not have a total line
        //  frmNewUpcomingInstalls_Load                 has a total line with the totalLineTex = "Total Flags"
        //  frmRecordsToInactivate_Load                 does not have a total line
        //  ReportDelinquentByCaptain_Load              does not have a total line
        //  ReportDelinquentByRoute_Load                does not have a total line
        //  frmUnPaidUpcomingHolidayParticipants_Load   has a total line with the totalLineTex = "Total"
        //  frmUnresolvedPayPalTransaction_Load         

        public ListViewColumnSorter(string _totalLineText = null)
        {
            if (_totalLineText != null)
                totalLineText = _totalLineText;

            // Initialize the column to '0'
            ColumnToSort = 0;

            // Initialize the sort order to 'none'
            OrderOfSort = System.Windows.Forms.SortOrder.None;

            // Initialize the CaseInsensitiveComparer object
            ObjectCompare = new CaseInsensitiveComparer();

            // Initialize the IntComparer object
            IntCompare = new Comparer(new CultureInfo("es-US", false));
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX, listviewY;

            // Cast the objects to be compared to ListViewItem objects
            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;

            // Compare the two items

            int listviewXValue;
            int listviewYValue;

            // This test will leave blank lines and Total lines where they are. 

            if ((string.Compare(listviewY.Text, totalLineText, true) != 0) && listviewY.Text != "")
            {
                if (Int32.TryParse(listviewX.SubItems[ColumnToSort].Text, out listviewXValue) && Int32.TryParse(listviewY.SubItems[ColumnToSort].Text, out listviewYValue))
                {
                    // do integer sort
                    compareResult = IntCompare.Compare(listviewXValue, listviewYValue);
                }
                else
                {
                    compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);
                }

                // Calculate correct return value based on object comparison
                if (OrderOfSort == System.Windows.Forms.SortOrder.Ascending)
                {
                    // Ascending sort is selected, return normal result of compare operation
                    return compareResult;
                }
                else if (OrderOfSort == System.Windows.Forms.SortOrder.Descending)
                {
                    // Descending sort is selected, return negative result of compare operation
                    return (-compareResult);
                }
                else
                {
                    // Return '0' to indicate they are equal
                    return 0;
                }
            }
            else
                return 0;
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public System.Windows.Forms.SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }

    }
}
