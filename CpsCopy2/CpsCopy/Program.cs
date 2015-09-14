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
                    var sourceRoute = args[0];
                    var destinationRoute = args[1];
                    var exit = false;

                    #region Validation

                    try
                    {
                        // The source must not be empty.
                        if (!string.IsNullOrEmpty(sourceRoute))
                        {
                            // The first argument must be an existing file.
                            if (!File.Exists(sourceRoute))
                            {
                                Console.WriteLine("Source file or route is not valid.");
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
                        if (!string.IsNullOrEmpty(destinationRoute))
                        {
                            // If the second argument directory doesn't exist, then create it.
                            if (!Directory.Exists(destinationRoute))
                            {
                                Directory.CreateDirectory(destinationRoute);
                                Console.WriteLine("Destination created.");
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

                    #region Copy

                    try
                    {
                        // The source and destination must exist.
                        if (File.Exists(sourceRoute) && Directory.Exists(destinationRoute))
                        {
                            decimal totalBytesTransferred=0;
                            decimal lastTotalBytesTransferred=0;
                            decimal totalFileSize;
                            var progress = 0;

                            // Run the copy process.
                            System.Threading.Tasks.Task.Run(() => 
                            {
                                FileCopy.Copy(sourceRoute, Path.Combine(destinationRoute, Path.GetFileName(sourceRoute)), 
                                    (long TotalFileSize, long TotalBytesTransferred, long StreamSize, long StreamBytesTransferred, uint dwStreamNumber, 
                                        FileCopy.CopyProgressCallbackReason dwCallbackReason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData) =>
                                {
                                    // Count the percentage of the done process.
                                    totalBytesTransferred = TotalBytesTransferred;
                                    totalFileSize = TotalFileSize;
                                    progress = (int)((totalBytesTransferred / totalFileSize) * 100);

                                    // Check if the files are transferred.
                                    if (TotalBytesTransferred == TotalFileSize)
                                    {
                                        exit = true;
                                        return FileCopy.CopyProgressResult.PROGRESS_CONTINUE;
                                    }

                                    return FileCopy.CopyProgressResult.PROGRESS_CONTINUE;

                                });
                            });


                            do
                            {
                                // Show the percentage of the done process.
                                Console.Write("{0,3} % bytes/sec: {1, -25}  \r", progress, (totalBytesTransferred - lastTotalBytesTransferred) * 2);
                                lastTotalBytesTransferred = totalBytesTransferred;

                                System.Threading.Thread.Sleep(500);
                            } while (!exit);

                        }
                        else
                        {
                            exit = true;
                            Console.WriteLine("Copy failed.");
                        }
                    }
                    catch (Exception ex)
                    {
                        exit = true;
                        Console.WriteLine(string.Format("An error occured at copy: {0}", ex.Message));
                    }

                    #endregion Copy
                    
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
