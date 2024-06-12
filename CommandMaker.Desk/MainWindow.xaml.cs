using System.Text;
using System.Text.RegularExpressions;
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
            bool isCsvTextValid = IsValidCsv(csvBox.Text);
            mysqlBox.Text = GenerateVarchaMysqlTableCommand(TableNameBox.Text, csvBox.Text);
            if (isCsvTextValid)
            {
                ErrorMsg.Text = String.Empty;
                
            }
            else
            {
                ErrorMsg.Text = "Invalid CSV careful with generated Mysql command/s";
                //mysqlBox.Text = String.Empty;
            }
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
        private bool IsValidCsv(string text)
        {
            // Split the text into rows
            string[] rows = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            foreach (string row in rows)
            {
                // Split each row into values using comma as the delimiter
                string[] values = row.Split(',');

                foreach (string value in values)
                {
                    // Trim whitespace and remove double quotes
                    string trimmedValue = value.Trim().Trim('"');

                    // Check if the trimmed value is empty
                    if (string.IsNullOrEmpty(trimmedValue)|| Regex.IsMatch(trimmedValue, @"[,\n""]"))
                    {
                        return false; // Empty value is not valid
                    }

                }
            }

            return true; // All values are valid
        }
    }
}