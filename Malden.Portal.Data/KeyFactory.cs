namespace Malden.Portal.Data.TableStorage
{
    public static class KeyFactory
    {
        public static string PartitionKey(string partitionKeyValue)
        {
            return System.Convert.ToBase64String(System.Text.UnicodeEncoding.Unicode.GetBytes(partitionKeyValue)); ;
        }

        public static string RowKey(string rowKeyValue)
        {
            return System.Convert.ToBase64String(System.Text.UnicodeEncoding.Unicode.GetBytes(rowKeyValue)); ;
        }
    }
}