using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace TestHttp {
  class Program {
    static void Main(string[] args) {

      var server = new TcpListener(IPAddress.Any, 8811);
      try {
        server.Start();
        byte[] bytes = new byte[1024];
        string data;

        while (true) {
          Console.Write("Waiting for a connection... ");
          TcpClient client = server.AcceptTcpClient();
          Console.WriteLine("Connected!");

          NetworkStream stream = client.GetStream();
          int count = stream.Read(bytes, 0, bytes.Length);
          while (count != 0) {
            // Translate data bytes to a ASCII string.
            data = Encoding.UTF8.GetString(bytes, 0, count);
            Console.WriteLine("Received:");
            Console.WriteLine(data);

            //// Send back a response.
            byte[] msg = Encoding.UTF8.GetBytes(GenerateAnswer("Hello Http!!"));
            stream.Write(msg, 0, msg.Length);
            Console.WriteLine("Sent answer");

            count = stream.Read(bytes, 0, bytes.Length);
          }

          // Shutdown and end connection
          client.Close();
        }
      }
      catch (Exception ex) {
        server.Stop();
        throw;
      }
    }

    static string GenerateAnswer(string message) {

      StringBuilder sb = new StringBuilder();
      sb.AppendLine("HTTP / 1.1 200 OK");
      sb.AppendLine($"Date: {DateTime.Now.ToLongDateString()}"); // Wed, 11 Feb 2009 11:20:59 GMT
      sb.AppendLine($"Server: myne");
      sb.AppendLine($"Last-Modified: {DateTime.Now.ToLongDateString()}");
      sb.AppendLine($"Content-Language: en");
      sb.AppendLine($"Content-Type: text/html;charset = utf-8");
      sb.AppendLine($"Content-Length: {message.Length}");
      sb.AppendLine($"Connection: close");
      sb.AppendLine($"");
      sb.AppendLine(message);

      return sb.ToString();
    }

    static void Client() {

      using (var client = new TcpClient("ricco.kh.ua", 80)) {

        Console.WriteLine($"IsConnected: {client.Connected}");
        StringBuilder request = new StringBuilder();
        request.AppendLine(@"GET /menu.html HTTP/1.1");
        request.AppendLine(@"Host: ricco.kh.ua");
        request.AppendLine(@"");

        using (var stream = client.GetStream()) {
          var data = Encoding.UTF8.GetBytes(request.ToString());
          stream.Write(data, 0, data.Length);
          stream.Flush();

          var task = new StreamReader(stream, Encoding.GetEncoding(1251)).ReadToEndAsync();
          task.Wait();
          Console.Write(task.Result);
          //var answerByte = new byte[1024];
          //while (stream.ReadAsync(answerByte, 0, answerByte.Length) > 0) {
          //Console.Write(Encoding.GetEncoding(1251).GetString(answerByte));
          //}
        }
        Console.WriteLine();
      }
    }
  }
}
