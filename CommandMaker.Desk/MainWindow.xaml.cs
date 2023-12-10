using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CommandMaker.Desk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mysqlBox.Text = GenerateVarchaMysqlTableCommand(TableNameBox.Text,csvBox.Text);
        }
        string GenerateVarchaMysqlTableCommand(string tableName,string csvText)
        {
            string[] lines = csvText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            // Create the table
            string createTableQuery = $"CREATE TABLE {tableName} (id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, ";
            string[] columnNames = lines[0].Replace(' ', '_').Split(',');
            for (int i = 0; i < columnNames.Length; i++)
            {
                if (!String.IsNullOrEmpty(columnNames[i]))
                {
                    createTableQuery += $"{columnNames[i]} VARCHAR(255), ";
                }

            }
            createTableQuery = createTableQuery.TrimEnd(',', ' ') + ");";

            // Insert the data
            string insertDataQuery = $"INSERT INTO {tableName} (";
            for (int i = 0; i < columnNames.Length; i++)
            {
                insertDataQuery += $"{columnNames[i]}, ";
            }
            insertDataQuery = insertDataQuery.TrimEnd(',', ' ') + ") VALUES ";
            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Replace("'", "`").Split(',');
                insertDataQuery += "(";
                for (int j = 0; j < values.Length; j++)
                {
                    if (!String.IsNullOrEmpty(columnNames[j]))
                    {
                        insertDataQuery += $"'{values[j]}', ";
                    }

                }
                insertDataQuery = insertDataQuery.TrimEnd(',', ' ') + "), ";
            }
            insertDataQuery = insertDataQuery.TrimEnd(',', ' ') + ";";

            return String.Concat(createTableQuery, "\n", insertDataQuery);
        }
    }
}