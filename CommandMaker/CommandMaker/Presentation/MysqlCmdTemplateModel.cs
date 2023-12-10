using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandMaker.Presentation;
public class MysqlCmdTemplateModel
{
    
    public string SchemaName { get; set; }
    public string TableName { get; set; }
    public string CreateTableScript { get; set; }
    public string InsertRecordsScript { get; set; }
    public string CsvText { get; set; }
    public string SchemaLabel => "Schema Name";
    public string CsvLabel => "Csv Content";
    public MysqlCmdTemplateModel(string schema = "Sample")
    {
        SchemaName = schema;
        CsvText = @"ISO3,Status,English Name,EngName,Continent,Region,ISO2,
MNP,US Territory,USA,Northern Mariana Islands,Oceania,Micronesia,MP,
EGY,Member State,EGY,Egypt,Africa,Northern Africa,EG,
DEU,Member State,DEU,Germany,Europe,Western Europe,DE,
CIV,Member State,CIV,Côte d'Ivoire,Africa,Western Africa,CI,
URY,Member State,URY,Uruguay,Americas,South America,UY,";
    }
    public async Task Formulate()
    {
       await Task.Factory.StartNew(() => GenerateVarchaMysqlTableCommand(TableName,CsvText));
    }
    void GenerateVarchaMysqlTableCommand(string tableName, string csvText)
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
        CreateTableScript = createTableQuery.TrimEnd(',', ' ') + ");";
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
        InsertRecordsScript = insertDataQuery.TrimEnd(',', ' ') + ";";

    }
}
