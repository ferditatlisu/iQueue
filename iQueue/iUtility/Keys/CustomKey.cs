using System;
using System.Collections.Generic;
using System.Text;

namespace iUtility.Keys
{
    public class CustomKey
    {
        public const string AMPERSAND_KEY = "&";
        public const string QUESTION_MARK_KEY = "?";
        public const string EQUALS_SIGN_KEY = "=";
        public const string COMMA_KEY = ",";
        public const string SPACE_KEY = " ";
        public const char PLUS_KEY = '+';

        public const string JSON_CONTENT_TYPE = "application/json";

        public const string HEALTH_CHECK_RESPONSE_KEY = "Success";
        public const string QUEUE_DEFAULT_EXCHANGE_KEY = "";

        public const string SCHEDULE_QUEUE_NAME_FORMAT = "{0}_Schedule"; //0 QueueName
        public const string SCHEDULE_QUEUE_EXCHANGE_NAME_FORMAT = "{0}_Exchange"; //0 QueueName
    }
}
