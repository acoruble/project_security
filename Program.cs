using System;
using System.IO;
﻿using System.Net;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Data;

namespace projetSecurity
{
    class Program
    {
        static void Main(string[] args)
        {

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            
            listener.Start(); 
            Console.WriteLine("Listening...");

            int compteur;
            for (compteur = 0; compteur < 1;)
            {

            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            // Obtain a response object.
            HttpListenerResponse response = context.Response;
                        // Note: The GetContext method blocks while waiting for a request. 

            if (request.Url.OriginalString == @"http://localhost:8080/")
            {
            // Construct a response.
            string responseString = @"
            <HTML><BODY>
            <form action=""login.php"" method=""post"">
            <label for=""login"">Login :</label>
            <input type=""text"" name=""login"">
            <label for=""password"">Password :</label>
            <input type=""password"" name=""password"">
            <button type=""submit"">Connexion</button>
            </form>
            </BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer,0,buffer.Length);
            // You must close the output stream.
            output.Close();
            }

            if (request.Url.OriginalString == @"http://localhost:8080/login.php")
            {
            string responseString = @"
            <HTML><BODY>
            T'es connecté :D !
            </BODY></HTML>";

            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader(request.InputStream)) 
            {
                string line;
                while ((line = sr.ReadLine()) != null) 
                {
                    Console.WriteLine(line);
                    var tab = line.Split(new[] {'=', '&'});
                    Console.WriteLine(tab[1]);
                    Console.WriteLine(tab[3]);

                    Encoding unicode = Encoding.Unicode;
                    byte[] unicodeBytes = unicode.GetBytes(tab[3]);

                    SHA256 mySHA256 = SHA256.Create();
                    byte[] hashValue = mySHA256.ComputeHash(unicodeBytes);
                    // Write the name and hash value of the file to the console.
                    Console.Write(hashValue);

                    try
                        {
                        Console.Write("Connecting to SQL Server ... ");
                        SqlConnection connection = new SqlConnection("Server=.;Database=jdr;Trusted_Connection=True;");
                        connection.Open();
                        Console.WriteLine("Done.");

                        //Preparation à l'envoie de la demandes
                       // SqlCommand command = new SqlCommand(null, connection);
                       // command.CommandText = "INSERT INTO connexion (login, password) VALUES (@login, @password)";
                       // SqlParameter loginParam = new SqlParameter("@login", SqlDbType.NVarChar, 10);
                       // SqlParameter passwordParam = new SqlParameter("@password", SqlDbType.Binary,hashValue.Length);
                       // loginParam.Value = tab[1];
                       // passwordParam.Value = hashValue;
                       // command.Parameters.Add(loginParam);
                       // command.Parameters.Add(passwordParam);
                       // command.Prepare();
                       // command.ExecuteNonQuery();

                       //Preparation à l'envoie de la demande
                        SqlCommand command = new SqlCommand(null, connection);
                        command.CommandText = "SELECT password FROM connexion WHERE login=@login";
                        SqlParameter loginParam = new SqlParameter("@login", SqlDbType.NVarChar, 10);
                        loginParam.Value = tab[1];
                        command.Parameters.Add(loginParam);
                        command.Prepare();
                        SqlDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                Console.WriteLine(String.Format("{0}",
                                reader[0]));
                            }
                        }

                    catch (SqlException e)
                        {
                        Console.WriteLine(e.ToString());
                        }
                    }
            }

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer,0,buffer.Length);
            // You must close the output stream.
            output.Close();
         
            }
     }
    }
}
}
