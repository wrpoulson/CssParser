using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssParser.ConsoleApp.Utilities.ClaimForm
{
    public static class SqlQueryBuilder
    {
        public static string Use(string database) => $"USE {database}\nGO\n\n";

        public static string BeginTransaction() => $"BEGIN TRANSACTION;\nGO\n";

        public static string RollbackTransaction() => "\nROLLBACK TRANSACTION;";

        public static string Print(string text) => $"\nPRINT CAST(GETDATE() as Datetime2(3))\nPRINT N'{text}'";

        public static string If(string condition, string conditionalOperation) => $"IF {condition}\nBEGIN\n{conditionalOperation.Replace("\n", "\n\t")}\nEND;";

        public static string IfElse(string condition, string ifConditionalOperation, string elseConditionalOperation)
        {
            return $"\nIF {condition}\nBEGIN{ifConditionalOperation.Replace("\n", "\n\t")}\nEND;\n\nELSE\nBEGIN{elseConditionalOperation.Replace("\n", "\n\t")}\nEND;";
        }

        public static string TableExistsCondition(string schema, string tableName)
        {
            return $"(EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{schema}' AND TABLE_NAME = '{tableName}'))";
        }

        public static string Try(string tryBlock)
        {
            return $"\nBEGIN TRY\n{tryBlock}\nEND TRY\n";
        }

        public static string Catch(string catchBlock)
        {
            return $"\nBEGIN CATCH\n{catchBlock}\nEND CATCH\n";
        }
    }
}
