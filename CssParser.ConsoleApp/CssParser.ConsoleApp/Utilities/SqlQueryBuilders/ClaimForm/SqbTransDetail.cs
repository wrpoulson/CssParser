using CssParser.ConsoleApp.Models.ClaimForm;

namespace CssParser.ConsoleApp.Utilities.SqlQueryBuilders.ClaimForm
{
    public static class SqbTransDetail
    {
        public static string UpdateTransDetRecord(TransDetail transDetail)
        {
            return $"\nUPDATE TRANS_DET\nSET XP_CSS_LEFT = '{transDetail.Xp_Css_Left}', XP_CSS_TOP = '{transDetail.Xp_Css_Top}', XP_CSS_WIDTH = '{transDetail.Xp_Css_Width}'\nWHERE FIELDNAME = '{transDetail.FieldName}' AND [TYPE] = '{transDetail.Type}' AND [NAME] = '{transDetail.Name}'";
        }
        public static string SelectCountForTransDetRecord(TransDetail transDetail)
        {
            return $"(SELECT COUNT(*) FROM TRANS_DET WHERE FIELDNAME = '{transDetail.FieldName}' AND [TYPE] = '{transDetail.Type}' AND [NAME] = '{transDetail.Name}') = 1";
        }

        public static string PrintTransDetailUpdateError(TransDetail transDetail)
        {
            return SqlQueryBuilder.Print($"\tWARNING: Record was NOT UPDATED. --- FIELDNAME: {transDetail.FieldName} --- NAME: {transDetail.Name} --- TYPE: {transDetail.Type} ");
        }

        public static string PrintTableDoesNotExistRollback(string tableName)
        {
            return $"{SqlQueryBuilder.Print($"ERROR: Table: {tableName} does NOT exist. Commencing transaction rollback.")}";
        }

        public static string SqlIfElseUpdateTransDet(TransDetail transDetail)
        {
            return $"\t{SqlQueryBuilder.IfElse(SelectCountForTransDetRecord(transDetail), UpdateTransDetRecord(transDetail), PrintTransDetailUpdateError(transDetail))}";
        }
    }
}
