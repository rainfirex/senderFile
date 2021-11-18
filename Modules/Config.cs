using System;

namespace senderFile.Modules
{
    public class Config
    {
        public string ProgrammName { get; } = "SendFile";

        public string Version { get; } = "1.1";

        public string Programmist { get; } = "Poplavskiy Aleksandr";

        public string Receivers { get; set; } = "";

        public string Path_File { get; set; } = @"C:\";

        public string USER { get; set; } = String.Format(@"{0}\\{1}", Environment.UserDomainName, Environment.UserName);

        public string SMTP_HOST { get; set; } = "127.0.0.1";

        public int    SMTP_PORT { get; set; } = 25;

        public bool SMTP_SSL { get; set; } = false;

        public string SMTP_USER { get; set; } = "example@sakh.dvec.ru";

        public string SMTP_PASSWORD { get; set; } = "password";

        public int SMTP_TIMEOUT { get; set; } = 40;
    }
}
