using System;
using System.IO;

namespace CpsCopy
{
    /// <summary>
    /// Interaction logic for Program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The Main method.
        /// </summary>
        /// <param name="args">Argument input.</param>
        static void Main(string[] args)
        {
            try
            {
                // Test input for development.
                //args = new string[2]; 
                //args[0] = @"C:\Users\Admin\Desktop\movie.mkv";
                //args[1] = @"C:\Users\Admin\Desktop\Movie";

                // Only start the process if two arguments are given.
                if (args.Length == 2)
                {
                    string sourceRoute = args[0];
                    string destinationRoute = args[1];
                    bool exitString = false;

                    #region Validation

                    try
                    {
                        // The source must not be empty.
                        if (!string.IsNullOrEmpty(sourceRoute) && !string.IsNullOrWhiteSpace(sourceRoute))
                        {
                            // The first argument must be an existing file.
                            if (!File.Exists(sourceRoute))
                            {
                                Console.WriteLine("Source file or route is not valid.");
                            }
                            else
                            {
                                Console.WriteLine("Source validated.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Source route is missing.");
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("An error occured at source route validation.");
                    }

                    try
                    {
                        // The destination must not be empty.
                        if (!string.IsNullOrEmpty(destinationRoute) && !string.IsNullOrWhiteSpace(destinationRoute))
                        {
                            // If the second argument directory doesn't exist, then create it.
                            if (!Directory.Exists(destinationRoute))
                            {
                                Directory.CreateDirectory(destinationRoute);
                                Console.WriteLine("Destination created.");
                            }
                            else
                            {
                                Console.WriteLine("Destination validated.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Destination route is missing.");
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("An error occured at destination route validation.");
                    }

                    Console.WriteLine();

                    #endregion Validation

                    while (!exitString)
                    {
                        #region Copy

                        try
                        {
                            // The source and destination must exist.
                            if (File.Exists(sourceRoute) && Directory.Exists(destinationRoute))
                            {
                                FileCopy.Copy(sourceRoute, Path.Combine(destinationRoute, Path.GetFileName(sourceRoute)), 
                                    (long TotalFileSize, long TotalBytesTransferred, long StreamSize, long StreamBytesTransferred, uint dwStreamNumber, 
                                        FileCopy.CopyProgressCallbackReason dwCallbackReason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData) =>
                                {
                                    // Count the percentage of the done process.
                                    int progress = (int)(((decimal)TotalBytesTransferred / (decimal)TotalFileSize) * 100);

                                    // Check if the files are transferred.
                                    if (progress == 100)
                                    {
                                        exitString = true;
                                    }

                                    // Show the percentage of the done process.
                                    if (progress <= 100)
                                    {
                                        Console.WriteLine(progress + "%");
										
                                        return FileCopy.CopyProgressResult.PROGRESS_CONTINUE;
                                    }

                                    return FileCopy.CopyProgressResult.PROGRESS_STOP;
                                });
                            }
                            else
                            {
                                Console.WriteLine("Copy failed.");
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("An error occured at copy.");
                        }

                        #endregion Copy
                    }
                }
                else
                {
                    Console.WriteLine("Two arguments are required.");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("An error occured at the input reeding.");
            }
        }
    }
}
