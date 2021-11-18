using System;
using System.IO;
using senderFile.Modules;

namespace senderFile
{
    public class FileSender
    {
        public string getTitle() => $"{ cfg.ProgrammName } v.{ cfg.Version } dev: { cfg.ProgrammName }";

        private Config cfg = new Config();
        private IniFile INI = new IniFile("config.ini");

        public FileSender()
        {
            this.Load();
        }

        public void Run()
        {
            string[] fileData = cfg.Path_File.Split('_');
            string date = DateTime.Now.ToShortDateString();
            string fullname = fileData[0] + "_" + date + fileData[1];

            if (File.Exists(fullname))
            {
                string[] emails = cfg.Receivers.Split(';');
                foreach (var email in emails)
                {
                    if (String.IsNullOrEmpty(email)) return;

                    try
                    {
                        new SendMail(cfg.SMTP_HOST, cfg.SMTP_PORT, cfg.SMTP_USER, cfg.SMTP_PASSWORD, cfg.SMTP_SSL, cfg.SMTP_TIMEOUT)
                            .InitAddress("robot@sakh.dvec.ru", "robot", email)
                            .AddText($"Инфомат., файл за { date }", $"<p>Файл был отправлен автоматически.</p>")
                            .AddFile(fullname)
                            .send();

                        AddLog($"Сообщение отправлено на адрес: {email}");
                    }
                    catch (Exception error)
                    {                        
                        if (error.InnerException != null)
                        {
                            string erro = error.InnerException.Message;
                            AddLog(erro);
                        }
                        else
                        {
                            AddLog(error.ToString());
                        }
                        Console.WriteLine("Ошибка при отправке");
                        Console.ReadKey();
                    }
                }
            }
            else
                AddLog("Нет файлов для отправки.");
        }

        private void Load()
        {
            if (INI.KeyExists("emails", "receivers")) cfg.Receivers = INI.ReadINI("receivers", "emails");
            if (INI.KeyExists("path", "receivers")) cfg.Path_File = INI.ReadINI("receivers", "path");
            if (INI.KeyExists("host", "smtp")) cfg.SMTP_HOST = INI.ReadINI("smtp", "host");
            if (INI.KeyExists("port", "smtp")) cfg.SMTP_PORT = int.Parse(INI.ReadINI("smtp", "port"));
            if (INI.KeyExists("user", "smtp")) cfg.SMTP_USER = INI.ReadINI("smtp", "user");
            if (INI.KeyExists("password", "smtp")) cfg.SMTP_PASSWORD = Security.DeCrypt(INI.ReadINI("smtp", "password"), "test8");
            if (INI.KeyExists("ssl", "smtp")) cfg.SMTP_SSL = Boolean.Parse(INI.ReadINI("smtp", "ssl"));
            if (INI.KeyExists("timeout", "smtp")) cfg.SMTP_TIMEOUT = int.Parse(INI.ReadINI("smtp", "timeout"));
        }

        private void Save()
        {
            INI.Write("receivers", "emails", cfg.Receivers);
            INI.Write("receivers", "path", cfg.Path_File);
            INI.Write("smtp", "host", cfg.SMTP_HOST);
            INI.Write("smtp", "port", cfg.SMTP_PORT.ToString());
            INI.Write("smtp", "user", cfg.SMTP_USER);
            INI.Write("smtp", "password", Security.Crypt(cfg.SMTP_PASSWORD, "test8"));
            INI.Write("smtp", "ssl", cfg.SMTP_SSL.ToString());
            INI.Write("smtp", "timeout", cfg.SMTP_TIMEOUT.ToString());
        }

        private void AddLog(string text)
        {
            text = $"\n{ DateTime.Now } - { text }";
            // имя файла
            string filePath = @"log.txt";
            FileStream fileStream = null;

            // проверяем существует ли файл файл
            if (!File.Exists(filePath))
                fileStream = File.Create(filePath); // создаем файл
            else
                fileStream = File.Open(filePath, FileMode.Append); // открываем файл и в конец будут добавлены данные

            // получаем поток
            StreamWriter output = new StreamWriter(fileStream);
            // записываем текст в поток
            output.Write(text);
            // закрываем поток
            output.Close();
        }
    }
}
