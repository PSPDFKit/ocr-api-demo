using System;
using System.IO;
using System.Net;
using RestSharp;

namespace PspdfkitApiDemo
{
  class Program
  {
    static void Main(string[] args)
    {
      var client = new RestClient("https://api.pspdfkit.com/build");

      var bearer = "YOUR BEARER CODE";

      var request = new RestRequest(Method.POST)
        .AddHeader("Authorization", bearer)
        .AddFile("scanned", "document.pdf")
        .AddParameter("instructions", new JsonObject
        {
          ["parts"] = new JsonArray
          {
            new JsonObject
            {
              ["file"] = "scanned"
            }
          },
          ["actions"] = new JsonArray
          {
            new JsonObject
            {
              ["type"] = "ocr",
              ["language"] = "english"
            }
          }
        }.ToString());

      request.AdvancedResponseWriter = (responseStream, response) =>
      {
        if (response.StatusCode == HttpStatusCode.OK)
        {
          using (responseStream)
          {
            using var outputFileWriter = File.OpenWrite("result.pdf");
            responseStream.CopyTo(outputFileWriter);
          }
        }
        else
        {
          var responseStreamReader = new StreamReader(responseStream);
          Console.Write(responseStreamReader.ReadToEnd());
        }
      };

      client.Execute(request);
    }
  }
}