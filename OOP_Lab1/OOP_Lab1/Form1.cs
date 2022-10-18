using OOP_Lab_01;
using System.Configuration;
using System.Data;
using System.Formats.Asn1;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Windows.Forms;

namespace OOP_Lab1
{
    public partial class Form1 : Form
    {

        const int st = 100;
        int columns = 0;
        int rows = 0;
        Dictionary<int, object> _values = new Dictionary<int, object>();
        Cell[,] table = new Cell[st, st];
        CreateNewElement NewElement = new CreateNewElement();
        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < st; i++)
                for (int j = 0; j < st; j++)
                    table[i, j] = new Cell();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox1.Text = table[dataGridView1.CurrentCell.RowIndex, dataGridView1.CurrentCell.ColumnIndex].Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NewElement.AddColumn(dataGridView1, columns);
            columns++;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NewElement.AddRow(rows, dataGridView1);
            rows++;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            columns = 5;
            rows = 5;
            string temp = null;
            char c = 'A';
            Cell[,] table = new Cell[st, st];
            for (int i = 0; i < columns; i++)
            {
                temp += c;
                dataGridView1.Columns.Add(Name, temp);
                c++;
                temp = null;
            }
            for (int i = 0; i < rows; i++)
            {
                dataGridView1.Rows.Add();
            }

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].HeaderCell.Value = i.ToString();
            }
            dataGridView1.AllowUserToAddRows = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            if (dataGridView1.RowCount > 2)
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    addNewValue("0", dataGridView1.RowCount - 1, i);
                }
                //dataGridView1.Refresh();
                dataGridView1.Rows.RemoveAt(dataGridView1.RowCount - 1);
                rows--;
            }
            else
                MessageBox.Show("Unable to delete last two rows");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool columnIsEmpty = true;
            columns = dataGridView1.ColumnCount;
            for(int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[columns - 1].Value != null)
                {
                    columnIsEmpty = false;
                }
            }

            if (dataGridView1.Columns.Count <= 2)
            {

                return;
            }
            if (!columnIsEmpty)
            {

                return;
            }
            else
            {
                for(int i = 0; i < rows; i++)
                {
                    //addNewValue("0", i, columns - 1);
                }
                dataGridView1.Columns.RemoveAt(columns - 1);
                columns--;
            }
        }

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            textBox1.Text = table[dataGridView1.CurrentCell.RowIndex, dataGridView1.CurrentCell.ColumnIndex].Value;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string temp = (string)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            int r = dataGridView1.CurrentCell.RowIndex;
            int c = dataGridView1.CurrentCell.ColumnIndex;
            if (temp != null)
                addNewValue(temp, r, c);
        }


        private void textBox1_KeyDown (object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter || e.KeyCode == Keys.M)
            {
                string temp = (string)textBox1.Text;
                int r = dataGridView1.CurrentCell.RowIndex;
                int c = dataGridView1.CurrentCell.ColumnIndex;
                addNewValue(temp, r, c);
            }
        }

        public void addNewValue(string expression, int r, int c)
        {
            Lexer parser = new Lexer();
            parser.SetExpression(expression);
            table[r, c].Value = expression;
            int h = 0;  
            if (expression != null)
            {
                while (h < expression.Length)
                {
                    string str = null;
                    int t2, t1 = (int)expression[h];
                    str += expression[h];
                    h++;
                    if (t1 > 64 && t1 < 91)
                    {
                        str += expression[h];
                        t2 = (int)expression[h];
                        h++;
                        table[t2 - 48, t1 - 65].dependents.Add(table[r, c].getName(c, r));
                        expression = expression.Replace(str, table[t2 - 48, t1 - 65].value_s);
                    }                  
                }
                if (circle(table[r, c], r, c))
                {
                    Report Rep = parser.Evaluate(expression);
                    
                    if (Rep.Exception())
                    {
                        table[r, c].value_s = Rep.GetValue();
                        update(table[r, c], r, c);
                        dataGridView1.Rows[r].Cells[c].Value = Rep.GetValue();
                    }
                    else
                        dataGridView1.Rows[r].Cells[c].Value = Rep.GetValue();
                }
            }
        }
        public void update(Cell cell, int r, int c)
        {
            if (cell.dependents.Count > 1 && cell.dependents[1].Length > 0)
            {
                for (int i = 0; i < cell.dependents.Count; i++)
                {
                    int t2 = (int)cell.dependents[1][1] - 48, t1 = cell.dependents[1][0] - 65;

                    Connection(table[t2, t1].Value, t2, t1);
                }
            }
        }

        public bool circle(Cell cell, int r, int c)
        {
            for (int i = 0; i < cell.dependents.Count; i++)
            {
                if (cell.dependents.Count > 1 && cell.dependents[1].Length > 0)
                {
                    int t2 = (int)cell.dependents[1][1] - 48, t1 = cell.dependents[1][0] - 65;
                    for (int j = 0; j < table[t2, t1].dependents.Count; j++)
                    {
                        if (table[t2, t1].dependents[j] == table[r, c].getName(r, c))
                        {
                            nameCircle(cell, r, c);
                            nameCircle(table[t2, t1], t2, t1);
                            table[t2, t1].dependents.RemoveAt(j);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public void nameCircle(Cell cell, int r, int c)
        {
            for (int flag = 0; flag < cell.dependents.Count; flag++)
            {
                int _t2 = (int)cell.dependents[flag][1] - 48, _t1 = (int)cell.dependents[flag][0] - 65;
                dataGridView1.Rows[_t2].Cells[_t1].Value = "#CIRCLE";
            }
        }

        public void Connection(string temp, int row, int column)
        {
            Lexer parser = new Lexer();
            int h1 = 0;
            while (h1 < temp.Length)
            {
                string str = null;
                int t2, t1 = (int)temp[h1];
                str += temp[h1];
                h1++;

                if ((t1 > 64) && (t1 < 91))
                {
                    str += temp[h1];
                    t2 = (int)temp[h1];
                    h1++;
                    temp = temp.Replace(str, table[t2 - 48, t1 - 65].value_s);
                }
            }
            if (circle(table[row, column], row, column))
            {
                Report Rep = parser.Evaluate(temp);
                if (Rep.Exception())
                {
                    table[row, column].value_s = Rep.GetValue();
                    update(table[row, column], row, column);
                    dataGridView1.Rows[row].Cells[column].Value = Rep.GetValue();
                }
                else return;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string temp = (string)textBox1.Text;
            int r = dataGridView1.CurrentCell.RowIndex;
            int c = dataGridView1.CurrentCell.ColumnIndex;
            addNewValue(temp, r, c);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Текстовый документ (*.txt)|*.txt|Все файлы (*.*)|*.*";
            
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.Unicode);
                try
                {
                    List<int> col_n = new List<int>();
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                        if (col.Visible)
                        {
                            //sw.Write(col.HeaderText + "\t");
                            col_n.Add(col.Index);
                        }
                    //sw.WriteLine();
                    int x = dataGridView1.RowCount;
                    if (dataGridView1.AllowUserToAddRows) x--;

                    for (int i = 0; i < x; i++)
                    {
                        for (int y = 0; y < col_n.Count; y++)
                        {
                            if(i != col_n.Count - 1)
                                sw.Write(dataGridView1[col_n[y], i].Value + " ");
                            else
                                sw.Write(dataGridView1[col_n[y], i].Value);
                        }
                        sw.Write("\n");
                    }
                    sw.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }


        	
        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();


            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

                StreamReader rd = new StreamReader(openFileDialog.FileName);
                string header = rd.ReadLine();
                
                int i = 0; 
                while(header != null)
                {
                    string[] col = System.Text.RegularExpressions.Regex.Split(header, " ");
                    if (col.Length > dataGridView1.ColumnCount)
                    {
                        while (col.Length > dataGridView1.ColumnCount)
                        {
                            NewElement.AddColumn(dataGridView1, columns);
                            columns++;
                        }
                    }
                    for (int j = 0; j < col.Length; j++)
                    {
                        addNewValue(col[j], i, j);
                    }
                    header = rd.ReadLine();
                    i++;
                    if(i >= dataGridView1.RowCount && header != null)
                    {
                        NewElement.AddRow(rows, dataGridView1);
                        rows++;
                    }
                }
                return;    
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Для введення даних натисніть на клітинку таблиці, після чого напишіть бажану формулу у текст бокс і натисніть кнопку Enter справа від текст боксу. Літери нижнього реєстру програмою сприймаються як помилка синтаксису.");
        }
    }
}