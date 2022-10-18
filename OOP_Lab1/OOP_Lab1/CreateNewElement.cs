using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace OOP_Lab_01
{
    class CreateNewElement
    {
        const int b = 65;
        const int c = 26;
        char letter = 'A';
        char firstLetter = 'A';
        int firstLetterN = 1;
        string temp;
        int r;
        public void AddColumn(DataGridView dgv, int columnsCount)
        {
            if (columnsCount < c)
            {
                letter = (char)(b + columnsCount);
                temp += letter;
            }
            else
            {
                int y = 0;
                while (columnsCount >= c)
                {
                    columnsCount -= c;
                    ++y;
                }
                firstLetter = (char)(y + b - 1);
                letter = (char)(columnsCount + b);
                temp += firstLetter;
                temp += letter;
            }

            DataGridViewColumn newColumn = (DataGridViewColumn)dgv.Columns[0].Clone();
            newColumn.HeaderCell.Value = temp;

            dgv.Columns.Add(newColumn);
            temp = null;

            if (firstLetter != 'Z')
            {
                if (letter != 'Z')
                    letter++;
                else
                    letter = 'A';
            }
            else
            {
                firstLetter = 'A';
                firstLetterN++;
            }
        }



        public int AddRow(int rowsCount, DataGridView dgv)
        {
            r = rowsCount;
            ++rowsCount;
            DataGridViewRow newRow = (DataGridViewRow)dgv.Rows[0].Clone();
            dgv.Rows.Add(newRow); 
            dgv.Rows[r].HeaderCell.Value = (rowsCount - 1).ToString();
            return (0);
        }
    }
}
