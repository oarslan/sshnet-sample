using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SshConnection
{
    public class Client
    {
        private const int TIMEOUT=2000;

        private bool connected;

        private SshClient client;

        private String password;

        private StreamReader reader;

        private StreamWriter writer;

        private ShellStream shellStream;
        public bool Connected
        {

            get
            {
                return this.connected;
            }
        }

        private void HandleKeyEvent(Object sender, AuthenticationPromptEventArgs e)
        {

            foreach (AuthenticationPrompt prompt in e.Prompts)
            {

                if (prompt.Request.IndexOf("Password:", StringComparison.InvariantCulture) != -1)
                {
                    prompt.Response = this.password;
                }

            }
        }

        private void CreateClient(string ip, int port, string user, string password)
        {

            if (this.client != null)
                return;

            var kAuth = new KeyboardInteractiveAuthenticationMethod(user);
            var pAuth = new PasswordAuthenticationMethod(user, password);

            kAuth.AuthenticationPrompt += new EventHandler<AuthenticationPromptEventArgs>(HandleKeyEvent);

            var connectionInfo = new ConnectionInfo(ip, port, user, pAuth, kAuth);
            connectionInfo.Timeout= TimeSpan.FromMilliseconds(TIMEOUT);
            this.client = new SshClient(connectionInfo);
            this.client.Connect();

        }


        public void Connect(string ip, int port, string user, string password)
        {
            this.password = password;

            CreateClient(ip, port, user, password);

            if (this.client.IsConnected)
                this.connected = true;

        }

    
        public string CreateCommand(string command)
        {
            var cmdResult = this.client.CreateCommand(command);
            cmdResult.Execute();
            return cmdResult.Result;
        }

        public string CreateCommandWithStream(String command)
        {
            this.shellStream = this.client.CreateShellStream("", 0, 0, 0, 0, 1024);

            this.reader = new StreamReader(this.shellStream);
            this.writer = new StreamWriter(this.shellStream)
            {
                AutoFlush = true
            };

            while (!this.shellStream.DataAvailable)
            {
                WriteStream(command, this.writer, this.shellStream);
                Thread.Sleep(TIMEOUT);
            }
            return  ReadStream(reader);
     
        }

        private void WriteStream(string cmd, StreamWriter writer, ShellStream stream)
        {
            writer.WriteLine(cmd);
        }

        private String ReadStream(StreamReader reader)
        {
            var result = new StringBuilder();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                result.AppendLine(line);
            }
            return result.ToString();
        }


        public void Disconnect()
        {
            this.connected = false;
            this.writer.Close();
            this.reader.Close();
            this.shellStream.Close();
            this.client.Disconnect();
        }

    }
}
