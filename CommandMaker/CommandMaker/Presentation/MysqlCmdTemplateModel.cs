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
    public void Formulate()
    {
        string[] lines = CsvText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        // Create the table
        CreateTableScript = $"CREATE TABLE {TableName} (id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, ";
        string[] columnNames = lines[0].Split(',');
        for (int i = 0; i < columnNames.Length; i++)
        {
            CreateTableScript += $"{columnNames[i]} VARCHAR(255), ";
        }
        CreateTableScript = CreateTableScript.TrimEnd(',', ' ') + ");";

        // Insert the data
        InsertRecordsScript = $"INSERT INTO {TableName} (";
        for (int i = 0; i < columnNames.Length; i++)
        {
            InsertRecordsScript += $"{columnNames[i]}, ";
        }
        InsertRecordsScript = InsertRecordsScript.TrimEnd(',', ' ') + ") VALUES ";
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            InsertRecordsScript += "(";
            for (int j = 0; j < values.Length; j++)
            {
                InsertRecordsScript += $"'{values[j]}', ";
            }
            InsertRecordsScript = InsertRecordsScript.TrimEnd(',', ' ') + "), ";
        }
        InsertRecordsScript = InsertRecordsScript.TrimEnd(',', ' ') + ";";
    }
}
