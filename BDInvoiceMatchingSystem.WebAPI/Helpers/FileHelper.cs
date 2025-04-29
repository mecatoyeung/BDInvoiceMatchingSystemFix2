using System;
using System.IO;

namespace BDInvoiceMatchingSystem.WebAPI.Helpers
{
    public class FileHelper
    {
        public static void CopyFile(string sourcePath, string destinationPath)
        {
            if (File.Exists(sourcePath))
            {
                try
                {
                    File.Copy(sourcePath, destinationPath, true);
                    Console.WriteLine($"File copied from {sourcePath} to {destinationPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Source file does not exist: {sourcePath}");
            }
        }
    }
}