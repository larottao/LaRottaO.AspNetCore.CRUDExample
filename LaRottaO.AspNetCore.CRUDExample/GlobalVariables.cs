namespace LaRottaO.AspNetCore.CRUDExample
{
    public static class GlobalVariables
    {
        internal static readonly string MESSAGE_ERROR_IN_DATABASE = "Database Error. Please try again later.";

        internal static readonly string PATH_LOGGING_FILE = "C:\\Logs\\CRUDExample.txt";

        //"Server=localhost;Database=YourDatabase;User=root;Password=YourPassword;"
        internal static readonly string MYSQL_CONNECTION_STRING = "server='127.0.0.1';user='root';database='local_tests';port='3308';password='admin';SSLMode=none";
    }
}