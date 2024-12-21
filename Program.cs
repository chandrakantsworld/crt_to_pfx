using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Paths to the input files
            string certificatePath = "certificate.crt"; // Replace with your .crt file path
            string privateKeyPath = "privatekey.pem"; // Replace with your .pem file path

            // Path for the output .pfx file
            string outputPfxPath = "path/to/output.pfx"; // Replace with your desired .pfx file path
            string pfxPassword = "your-password"; // Password to protect the .pfx file

            // Read the certificate
            string certificateContent = File.ReadAllText(certificatePath);
            X509Certificate2 certificate = new X509Certificate2(Convert.FromBase64String(certificateContent));

            // Read the private key
            string privateKeyContent = File.ReadAllText(privateKeyPath);
            privateKeyContent = privateKeyContent.Replace("-----BEGIN PRIVATE KEY-----", "")
                                                 .Replace("-----END PRIVATE KEY-----", "")
                                                 .Replace("\n", "")
                                                 .Replace("\r", "");

            byte[] privateKeyBytes = Convert.FromBase64String(privateKeyContent);

            // Create a combined certificate with the private key
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
                certificate = certificate.CopyWithPrivateKey(rsa);
            }

            // Export the combined certificate as a .pfx file
            byte[] pfxBytes = certificate.Export(X509ContentType.Pfx, pfxPassword);
            File.WriteAllBytes(outputPfxPath, pfxBytes);

            Console.WriteLine("PFX file created successfully at: " + outputPfxPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}

